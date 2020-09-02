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
		LineSegment3D lineSegment;
		Vector3 previousPosition;
		bool moveTowardsEnd = true;

		public override void OnEnable ()
		{
			lineSegment = new LineSegment3D(wayPoints[0].position, wayPoints[1].position);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public override void DoUpdate ()
		{
			if (trs.position == previousPosition)
			{
				if (repeat)
					moveTowardsEnd = !moveTowardsEnd;
				else if (trs.position == lineSegment.end)
					GameManager.updatables = GameManager.updatables.Remove(this);
			}
			if (moveTowardsEnd)
				rigid.velocity = lineSegment.GetDirection() * moveSpeed;
			else
				rigid.velocity = -lineSegment.GetDirection() * moveSpeed;
			trs.position = lineSegment.ClosestPoint(trs.position);
			previousPosition = trs.position;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}