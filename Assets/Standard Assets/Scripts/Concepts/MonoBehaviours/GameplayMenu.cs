using UnityEngine;
using System.Collections;
using Extensions;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace VisionGame
{
	public class GameplayMenu : SingletonUpdateWhileEnabled<GameplayMenu>
	{
		public bool isTopTier;
		public Transform trs;
		public float distanceFromCamera;
		public SphereCollider centerOptionRange;
		public Option centerOption;
		public float optionSeperationFromCenterOption;
		public Option[] options = new Option[0];
		public Transform selectorTrs;
		[HideInInspector]
		public bool interactive;
		Option selectedOption;
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;
		bool gameplayMenuInput;
		bool previousGameplayMenuInput;

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
			if (isTopTier)
				instance = this;
			gameObject.SetActive(false);
		}

		public override void DoUpdate ()
		{
			leftGameplayMenuInput = InputManager.LeftGameplayMenuInput;
			rightGameplayMenuInput = InputManager.RightGameplayMenuInput;
			gameplayMenuInput = InputManager.GameplayMenuInput;
			HandleSelecting ();
			HandleInteracting ();
			previousLeftGameplayMenuInput = leftGameplayMenuInput;
			previousRightGameplayMenuInput = rightGameplayMenuInput;
			previousGameplayMenuInput = gameplayMenuInput;
		}

		void HandleSelecting ()
		{
			Plane plane = new Plane(trs.forward, centerOptionRange.bounds.center);
			float hitDistance;
			Ray ray = new Ray(selectorTrs.position, selectorTrs.forward);
			if (plane.Raycast(ray, out hitDistance))
			{
				Vector3 hitPoint = ray.GetPoint(hitDistance);
				List<Transform> optionTransforms = new List<Transform>();
				for (int i = 0; i < options.Length; i ++)
				{
					Option option = options[i];
					optionTransforms.Add(option.trs);
				}
				optionTransforms.Add(centerOption.trs);
				Transform closestOptionTrs = TransformExtensions.GetClosestTransform_3D(optionTransforms.ToArray(), hitPoint);
				for (int i = 0; i < optionTransforms.Count - 1; i ++)
				{
					Transform optionTrs = optionTransforms[i];
					if (closestOptionTrs == optionTrs)
					{
						Select (options[i]);
						return;
					}
				}
				Select (centerOption);
				// 	int optionIndex = 0;
				// 	for (float angle = 0; angle < 360; angle += 360f / options.Length)
				// 	{
				// 		Option option = options[optionIndex];
				// 		if (Vector3.Angle(trs.rotation * (selectorTrs.position - centerOptionRange.bounds.center), trs.rotation * VectorExtensions.FromFacingAngle(angle)) <= 360f / options.Length / 2)
				// 			Select (option);
				// 		optionIndex ++;
				// 	}
				// }
			}
			// List<Transform> optionTransforms = new List<Transform>();
			// for (int i = 0; i < options.Length; i ++)
			// {
			// 	Option option = options[i];
			// 	optionTransforms.Add(option.trs);
			// }
			// optionTransforms.Add(centerOption.trs);
			// Transform closestOptionTrs = selectorTrs.GetClosestTransform_3D(optionTransforms.ToArray());
			// for (int i = 0; i < optionTransforms.Count; i ++)
			// {
			// 	Transform optionTrs = optionTransforms[i];
			// 	if (closestOptionTrs == optionTrs)
			// 	{
			// 		Select (options[i]);
			// 		return;
			// 	}
			// }
			// Select (centerOption);
		}

		void HandleInteracting ()
		{
			if (!selectedOption.Equals(default(Option)) && selectedOption.isInteractive)
			{
				if (selectorTrs == Player.instance.leftHandTrs && !leftGameplayMenuInput && previousLeftGameplayMenuInput)
					selectedOption.interactUnityEvent.Invoke();
				else if (selectorTrs == Player.instance.leftHandTrs && !rightGameplayMenuInput && previousRightGameplayMenuInput)
					selectedOption.interactUnityEvent.Invoke();
				else if (!gameplayMenuInput && previousGameplayMenuInput)
					selectedOption.interactUnityEvent.Invoke();
			}
		}

		void Select (Option option)
		{
			if (selectedOption.Equals(option))
				return;
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
			if (option.isInteractive)
			{
				option.selectedGo.SetActive(false);
				option.deselectedGo.SetActive(true);
			}
		}

		public void RestartLevel ()
		{
			Player.instance.invulnerable = true;
			GameManager.instance.ReloadActiveScene ();
		}

		public void ViewOrbVision ()
		{
			gameObject.SetActive(false);
			interactive = false;
			StopAllCoroutines();
			if (selectorTrs == Player.instance.leftHandTrs)
				GameManager.Instance.StartCoroutine(ViewOrbVisionRoutine (Level.Instance.leftOrb));
			else
				GameManager.Instance.StartCoroutine(ViewOrbVisionRoutine (Level.Instance.rightOrb));
		}

		IEnumerator ViewOrbVisionRoutine (Orb orb)
		{
			orb.camera.enabled = true;
			while (true)
			{
				leftGameplayMenuInput = InputManager.LeftGameplayMenuInput;
				rightGameplayMenuInput = InputManager.RightGameplayMenuInput;
				gameplayMenuInput = InputManager.GameplayMenuInput;
				if ((leftGameplayMenuInput && !previousLeftGameplayMenuInput) || (rightGameplayMenuInput && !previousRightGameplayMenuInput) || (gameplayMenuInput && !previousGameplayMenuInput))
					break;
				previousLeftGameplayMenuInput = leftGameplayMenuInput;
				previousRightGameplayMenuInput = rightGameplayMenuInput;
				previousGameplayMenuInput = gameplayMenuInput;
				yield return new WaitForEndOfFrame();
			}
			orb.camera.enabled = false;
			interactive = true;
		}

		[Serializable]
		public struct Option
		{
			public Transform trs;
			public GameObject selectedGo;
			public GameObject deselectedGo;
			public bool isInteractive;
			public GameObject uninteractiveGo;
			public UnityEvent interactUnityEvent;
		}
	}
}