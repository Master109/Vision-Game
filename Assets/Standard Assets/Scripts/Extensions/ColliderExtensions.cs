using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class ColliderExtensions
	{
		public static Bounds GetUnrotatedBounds (this Collider collider)
		{
			return collider.GetUnrotatedBounds(collider.GetComponent<Transform>());
		}

		public static Bounds GetUnrotatedBounds (this Collider collider, Transform trs)
		{
			Bounds output = new Bounds();
			BoxCollider boxCollider = collider as BoxCollider;
			if (boxCollider != null)
				output = new Bounds(boxCollider.center, trs.lossyScale.Multiply(Quaternion.Inverse(trs.rotation) * boxCollider.size));
			else
			{
				SphereCollider sphereCollider = collider as SphereCollider;
				if (sphereCollider != null)
					output = new Bounds(sphereCollider.center, sphereCollider.bounds.size);
			}
			return output.SetToPositiveSize();
		}
	}
}