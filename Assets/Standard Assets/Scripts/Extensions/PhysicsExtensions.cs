using UnityEngine;

namespace Extensions
{
	public static class PhysicsExtensions
	{
		public static Vector3 GetForce (this Collision coll)
		{
			return coll.impulse / Time.fixedDeltaTime;
		}
	}
}