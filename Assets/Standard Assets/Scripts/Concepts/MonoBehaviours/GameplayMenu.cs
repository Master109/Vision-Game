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

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			if ((selectorTrs.position - centerOptionRange.bounds.center).sqrMagnitude <= centerOptionRange.bounds.extents.x)
			{
				
			}
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
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
		public class Option
		{
			public GameObject selectedGo;
			public GameObject deselectedGo;
			public GameObject uninteractiveGo;
			public UnityEvent interactUnityEvent;
		}
	}
}