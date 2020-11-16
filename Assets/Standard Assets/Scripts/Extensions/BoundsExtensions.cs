using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class BoundsExtensions
	{
		public static Bounds NULL = new Bounds(VectorExtensions.NULL3, VectorExtensions.NULL3);

		public static bool IsEncapsulating (this Bounds b1, Bounds b2, bool equalBoundsRetunsTrue)
		{
			if (equalBoundsRetunsTrue)
			{
				bool minIsOk = b1.min.x <= b2.min.x && b1.min.y <= b2.min.y && b1.min.z <= b2.min.z;
				bool maxIsOk = b1.min.x >= b2.min.x && b1.min.y >= b2.min.y && b1.min.z >= b2.min.z;
				return minIsOk && maxIsOk;
			}
			else
			{
				bool minIsOk = b1.min.x < b2.min.x && b1.min.y < b2.min.y && b1.min.z < b2.min.z;
				bool maxIsOk = b1.max.x > b2.max.x && b1.max.y > b2.max.y && b1.max.z > b2.max.z;
				return minIsOk && maxIsOk;
			}
		}
		
		public static Bounds Combine (this Bounds[] boundsArray)
		{
			Bounds output = boundsArray[0];
			for (int i = 1; i < boundsArray.Length; i ++)
			{
				Bounds bounds = boundsArray[i];
				if (bounds.min.x < output.min.x)
					output.min = new Vector3(bounds.min.x, output.min.y, output.min.z);
				if (bounds.min.y < output.min.y)
					output.min = new Vector3(output.min.x, bounds.min.y, output.min.z);
				if (bounds.min.z < output.min.z)
					output.min = new Vector3(output.min.x, output.min.y, bounds.min.z);
				if (bounds.max.x > output.max.x)
					output.max = new Vector3(bounds.max.x, output.max.y, output.max.z);
				if (bounds.max.y > output.max.y)
					output.max = new Vector3(output.max.x, bounds.max.y, output.max.z);
				if (bounds.max.z > output.max.z)
					output.max = new Vector3(output.max.x, output.max.y, bounds.max.z);
			}
			return output;
		}
		
		public static bool Intersects (this Bounds b1, Bounds b2, Vector3 expandB1 = new Vector3(), Vector3 expandB2 = new Vector3())
		{
			b1.Expand(expandB1);
			b2.Expand(expandB2);
			return b1.Intersects(b2);
		}

		public static Bounds SetToPositiveSize (this Bounds bounds)
		{
			Bounds output = bounds;
			output.size = new Vector3(Mathf.Abs(output.size.x), Mathf.Abs(output.size.y), Mathf.Abs(output.size.z));
			output.center = bounds.center;
			return output;
		}

		public static Bounds AnchorToPoint (this Bounds bounds, Vector3 point, Vector3 anchorPoint)
		{
			Bounds output = bounds;
			output.min = point - (output.size.Multiply(anchorPoint));
			return output;
		}
		
		public static Bounds FromPoints (params Vector3[] points)
		{
			Bounds output = new Bounds(points[0], Vector3.zero);
			for (int i = 1; i < points.Length; i ++)
			{
				Vector3 point = points[i];
				if (point.x < output.min.x)
					output.min = new Vector3(point.x, output.min.y, output.min.z);
				if (point.y < output.min.y)
					output.min = new Vector3(output.min.x, point.y, output.min.z);
				if (point.z < output.min.z)
					output.min = new Vector3(output.min.x, output.min.y, point.z);
				if (point.x > output.max.x)
					output.max = new Vector3(point.x, output.max.y, output.max.z);
				if (point.y > output.max.y)
					output.max = new Vector3(output.max.x, point.y, output.max.z);
				if (point.z > output.max.z)
					output.max = new Vector3(output.max.x, output.max.y, point.z);
			}
			return output;
		}
	}
}