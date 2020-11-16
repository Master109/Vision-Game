using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class Level : SingletonMonoBehaviour<Level>, IUpdatable
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
		public Transform trs;
		public Light[] lights = new Light[0];

		void OnEnable ()
		{
			trs.localScale = Vector3.one * GameManager.Instance.distanceScale;
			for (int i = 0; i < lights.Length; i ++)
			{
				Light light = lights[i];
				light.range *= GameManager.instance.distanceScale;
			}
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			if (InputManager.RestartInput)
				GameManager.Instance.ReloadActiveScene ();
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}