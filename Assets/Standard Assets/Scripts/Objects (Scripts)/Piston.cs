using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class Piston : ObjectWithWaypoints
	{
		public Rigidbody rigid;
		public float moveSpeed;
		public bool repeat;
		[HideInInspector]
		public LineSegment3D[] allLines = new LineSegment3D[0];
		public bool moveTowardsEnd;
		[HideInInspector]
		public Vector3 previousPosition;

		public override void OnEnable ()
		{
			int lineCount = wayPoints.Length - 1;
			allLines = new LineSegment3D[lineCount];
			for (int i = 0; i < lineCount; i ++)
				allLines[i] = new LineSegment3D(wayPoints[i].position, wayPoints[i + 1].position);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public override void DoUpdate ()
		{
			if (trs.position == previousPosition)
			{
				if (repeat)
					moveTowardsEnd = !moveTowardsEnd;
				else
					GameManager.updatables = GameManager.updatables.Remove(this);
			}
			previousPosition = trs.position;
			float distanceToClosestPoint = (allLines[0].ClosestPoint(trs.position) - trs.position).sqrMagnitude;
			float closestDistance = distanceToClosestPoint;
			LineSegment3D stuckOnLine = allLines[0];
			for (int i = 1; i < allLines.Length; i ++)
			{
				LineSegment3D lineSegment = allLines[i];
				distanceToClosestPoint = (lineSegment.ClosestPoint(trs.position) - trs.position).sqrMagnitude;
				if (distanceToClosestPoint < closestDistance)
				{
					closestDistance = distanceToClosestPoint;
					stuckOnLine = lineSegment;
				}
			}
			if (moveTowardsEnd)
				trs.position = stuckOnLine.GetPointWithDirectedDistance(stuckOnLine.GetDirectedDistanceAlongParallel(trs.position) + moveSpeed * Time.deltaTime);
			else
				trs.position = stuckOnLine.GetPointWithDirectedDistance(stuckOnLine.GetDirectedDistanceAlongParallel(trs.position) - moveSpeed * Time.deltaTime);
			trs.position = stuckOnLine.ClosestPoint(trs.position);
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}