using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisionGame
{
	public class DisableObject : MonoBehaviour
	{
		public virtual void OnDisable ()
		{
			enabled = false;
		}
		
		public virtual void Awake ()
		{
			if (!enabled)
				return;
			gameObject.SetActive(false);
		}
	}
}