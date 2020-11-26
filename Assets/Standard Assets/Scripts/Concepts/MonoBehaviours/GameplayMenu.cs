using UnityEngine;
using System.Collections;
using Extensions;
using System;
using UnityEngine.Events;

namespace VisionGame
{
	public class GameplayMenu : SingletonMonoBehaviour<GameplayMenu>, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Transform trs;
		public float distanceFromCamera;
		public SphereCollider centerOptionRange;
		public Option centerOption;
		public float optionSeperationFromCenterOption;
		public Option[] options = new Option[0];
		public Transform selectorTrs;
		Option selectedOption;
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;

#if UNITY_EDITOR
		void OnValidate ()
		{
			int optionIndex = 0;
			for (float angle = 0; angle < 360; angle += 360f / options.Length)
			{
				options[optionIndex].trs.position = centerOptionRange.bounds.center + (trs.rotation * VectorExtensions.FromFacingAngle(angle)) * (centerOptionRange.bounds.size.x + optionSeperationFromCenterOption);
				optionIndex ++;
			}
		}
#endif

		public override void Awake ()
		{
			instance = this;
			gameObject.SetActive(false);
		}

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			leftGameplayMenuInput = InputManager.LeftGameplayMenuInput;
			rightGameplayMenuInput = InputManager.RightGameplayMenuInput;
			HandleSelecting ();
			HandleInteracting ();
			previousLeftGameplayMenuInput = leftGameplayMenuInput;
			previousRightGameplayMenuInput = rightGameplayMenuInput;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void HandleSelecting ()
		{
			print((selectorTrs.position - centerOptionRange.bounds.center).sqrMagnitude + " ," + (centerOptionRange.bounds.extents.x * centerOptionRange.bounds.extents.x));
			if ((selectorTrs.position - centerOptionRange.bounds.center).sqrMagnitude <= centerOptionRange.bounds.extents.x * centerOptionRange.bounds.extents.x)
				Select (centerOption);
			else
			{
				int optionIndex = 0;
				for (float angle = 0; angle < 360; angle += 360f / options.Length)
				{
					if (Vector3.Angle(trs.rotation * (selectorTrs.position - centerOptionRange.bounds.center), trs.rotation * VectorExtensions.FromFacingAngle(angle)) <= 360f / options.Length / 2)
						Select (options[optionIndex]);
					optionIndex ++;
				}
			}
		}

		void HandleInteracting ()
		{
			if (!selectedOption.Equals(default(Option)) && selectedOption.isInteractive)
			{
				if (selectorTrs == Player.Instance.leftHandTrs)
				{
					if (leftGameplayMenuInput && !previousLeftGameplayMenuInput)
						selectedOption.interactUnityEvent.Invoke();
				}
				else if (rightGameplayMenuInput && !previousRightGameplayMenuInput)
					selectedOption.interactUnityEvent.Invoke();
			}
		}

		void Select (Option option)
		{
			if (selectedOption.Equals(option))
				return;
			print("Selected " + option.trs.name);
			if (!selectedOption.Equals(default(Option)))
				Deselect (selectedOption);
			if (option.isInteractive)
			{
				selectedOption = option;
				selectedOption.deselectedGo.SetActive(false);
				selectedOption.selectedGo.SetActive(true);
			}
			else
				selectedOption = default(Option);
		}

		void Deselect (Option option)
		{
			print("Deselected " + option.trs.name);
			if (option.isInteractive)
			{
				option.selectedGo.SetActive(false);
				option.deselectedGo.SetActive(true);
			}
		}

		public void ViewOrbVision ()
		{
			if (selectorTrs == Player.Instance.leftHandTrs)
				StartCoroutine(ViewOrbVisionRoutine (Level.Instance.leftOrb));
			else
				StartCoroutine(ViewOrbVisionRoutine (Level.Instance.rightOrb));
		}

		IEnumerator ViewOrbVisionRoutine (Orb orb)
		{
			orb.camera.enabled = true;
			while (true)
			{
				if ((leftGameplayMenuInput && !previousLeftGameplayMenuInput) || (rightGameplayMenuInput && !previousRightGameplayMenuInput))
					break;
				yield return new WaitForEndOfFrame();
			}
			orb.camera.enabled = false;
		} 

		[Serializable]
		public struct Option
		{
#if UNITY_EDITOR
			public Transform trs;
#endif
			public GameObject selectedGo;
			public GameObject deselectedGo;
			public bool isInteractive;
			public GameObject uninteractiveGo;
			public UnityEvent interactUnityEvent;
		}
	}
}