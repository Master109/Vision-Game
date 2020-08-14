using UnityEngine;

namespace VisionGame
{
	public class StoreVelocity : MonoBehaviour
	{
		public Transform trs;
		public Rigidbody rigid;
		[HideInInspector]
		public Vector3 velocity;

		void OnEnable ()
		{
			rigid.velocity = trs.TransformVector(velocity);
		}

		void OnDisable ()
		{
			velocity = trs.InverseTransformVector(rigid.velocity);
		}
	}
}