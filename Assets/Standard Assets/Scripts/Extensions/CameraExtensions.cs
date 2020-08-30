using UnityEngine;
using System;

namespace Extensions
{
	public static class CameraExtensions
	{
		public static bool ViewFrustumContainsPoint (this Camera camera, Vector3 point)
		{
			Vector2 viewportPoint = camera.WorldToViewportPoint(point);
			return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
		}

		public static bool ClosestPointOnViewFrustum (this Camera camera, Transform trs, Vector3 point, out Vector3 output)
		{
			Vector3 viewportPoint = camera.WorldToViewportPoint(point);
			if (viewportPoint.x == .5f && viewportPoint.y == .5f)
			{
				output = VectorExtensions.NULL3;
				return false;
			}
			else
			{
				viewportPoint = viewportPoint.ClampComponents(Vector2.zero, Vector2.one);
				if (Mathf.Abs(viewportPoint.x - .5f) > Mathf.Abs(viewportPoint.y - .5f))
					viewportPoint.x = (viewportPoint.x > .5f).GetHashCode();
				else
					viewportPoint.y = (viewportPoint.y > .5f).GetHashCode();
			}
			Ray ray = camera.ViewportPointToRay(viewportPoint);
			Vector3 localPositionOfPoint = trs.InverseTransformPoint(point);
			Vector3 forwardVector = Vector3.Project(trs.forward * localPositionOfPoint.magnitude, localPositionOfPoint);
			float distance = Vector3.Project(forwardVector, ray.direction).magnitude;
			output = ray.GetPoint(distance);
			return true;
		}
	}
}