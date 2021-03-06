using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class PhysicsObject : Spawnable
	{
		public static List<Rigidbody> stuckRigidbodies = new List<Rigidbody>();
		public Transform childrenParent;
		public Transform capturedVisualizerTrs;
		public Rigidbody rigid;
		public new Collider collider;
		[HideInInspector]
		public Vector3 velocity;
		[HideInInspector]
		public Vector3 angularVelocity;
		public float overlapAmountToGetStuck;
		[HideInInspector]
		public bool isStuck;
		[HideInInspector]
		public int frameIWasEnabled;
		public bool isGrabbable;
		public LayerMask whatICollideWith;

		public virtual void OnEnable ()
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
			if (rigid == null)
			{
				Destroy(this);
				return;
			}
			frameIWasEnabled = GameManager.framesSinceLoadedScene;
			if (isStuck)
				return;
			else
			{
				rigid.isKinematic = false;
				stuckRigidbodies.Remove(rigid);
			}
			rigid.velocity = trs.TransformDirection(velocity);
			rigid.angularVelocity = trs.TransformDirection(angularVelocity);
		}

		public virtual void OnDisable ()
		{
			isStuck = false;
			if (rigid != null)
				rigid.isKinematic = false;
		}

		public virtual void OnCollisionEnter (Collision coll)
		{
			if (GameManager.framesSinceLoadedScene > frameIWasEnabled)
				return;
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				if (contactPoint.separation <= -overlapAmountToGetStuck)
				{
					Breakable breakable = coll.gameObject.GetComponentInParent<Breakable>();
					if (breakable != null)
					{
						breakable.Break (coll.relativeVelocity);
						return;
					}
					isStuck = true;
					if (coll.rigidbody == null)
					{
						stuckRigidbodies.Add(rigid);
						rigid.isKinematic = true;
						trs.SetParent(coll.transform);
					}
					else
					{
						if (coll.gameObject == Player.instance.gameObject)
							Destroy(coll.gameObject);
						else if (!stuckRigidbodies.Contains(coll.rigidbody))
						{
							stuckRigidbodies.Add(coll.rigidbody);
							stuckRigidbodies.Add(rigid);
							rigid.isKinematic = true;
							coll.rigidbody.isKinematic = true;
							PhysicsObject physicsObject = coll.rigidbody.GetComponent<PhysicsObject>();
							// PhysicsObject physicsObject = ObjectPool.instance.SpawnComponent<PhysicsObject> (prefabIndex, default(Vector3), default(Quaternion), trs.parent);
							trs.SetParent(physicsObject.childrenParent);
						}
					}
					return;
				}
			}
		}

		public virtual Collider[] OverlapCollider ()
		{
			List<Collider> output = new List<Collider>();
			SphereCollider sphereCollider = collider as SphereCollider;
			if (sphereCollider != null)
				output = new List<Collider>(Physics.OverlapSphere(sphereCollider.bounds.center, sphereCollider.bounds.extents.x, whatICollideWith));
			else
			{
				BoxCollider boxCollider = collider as BoxCollider;
				if (boxCollider != null)
					output = new List<Collider>(Physics.OverlapBox(boxCollider.bounds.center, boxCollider.size / 2, trs.rotation, whatICollideWith));
			}
			output.Remove(collider);
			return output.ToArray();
		}
	}
}