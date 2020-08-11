using System;
using UnityEngine;

namespace VisionGame
{
	[RequireComponent(typeof(Spawnable))]
	public class SpawnableHazard : Hazard//, ISpawnable
	{
		public Spawnable spawnable;
//		public int prefabIndex;
//		public int PrefabIndex
//		{
//			get
//			{
//				return prefabIndex;
//			}
//		}
		public TemporaryActiveGameObject temporaryActiveObject;

		public virtual void Start ()
		{
			if (temporaryActiveObject != null)
				temporaryActiveObject.Do ();
		}

		public virtual void OnDisable ()
		{
			Destroy(gameObject);
		}
	}
}