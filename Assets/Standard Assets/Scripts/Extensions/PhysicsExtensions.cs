using UnityEngine;

namespace Extensions
{
	public static class PhysicsExtensions
	{
		public static float GetDamage (this Collision coll, Rigidbody rigid)
		{
			// return coll.impulse / Time.deltaTime;
			float output = 0;
			Vector3 momentum = Vector3.zero;
			Vector3 momentum2 = Vector3.zero;
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				momentum += coll.rigidbody.GetPointVelocity(contactPoint.point) * coll.rigidbody.mass;
				momentum2 += rigid.GetPointVelocity(contactPoint.point) * rigid.mass;
			}
			output = -((Vector3.Dot(momentum.normalized, momentum2.normalized) - 1) / 2);
			float momentumMagnitude = momentum.magnitude;
			float momentum2Magnitude = momentum2.magnitude;
			if (Mathf.Max(momentumMagnitude, momentum2Magnitude) == momentumMagnitude)
				output *= momentumMagnitude - momentum2Magnitude;
			else
				output *= momentum2Magnitude - momentumMagnitude;
			return Mathf.Clamp(output, 0, Mathf.Infinity);
		}
	}
}