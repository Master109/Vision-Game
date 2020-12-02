using UnityEngine;

namespace VisionGame
{
	public class Level : SingletonMonoBehaviour<Level>
	{
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
		}
	}
}