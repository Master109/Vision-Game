using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisionGame
{
	public class Spawnable : MonoBehaviour, ISpawnable
	{
		public Transform trs;
		public int prefabIndex;
		public int PrefabIndex
		{
			get
			{
				return prefabIndex;
			}
		}
	}
}