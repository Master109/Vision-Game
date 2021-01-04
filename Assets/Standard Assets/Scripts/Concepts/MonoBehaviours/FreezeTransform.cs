using UnityEngine;
using System.Collections;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
using Extensions;
#endif

namespace VisionGame
{
	[ExecuteInEditMode]
	public class FreezeTransform : UpdateWhileEnabled
	{
		public Transform trs;
		public bool freezePosition;
		public bool freezeRelativePosition;
		public bool freezeRotation;
		public bool freezeScale;
		Vector3 previousRotation;
		Vector3 previousPosition;
		Vector3 previousRelativePosition;
		Vector3 previousWorldScale;

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				return;
			}
#endif
			UpdateVariables ();
			base.OnEnable ();
		}

#if UNITY_EDITOR
		void Update ()
		{
			DoUpdate ();
		}
#endif

		public override void DoUpdate ()
		{
			if (freezePosition)
				trs.position = previousPosition;
			else if (freezeRelativePosition)
				trs.position = trs.parent.position + previousRelativePosition;
			if (freezeRotation)
				trs.eulerAngles = previousRotation;
			if (freezeScale)
				trs.SetWorldScale (previousWorldScale);
			UpdateVariables ();
		}

		void UpdateVariables ()
		{
			previousRotation = trs.eulerAngles;
			previousPosition = trs.position;
			if (trs.parent != null)
				previousRelativePosition = trs.position - trs.parent.position;
			previousWorldScale = trs.lossyScale;
		}
	}
}