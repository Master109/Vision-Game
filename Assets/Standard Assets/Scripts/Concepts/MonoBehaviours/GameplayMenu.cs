using UnityEngine;
using System.Collections.Generic;
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
		public SphereCollider centerOptionRange;
		public Option centerOption;
		public Option[] options = new Option[0];
		public Transform selectorTrs;
		Option selectedOption;
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;

		void Awake ()
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
			if ((selectorTrs.position - centerOptionRange.bounds.center).sqrMagnitude <= centerOptionRange.bounds.extents.x)
			{
				Select (selectedOption);
			}
			else
			{
				
			}
		}

		void HandleInteracting ()
		{
			if (!selectedOption.Equals(default(Option)))
			{
				if (selectorTrs == Player.Instance.leftHandTrs)
				{
					if (leftGameplayMenuInput && !previousLeftGameplayMenuInput)
					{

					}
				}
				else
				{
					if (rightGameplayMenuInput && !previousRightGameplayMenuInput)
					{
						
					}
				}
			}
		}

		void Select (Option option)
		{
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

		// void HandleOrbViewing ()
		// {
		// 	if (InputManager.LeftOrbViewInput)
		// 	{
		// 		// Level.Instance.leftOrb.camera.depth = 1;
		// 		Level.Instance.leftOrb.camera.enabled = true;
		// 		return;
		// 	}
		// 	else
		// 		// Level.Instance.leftOrb.camera.depth = -1;
		// 		Level.Instance.leftOrb.camera.enabled = false;
		// 	if (InputManager.RightOrbViewInput)
		// 		// Level.Instance.rightOrb.camera.depth = 1;
		// 		Level.Instance.rightOrb.camera.enabled = true;
		// 	else
		// 		// Level.Instance.rightOrb.camera.depth = -1;
		// 		Level.Instance.rightOrb.camera.enabled = false;
		// }

		[Serializable]
		public struct Option
		{
			public GameObject selectedGo;
			public GameObject deselectedGo;
			public bool isInteractive;
			public GameObject uninteractiveGo;
			public UnityEvent interactUnityEvent;
		}
	}
}