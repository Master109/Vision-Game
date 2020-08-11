using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace VisionGame
{
	[Serializable]
	public class TemporaryActiveText : TemporaryActiveGameObject
	{
		public _Text text;
		public float durationPerCharacter;
		
		public override IEnumerator DoRoutine ()
		{
			if (text != null)
				duration = text.text.text.Length * durationPerCharacter;
			yield return base.DoRoutine ();
		}
	}
}