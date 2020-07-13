using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
using Extensions;

namespace BeatKiller
{
	[Serializable]
	public class TemporaryActiveGameObject : TemporaryActiveObject<GameObject>
	{
		public virtual void Activate ()
        {
            if (activeInstances.Contains(this))
                return;
            if (obj != null)
                obj.SetActive(true);
            activeInstances = activeInstances.Add(this);
        }

        public virtual void Deactivate ()
        {
            if (obj != null)
                obj.SetActive(false);
            activeInstances = activeInstances.Remove(this);
        }
	}
}

