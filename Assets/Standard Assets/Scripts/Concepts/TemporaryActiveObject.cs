﻿using UnityEngine;
using System.Collections;
using System;
using Extensions;

namespace VisionGame
{
	[Serializable]
	public class TemporaryActiveObject<T>
	{
		public T obj;
		public float duration;
		public bool realtime;
		public static TemporaryActiveObject<T>[] activeInstances = new TemporaryActiveObject<T>[0];

		public virtual void Do ()
		{
			GameManager.Instance.StartCoroutine(DoRoutine ());
		}

		public virtual void Stop ()
		{
			GameManager.Instance.StopCoroutine(DoRoutine ());
		}
		
		public virtual IEnumerator DoRoutine ()
		{
			Activate ();
			if (realtime)
				yield return new WaitForSecondsRealtime(duration);
			else
				yield return new WaitForSeconds(duration);
			Deactivate ();
		}

		public virtual void Activate ()
		{
			if (activeInstances.Contains(this))
				return;
			activeInstances = activeInstances.Add(this);
		}

		public virtual void Deactivate ()
		{
			activeInstances = activeInstances.Remove(this);
		}
	}
}