﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisionGame;
using UnityEngine.InputSystem.Controls;

namespace Extensions
{
	public static class VectorExtensions
	{
		public static Vector3 NULL2 = new Vector2(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);
		public static Vector3 NULL3 = new Vector3(MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT, MathfExtensions.NULL_FLOAT);
		public static Vector3 INFINITE = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
		
		public static Vector3 Snap (this Vector3 v, Vector3 snap)
		{
			return new Vector3(MathfExtensions.SnapToInterval(v.x, snap.x), MathfExtensions.SnapToInterval(v.y, snap.y), MathfExtensions.SnapToInterval(v.z, snap.z));
		}

		public static Vector2 Snap (this Vector2 v, Vector2 snap)
		{
			return new Vector2(MathfExtensions.SnapToInterval(v.x, snap.x), MathfExtensions.SnapToInterval(v.y, snap.y));
		}
		
		public static Vector3 Multiply (this Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
		}

		public static Vector2 Multiply (this Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x * v2.x, v1.y * v2.y);
		}

		public static Vector2Int Multiply (this Vector2Int v1, Vector2 v2)
		{
			return v1.ToVec2().Multiply(v2).ToVec2Int();
		}

		public static float Cross (this Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.y - v1.y * v2.x;
		}

		public static float Multiply_float (this Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.x + v1.y * v2.y;
		}
		
		public static Vector3 Divide (this Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
		}

