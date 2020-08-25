using UnityEngine;
using Extensions;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class PhysicsObject : MonoBehaviour
	{
		public Transform trs;
		public Rigidbody rigid;
		[HideInInspector]
		public Vector3 velocity;

		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (rigid == null)
					rigid = GetComponent<Rigidbody>();
				velocity = Vector3.zero;
				return;
			}
#endif
			rigid.velocity = trs.TransformDirection(velocity);
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			velocity = trs.InverseTransformDirection(rigid.velocity);
		}
	}
}