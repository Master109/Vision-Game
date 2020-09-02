using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisionGame;

namespace Extensions
{
	public static class ColliderExtensions
	{
		public static Rect GetUnrotatedRect (this Collider2D collider)
		{
#if UNITY_EDITOR
			return collider.GetUnrotatedRect(collider.GetComponent<Transform>());
#else
			return collider.bounds;
#endif
		}

		public static Rect GetUnrotatedRect (this Collider2D collider, Transform trs)
		{
#if UNITY_EDITOR
			Rect output = new Rect();
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				output = Rect.MinMaxRect(trs.position.x + (boxCollider.offset.x * trs.lossyScale.x) - (boxCollider.size.x / 2 + boxCollider.edgeRadius) * trs.lossyScale.x, trs.position.y + (boxCollider.offset.y * trs.lossyScale.y) - (boxCollider.size.y / 2 + boxCollider.edgeRadius) * trs.lossyScale.y, trs.position.x + (boxCollider.offset.x * trs.lossyScale.x) + (boxCollider.size.x / 2 + boxCollider.edgeRadius) * trs.lossyScale.x, trs.position.y + (boxCollider.offset.y * trs.lossyScale.y) + (boxCollider.size.y / 2 + boxCollider.edgeRadius) * trs.lossyScale.y);
			else
			{
				PolygonCollider2D polygonCollider = collider as PolygonCollider2D;
				if (polygonCollider != null)
				{
					Rect localRect = new Rect();
					foreach (Vector2 point in polygonCollider.points)
					{
						if (point.x < localRect.xMin)
							localRect.xMin = point.x;
						if (point.x > localRect.xMax)
							localRect.xMax = point.x;
						if (point.y < localRect.yMin)
							localRect.yMin = point.y;
						if (point.y > localRect.yMax)
							localRect.yMax = point.y;
					}
					output = Rect.MinMaxRect(trs.position.x + polygonCollider.offset.x * trs.lossyScale.x - localRect.size.x / 2 * trs.lossyScale.x, trs.position.y + polygonCollider.offset.y * trs.lossyScale.y - localRect.size.y / 2 * trs.lossyScale.y, trs.position.x + polygonCollider.offset.x * trs.lossyScale.x + localRect.size.x / 2 * trs.lossyScale.x, trs.position.y + polygonCollider.offset.y * trs.lossyScale.y + localRect.size.y / 2 * trs.lossyScale.y);
				}
				else
					output = collider.bounds.ToRect();
			}
			return output.SetToPositiveSize();
#else
			return collider.bounds.ToRect().SetToPositiveSize();
#endif
		}

		public static Bounds GetUnrotatedBounds (this Collider collider)
		{
#if UNITY_EDITOR
			return collider.GetUnrotatedBounds(collider.GetComponent<Transform>());
#else
			return collider.bounds;
#endif
		}

		public static Bounds GetUnrotatedBounds (this Collider collider, Transform trs)
		{
#if UNITY_EDITOR
			Bounds output = new Bounds();
			BoxCollider boxCollider = collider as BoxCollider;
			if (boxCollider != null)
				output = new Bounds();
			else
				output = collider.bounds;
			return output;
#else
			return collider.bounds;
#endif
		}

		public static Vector2 GetUnrotatedCenter (this Collider2D collider)
		{
#if UNITY_EDITOR
			return collider.GetUnrotatedCenter(collider.GetComponent<Transform>());
#else
			return collider.bounds.center;
#endif
		}

		public static Vector2 GetUnrotatedCenter (this Collider2D collider, Transform trs)
		{
#if UNITY_EDITOR
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				return new Vector2(trs.position.x + boxCollider.offset.x * trs.lossyScale.x, trs.position.y + boxCollider.offset.y * trs.lossyScale.y);
			else
				return collider.GetUnrotatedRect(trs).center;
#else
			return collider.bounds.center;
#endif
		}

		public static Vector3 GetUnrotatedCenter (this Collider collider)
		{
#if UNITY_EDITOR
			return collider.GetUnrotatedCenter(collider.GetComponent<Transform>());
#else
			return collider.bounds.center;
#endif
		}

		public static Vector3 GetUnrotatedCenter (this Collider collider, Transform trs)
		{
			BoxCollider boxCollider = collider as BoxCollider;
			if (boxCollider != null)
				return new Vector3(trs.position.x + boxCollider.center.x * trs.lossyScale.x, trs.position.y + boxCollider.center.y * trs.lossyScale.y, trs.position.z + boxCollider.center.z * trs.lossyScale.z);
			else
				return collider.GetUnrotatedBounds(trs).center;
		}

		public static Vector2 GetUnrotatedSize (this Collider2D collider)
		{
			return collider.GetUnrotatedSize(collider.GetComponent<Transform>());
		}

		public static Vector2 GetUnrotatedSize (this Collider2D collider, Transform trs)
		{
			BoxCollider2D boxCollider = collider as BoxCollider2D;
			if (boxCollider != null)
				return new Vector2((boxCollider.size.x + boxCollider.edgeRadius * 2) * trs.lossyScale.x, (boxCollider.size.y + boxCollider.edgeRadius * 2) * trs.lossyScale.y);
			else
				return collider.GetUnrotatedRect(trs).size;
		}

		public static Vector3 GetUnrotatedSize (this Collider collider)
		{
			return collider.GetUnrotatedSize(collider.GetComponent<Transform>());
		}

		public static Vector3 GetUnrotatedSize (this Collider collider, Transform trs)
		{
			BoxCollider boxCollider = collider as BoxCollider;
			if (boxCollider != null)
				return new Vector2(boxCollider.size.x * trs.lossyScale.x, boxCollider.size.y * trs.lossyScale.y);
			else
				return collider.GetUnrotatedBounds(trs).size;
		}
	}
}