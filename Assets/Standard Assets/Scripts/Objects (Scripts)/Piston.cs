using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Piston : UpdateWhileEnabled
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Transform trs;
		public Transform axelParent;
		public Transform axelTrs;
		public float axelOffset;
		public float extendAxel;
		public float childObjectsOffset;
		public Transform childObjectsParent;
		public IgnoreCollisionEntry[] ignoreCollisionEntries = new IgnoreCollisionEntry[0];
		public Rigidbody rigid;
		public float moveDistance;
		public float moveSpeed;
		public bool repeat;
		[HideInInspector]
		public bool moveTowardsEnd = true;
		public float distanceToAxelEnd;
#if UNITY_EDITOR
		public PhysicsObject physicsObject;
#endif
		public float stopDistance;
		LineSegment3D lineSegment;
		[HideInInspector]
		public RigidbodyConstraints rigidConstraints;

		void Awake ()
		{
			for (int i = 0; i < ignoreCollisionEntries.Length; i ++)
			{
				IgnoreCollisionEntry ignoreCollisionEntry = ignoreCollisionEntries[i];
				Physics.IgnoreCollision(ignoreCollisionEntry.collider, ignoreCollisionEntry.otherCollider, true);
			}
#if UNITY_EDITOR
			rigidConstraints = rigid.constraints;
			if (Application.isPlaying)
#endif
				rigid.constraints = RigidbodyConstraints.FreezeAll;
			lineSegment = new LineSegment3D(axelTrs.position, axelTrs.position + (axelParent.forward * moveDistance / 2));
		}

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				SetMass ();
				return;
			}
#endif
			rigid.constraints = rigidConstraints;
			base.OnEnable ();
		}

#if UNITY_EDITOR
		void OnValidate ()
		{
			if (Application.isPlaying)
				return;
			lineSegment = new LineSegment3D(trs.position + (trs.forward * (-Mathf.Abs(moveDistance) / 2 - axelOffset + distanceToAxelEnd)), trs.position + (trs.forward * (Mathf.Abs(moveDistance) / 2 - axelOffset + distanceToAxelEnd)));
			axelParent.position = lineSegment.start + lineSegment.GetDirection() * extendAxel / 2;
			axelParent.localScale = axelParent.localScale.SetZ(lineSegment.GetLength() + extendAxel);
			childObjectsParent.position = trs.position + (lineSegment.GetDirection() * (childObjectsOffset + distanceToAxelEnd));
			childObjectsParent.SetWorldScale (Vector3.one);
			SetMass ();
		}

		public void SetMass ()
		{
			rigid.mass = axelTrs.lossyScale.x * axelTrs.lossyScale.y * axelTrs.lossyScale.z;
			for (int i = 0; i < physicsObject.childrenParent.childCount; i ++)
			{
				Transform child = physicsObject.childrenParent.GetChild(i);
				Piston piston = child.GetComponent<Piston>();
				if (piston != null)
				{
					piston.SetMass ();
					rigid.mass += piston.rigid.mass;
				}
				else
					rigid.mass += child.lossyScale.x * child.lossyScale.y * child.lossyScale.z;
			}
		}
#endif

		public void DoUpdate ()
		{
			axelTrs.position = lineSegment.ClosestPoint(axelTrs.position);
			float directedDistanceAlongParallel = lineSegment.GetDirectedDistanceAlongParallel(axelTrs.position);
			bool isAtStart = directedDistanceAlongParallel <= stopDistance;
			bool isAtEnd = directedDistanceAlongParallel >= lineSegment.GetLength() - stopDistance;
			if (isAtStart || isAtEnd)
			// if (axelTrs.position == lineSegment.start || axelTrs.position == lineSegment.end)
			// if (directedDistanceAlongParallel <= stopDistance || directedDistanceAlongParallel >= lineSegment.GetLength() - stopDistance || axelTrs.position == lineSegment.start || axelTrs.position == lineSegment.end)
			{
				if (repeat)
					moveTowardsEnd = isAtStart;
				else
				{
					GameManager.updatables = GameManager.updatables.Remove(this);
					return;
				}
			}
			if (moveTowardsEnd)
				rigid.velocity = lineSegment.GetDirection() * moveSpeed;
			else
				rigid.velocity = -lineSegment.GetDirection() * moveSpeed;
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
			rigid.constraints = RigidbodyConstraints.FreezeAll;
			base.OnDisable ();
		}

		void OnApplicationQuit ()
		{
			rigid.constraints = rigidConstraints;
		}

		[Serializable]
		public struct IgnoreCollisionEntry
		{
			public Collider collider;
			public Collider otherCollider;
		}
	}
}