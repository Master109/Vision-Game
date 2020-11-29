using UnityEngine;

namespace Extensions
{
	public static class PhysicsExtensions
	{
		public static float GetForce (this Collision coll)
		{
			return (coll.impulse / Time.fixedDeltaTime).magnitude;
		}
		public static float GetForceSqr (this Collision coll)
		{
			return (coll.impulse / Time.fixedDeltaTime).sqrMagnitude;
		}
	}
}