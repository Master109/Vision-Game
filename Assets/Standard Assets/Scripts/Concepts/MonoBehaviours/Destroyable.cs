using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisionGame
{
	public class Destroyable : MonoBehaviour, IDestroyable
	{
		public GameObject Go
		{
			get
			{
				return gameObject;
			}
		}
	}
}