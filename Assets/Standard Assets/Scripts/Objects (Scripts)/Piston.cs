using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;
using Extensions;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Piston : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Transform trs;
		public Transform axelTrs;
		public float axelOffset;
		public float extendAxel;
		public float childObjectsOffset;
		public Transform childObjectsParent;
		public IgnoreCollisionEntry[] ignoreCollisionEntries = new IgnoreCollisionEntry[0];
		// public Rigidbody rigid;
		public float moveDistance;
		public float moveSpeed;
		public bool repeat;
		float distanceAlongLineSegment;
		LineSegment3D lineSegment;
		bool moveTowardsEnd = true;

		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
            for (int i = 0; i < ignoreCollisionEntries.Length; i ++)
			{
                IgnoreCollisionEntry ignoreCollisionEntry = ignoreCollisionEntries[i];
                Physics.IgnoreCollision(ignoreCollisionEntry.collider, ignoreCollisionEntry.otherCollider);
			}
			lineSegment = new LineSegment3D(axelTrs.position, axelTrs.position + axelTrs.forward * (moveDistance - axelOffset - extendAxel));
			GameManager.updatables = GameManager.updatables.Add(this);
		}

#if UNITY_EDITOR
		void OnValidate ()
		{
			lineSegment = new LineSegment3D(trs.position + (trs.forward * (-.5f - moveDistance + axelOffset)), trs.position + (trs.forward * (.5f + extendAxel + axelOffset)));
			axelTrs.position = lineSegment.start;
			axelTrs.localScale = axelTrs.localScale.SetZ(lineSegment.GetLength());
			childObjectsParent.position = lineSegment.end + (lineSegment.GetDirection() * childObjectsOffset);
			childObjectsParent.SetWorldScale(Vector3.one);
		}
#endif

		public void DoUpdate ()
		{
			// if (moveTowardsEnd)
			// 	rigid.velocity = lineSegment.GetDirection() * moveSpeed;
			// else
			// 	rigid.velocity = -lineSegment.GetDirection() * moveSpeed;
			// axelTrs.position = lineSegment.ClosestPoint(axelTrs.position);
			if (moveTowardsEnd)
				distanceAlongLineSegment += moveSpeed * Time.deltaTime;
			else
				distanceAlongLineSegment -= moveSpeed * Time.deltaTime;
			distanceAlongLineSegment = Mathf.Clamp(distanceAlongLineSegment, 0, lineSegment.GetLength());
			axelTrs.position = lineSegment.GetPointWithDirectedDistance(distanceAlongLineSegment);
			Physics.Simulate(float.Epsilon);
			distanceAlongLineSegment = lineSegment.GetDirectedDistanceAlongParallel(axelTrs.position);
			distanceAlongLineSegment = Mathf.Clamp(distanceAlongLineSegment, 0, lineSegment.GetLength());
			if (distanceAlongLineSegment == 0 || distanceAlongLineSegment == lineSegment.GetLength())
			{
				if (repeat)
					moveTowardsEnd = !moveTowardsEnd;
				else if (distanceAlongLineSegment == lineSegment.GetLength())
					GameManager.updatables = GameManager.updatables.Remove(this);
			}
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		[Serializable]
		public class IgnoreCollisionEntry
		{
			public Collider collider;
			public Collider otherCollider;
		}
	}
}