		public static Vector2 Divide (this Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x / v2.x, v1.y / v2.y);
		}
		
		public static Vector2 Rotate (this Vector2 v, float degrees)
		{
			float ang = GetFacingAngle(v) + degrees;
			ang *= Mathf.Deg2Rad;
			// ang = MathfExtensions.RegularizeAngle(ang);
			return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * v.magnitude;
		}
		
		public static Vector2 Rotate (this Vector2 v, Vector2 pivotPoint, float degrees)
		{
			float ang = GetFacingAngle(v - pivotPoint) + degrees;
			ang *= Mathf.Deg2Rad;
			return pivotPoint + (new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * Vector2.Distance(v, pivotPoint));
		}
		
		public static Vector2 Rotate (this Vector3 v, float degrees)
		{
			float ang = GetFacingAngle(v) + degrees;
			ang *= Mathf.Deg2Rad;
			// ang = MathfExtensions.RegularizeAngle(ang);
			return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * v.magnitude;
		}

		public static Vector2 Rotate (this Vector3 v, Vector3 pivotPoint, float degrees)
		{
			float ang = GetFacingAngle(v - pivotPoint) + degrees;
			ang *= Mathf.Deg2Rad;
			return (Vector2) pivotPoint + (new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * Vector2.Distance(v, pivotPoint));
		}
		
		public static float GetFacingAngle (this Vector2 v)
		{
			v = v.normalized;
			return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		}
		
		public static float GetFacingAngle (this Vector3 v)
		{
			v = v.normalized;
			return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		}
		
		public static Vector2 RotateTo (this Vector2 from, Vector2 to, float maxDegrees)
		{
			float ang = from.GetFacingAngle();
			ang += Mathf.Clamp(Vector2.SignedAngle(from, to), -maxDegrees, maxDegrees);
			ang *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * from.magnitude;
		}
		
		public static Vector2 RotateTo (this Vector3 from, Vector3 to, float maxDegrees)
		{
			float ang = from.GetFacingAngle();
			ang += Mathf.Clamp(Vector2.SignedAngle(from, to), -maxDegrees, maxDegrees);
			ang *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)).normalized * from.magnitude;
		}
		
		public static Vector3 ClampComponents (this Vector3 v, Vector3 min, Vector3 max)
		{
			return new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));
		}
		
		public static Vector2 ClampComponents (this Vector2 v, Vector2 min, Vector2 max)
		{
			return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
		}
		
		public static Vector3Int ToVec3Int (this Vector3 v, MathfExtensions.RoundingMethod roundMethod = MathfExtensions.RoundingMethod.HalfOrLessRoundsDown)
		{
			switch (roundMethod)
			{
				case MathfExtensions.RoundingMethod.HalfOrLessRoundsDown:
					return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
				default:
					throw new UnityException("The logic for handling the " + roundMethod.ToString() + " round method has not yet been implemented.");
			}
		}
		
		public static Vector3Int ToVec3Int (this Vector2 v, MathfExtensions.RoundingMethod roundMethod = MathfExtensions.RoundingMethod.HalfOrLessRoundsDown)
		{
			switch (roundMethod)
			{
				case MathfExtensions.RoundingMethod.HalfOrLessRoundsDown:
					return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), 0);
				default:
					throw new UnityException("The logic for handling the " + roundMethod.ToString() + " round method has not yet been implemented.");
			}
		}

		public static Vector2Int ToVec2Int (this Vector2 v, MathfExtensions.RoundingMethod roundMethod = MathfExtensions.RoundingMethod.HalfOrLessRoundsDown)
		{
			switch (roundMethod)
			{
				case MathfExtensions.RoundingMethod.HalfOrLessRoundsDown:
					return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
				default:
					throw new UnityException("The logic for handling the " + roundMethod.ToString() + " round method has not yet been implemented.");
			}
		}

		public static Vector2Int ToVec2Int (this Vector3 v, MathfExtensions.RoundingMethod roundMethod = MathfExtensions.RoundingMethod.HalfOrLessRoundsDown)
		{
			switch (roundMethod)
			{
				case MathfExtensions.RoundingMethod.HalfOrLessRoundsDown:
					return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
				default:
					throw new UnityException("The logic for handling the " + roundMethod.ToString() + " round method has not yet been implemented.");
			}
		}

		public static Vector3 ToVec3 (this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector3Int ToVec3Int (this Vector4 v)
		{
			return new Vector3Int((int) v.x, (int) v.y, (int) v.z);
		}

		public static Vector2 ToVec2 (this Vector2Int v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 ToVec2 (this Vector3Int v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector3 ToVec3 (this Vector2Int v)
		{
			return new Vector3(v.x, v.y);
		}

		public static Vector2Int ToVec2Int (this Vector3Int v)
		{
			return new Vector2Int(v.x, v.y);
		}

		public static Vector3Int ToVec3Int (this Vector2Int v)
		{
			return new Vector3Int(v.x, v.y, 0);
		}
		
		public static Vector3 SetX (this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}
		
		public static Vector3 SetY (this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}
		
		public static Vector3 SetZ (this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}
		
		public static Vector2 SetX (this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}
		
		public static Vector2 SetY (this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

        public static Vector3 SetZ (this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3Int SetZ (this Vector3Int v, int z)
        {
            return new Vector3Int(v.x, v.y, z);
        }

        public static Vector2 SetToMinComponents (this Vector2 v, Vector2 v2)
        {
	        return new Vector2(Mathf.Min(v.x, v2.x), Mathf.Min(v.y, v2.y));
        }

        public static Vector2 SetToMaxComponents (this Vector2 v, Vector2 v2)
        {
	        return new Vector2(Mathf.Max(v.x, v2.x), Mathf.Max(v.y, v2.y));
        }

        public static Vector2Int SetToMinComponents (this Vector2Int v, Vector2Int v2)
        {
	        return new Vector2Int(Mathf.Min(v.x, v2.x), Mathf.Min(v.y, v2.y));
        }

        public static Vector2Int SetToMaxComponents (this Vector2Int v, Vector2Int v2)
        {
	        return new Vector2Int(Mathf.Max(v.x, v2.x), Mathf.Max(v.y, v2.y));
        }
		
		public static Vector2 FromFacingAngle (float angle)
		{
			angle *= Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
		}
		
		public static Vector2Int FromFacingAngle (float angle, float maxLength)
		{
			angle *= Mathf.Deg2Rad;
			Vector2 actualResult = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * maxLength;
			// Find what actualResult is closest to
			return actualResult.ToVec2Int();
		}

		public static float Sign (Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
		}

		public static bool IsInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
		{
			float d1, d2, d3;
			bool has_neg, has_pos;
			d1 = Sign(pt, v1, v2);
			d2 = Sign(pt, v2, v3);
			d3 = Sign(pt, v3, v1);
			has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
			has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);
			return !(has_neg && has_pos);
		}

		public static bool IsFacingAngleWithinAngleRange (Vector2Int v, AngleRange angleRange, bool equalAnglesCountsAsIn = false)
		{
			return new Angle(v.ToVec2().GetFacingAngle()).IsWithinAngleRange(angleRange, equalAnglesCountsAsIn);
		}

		public static Vector2Int Snap (Vector2Int v, Angle snap, bool searchClockwise = false)
		{
			Vector2Int output = (Vector2.right * v.magnitude).ToVec2Int();
			float minDegreesBetween = Mathf.Infinity;
			float degreesBetween;
			Angle currentAngle = new Angle();
			do
			{
				degreesBetween = Mathf.Abs(currentAngle.degrees - snap.degrees);
				if (minDegreesBetween > degreesBetween)
				{
					minDegreesBetween = degreesBetween;
					output = VectorExtensions.FromFacingAngle(currentAngle.degrees, v.magnitude);
				}
				currentAngle.degrees += snap.degrees;
			} while (currentAngle.degrees < 360);
			return output;
		}

		public static Vector2 ToVec2 (this Vector2Control control)
		{
			return new Vector2(control.x.ReadValue(), control.y.ReadValue());
		}

        public static Vector3 GetXZ (this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector2Int GetXZ (this Vector3Int v)
        {
            return new Vector2Int(v.x, v.z);
        }
		
		public static Vector3 XYToXZ (this Vector2 v)
		{
			return new Vector3(v.x, 0, v.y);
		}
		
		public static Vector3 XYToXZ (this Vector3 v)
		{
			return new Vector3(v.x, 0, v.y);
		}
		
		public static Vector2 XZToXY (this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2 GetClosestPoint (this Vector3 v, params Vector3[] points)
		{
			Vector3 currentPoint = points[0];
			Vector3 closestPoint = currentPoint;
			float distanceToCurrentPointSqr = (v - closestPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
				{
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
					closestPoint = currentPoint;
				}
			}
			return closestPoint;
		}

		public static int GetIndexOfClosestPoint (this Vector3 v, params Vector3[] points)
		{
			int output = 0;
			Vector3 currentPoint = points[0];
			float distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
				{
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
					output = i;
				}
			}
			return output;
		}

		public static float GetDistanceSqrToClosestPoint (this Vector3 v, params Vector3[] points)
		{
			Vector3 currentPoint = points[0];
			float distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
			}
			return distanceToClosestPointSqr;
		}

		public static Vector2 GetClosestPoint (this Vector2 v, params Vector2[] points)
		{
			Vector2 currentPoint = points[0];
			Vector2 closestPoint = currentPoint;
			float distanceToCurrentPointSqr = (v - closestPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
				{
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
					closestPoint = currentPoint;
				}
			}
			return closestPoint;
		}

		public static int GetIndexOfClosestPoint (this Vector2 v, params Vector2[] points)
		{
			int output = 0;
			Vector2 currentPoint = points[0];
			float distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
				{
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
					output = i;
				}
			}
			return output;
		}

		public static float GetDistanceSqrToClosestPoint (this Vector2 v, params Vector2[] points)
		{
			Vector2 currentPoint = points[0];
			float distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
			float distanceToClosestPointSqr = distanceToCurrentPointSqr;
			for (int i = 1; i < points.Length; i ++)
			{
				currentPoint = points[i];
				distanceToCurrentPointSqr = (v - currentPoint).sqrMagnitude;
				if (distanceToCurrentPointSqr < distanceToClosestPointSqr)
					distanceToClosestPointSqr = distanceToCurrentPointSqr;
			}
			return distanceToClosestPointSqr;
		}
    }
}