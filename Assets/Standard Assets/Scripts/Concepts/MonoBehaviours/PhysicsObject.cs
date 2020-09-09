using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class PhysicsObject : Spawnable
	{
		public Transform childrenParent;
		public Rigidbody rigid;
		public new Collider collider;
		[HideInInspector]
		public Vector3 velocity;
		[HideInInspector]
		public Vector3 angularVelocity;
		public float overlapAmountToGetStuck;
		public static List<Rigidbody> stuckRigidbodies = new List<Rigidbody>();
		bool isStuck;
		int frameIWasEnabled;

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
				childrenParent.localScale = Vector3.one.Divide(trs.lossyScale);
				BoxCollider boxCollider = collider as BoxCollider;
				if (boxCollider != null)
					boxCollider.size = trs.lossyScale;
				return;
			}
#endif
			if (rigid == null)
			{
				Destroy(this);
				return;
			}
			frameIWasEnabled = GameManager.framesSinceLoadedScene;
			Physics.Simulate(float.Epsilon);
			if (isStuck)
			{
				return;
			}
			else
			{
				rigid.isKinematic = false;
				isStuck = false;
				stuckRigidbodies.Remove(rigid);
				// if (rigid == null)
				// 	rigid = gameObject.AddComponent<Rigidbody>();
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
			if (GameManager.framesSinceLoadedScene > frameIWasEnabled)
				return;
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				if (contactPoint.separation <= -overlapAmountToGetStuck)
				{
					isStuck = true;
					if (coll.rigidbody == null)
					{
						stuckRigidbodies.Add(rigid);
						rigid.isKinematic = true;
						// Destroy(rigid);
						trs.SetParent(coll.transform);
					}
					else if (!stuckRigidbodies.Contains(coll.rigidbody))
					{
						stuckRigidbodies.Add(coll.rigidbody);
						stuckRigidbodies.Add(rigid);
						rigid.isKinematic = true;
						coll.rigidbody.isKinematic = true;
						// Destroy(rigid);
						// Destroy(coll.rigidbody);
						PhysicsObject physicsObject = GameManager.GetSingleton<ObjectPool>().SpawnComponent<PhysicsObject> (prefabIndex, default(Vector3), default(Quaternion), trs.parent);
						trs.SetParent(physicsObject.childrenParent);
					}
					return;
				}
			}
		}
	}
}