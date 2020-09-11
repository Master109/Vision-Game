using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

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
		public Rigidbody rigid;
		public float moveDistance;
		public float moveSpeed;
		public bool repeat;
		float distanceAlongLineSegment;
		LineSegment3D lineSegment;
		bool moveTowardsEnd;

		void Awake ()
		{
			for (int i = 0; i < ignoreCollisionEntries.Length; i ++)
			{
				IgnoreCollisionEntry ignoreCollisionEntry = ignoreCollisionEntries[i];
				Physics.IgnoreCollision(ignoreCollisionEntry.collider, ignoreCollisionEntry.otherCollider);
			}
			lineSegment = new LineSegment3D(axelTrs.position, axelTrs.position + axelTrs.forward * (moveDistance - axelOffset - extendAxel));
			// lineSegment = new LineSegment3D(trs.position + (trs.forward * (-.5f - moveDistance + axelOffset)), trs.position + (trs.forward * (.5f + extendAxel + axelOffset)));
		}

		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

#if UNITY_EDITOR
		void OnValidate ()
		{
			lineSegment = new LineSegment3D(trs.position + (trs.forward * (-.5f - moveDistance + axelOffset)), trs.position + (trs.forward * (.5f + extendAxel + axelOffset)));
			// lineSegment.DrawGizmos(Color.green);
			axelTrs.position = lineSegment.start;
			axelTrs.localScale = axelTrs.localScale.SetZ(lineSegment.GetLength());
			childObjectsParent.position = lineSegment.end + (lineSegment.GetDirection() * childObjectsOffset);
			childObjectsParent.SetWorldScale(Vector3.one);
			// Vector3 lineSegmentOffset = Vector3.right;
			// lineSegment = new LineSegment3D(axelTrs.position, axelTrs.position + axelTrs.forward * (moveDistance - axelOffset - extendAxel));
			// lineSegment = lineSegment.Move(lineSegmentOffset);
			// lineSegment.DrawGizmos(Color.black);
		}
#endif

		public void DoUpdate ()
		{
			// axelTrs.position = lineSegment.ClosestPoint(axelTrs.position);
			// if (axelTrs.position == lineSegment.start || axelTrs.position == lineSegment.end)
			// {
			// 	if (repeat)
			// 		moveTowardsEnd = !moveTowardsEnd;
			// 	else
			// 	{
			// 		GameManager.updatables = GameManager.updatables.Remove(this);
			// 		return;
			// 	}
			// }
			// if (moveTowardsEnd)
			// 	rigid.velocity = lineSegment.GetDirection() * moveSpeed;
			// else
			// 	rigid.velocity = -lineSegment.GetDirection() * moveSpeed;
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