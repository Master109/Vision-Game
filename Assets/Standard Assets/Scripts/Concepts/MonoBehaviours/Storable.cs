using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisionGame
{
	public class Storable : MonoBehaviour, IStorable
	{
		public Transform Trs
		{
			get
			{
				return trs;
			}
		}
		public Transform trs;
	}
}