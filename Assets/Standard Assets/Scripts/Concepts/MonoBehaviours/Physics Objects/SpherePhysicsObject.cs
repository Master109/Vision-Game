using UnityEngine;

namespace VisionGame
{
	public class SpherePhysicsObject : PhysicsObject
	{
		public LayerMask whatICollideWith;
		public float collisionCheckDistance;

		public override void OnCollisionEnter (Collision coll)
		{
			OnCollisionStay (coll);
			base.OnCollisionEnter (coll);
		}

		void OnCollisionStay (Collision coll)
		{
			Vector3 velocity = -coll.relativeVelocity;
			if (Physics.Raycast(trs.position + (velocity.normalized * (collider.bounds.extents.x + Physics.defaultContactOffset)), velocity, collisionCheckDistance, whatICollideWith))
				rigid.freezeRotation = true;
		}

		void OnCollisionExit (Collision coll)
		{
			rigid.freezeRotation = false;
		}
	}
}