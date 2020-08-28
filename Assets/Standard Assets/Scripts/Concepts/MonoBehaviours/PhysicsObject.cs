using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class PhysicsObject : Spawnable
	{
		public Rigidbody rigid;
		[HideInInspector]
		public Vector3 velocity;
		[HideInInspector]
		public Vector3 angularVelocity;
		public float overlapAmountToGetStuck;
		public static List<KeyValuePair<Collider, Collider>> ignoreCollisions = new List<KeyValuePair<Collider, Collider>>();
		bool isStuck;

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
			Physics.Simulate(0);
			if (isStuck)
			{
				return;
			}
			else
			{
				// rigid.isKinematic = false;
				if (rigid == null)
					rigid = gameObject.AddComponent<Rigidbody>();
			}
			rigid.velocity = trs.TransformDirection(velocity);
			rigid.angularVelocity = trs.TransformDirection(angularVelocity);
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			velocity = trs.InverseTransformDirection(rigid.velocity);
			angularVelocity = trs.InverseTransformDirection(rigid.angularVelocity);
		}

		void OnCollisionEnter (Collision coll)
		{
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				if (contactPoint.separation <= -overlapAmountToGetStuck)
				{
					isStuck = true;
					if (!ignoreCollisions.Contains(new KeyValuePair<Collider, Collider>(contactPoint.otherCollider, contactPoint.thisCollider)))
					{
						ignoreCollisions.Add(new KeyValuePair<Collider, Collider>(contactPoint.thisCollider, contactPoint.otherCollider));
						// rigid.isKinematic = true;
						// coll.rigidbody.isKinematic = true;
						Destroy(rigid);
						Destroy(coll.rigidbody);
						PhysicsObject physicsObject = GameManager.GetSingleton<ObjectPool>().SpawnComponent<PhysicsObject> (prefabIndex, default(Vector3), default(Quaternion), trs.parent);
						trs.SetParent(physicsObject.trs);
					}
					return;
				}
			}
		}
	}
}