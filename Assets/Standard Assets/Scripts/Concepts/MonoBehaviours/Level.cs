using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class Level : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public Orb leftOrb;
		public Orb rightOrb;
		public Orb[] orbs = new Orb[0];

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			if (InputManager.RestartInput)
				GameManager.GetSingleton<GameManager>().ReloadActiveScene ();
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}