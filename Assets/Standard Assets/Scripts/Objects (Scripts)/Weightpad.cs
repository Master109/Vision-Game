using UnityEngine;

namespace VisionGame
{
	public class Weightpad : MonoBehaviour
	{
		public Transform trs;
		public Generator[] generators = new Generator[0];
		public float minImpactMomentumToPressSqr;

		void OnCollisionEnter (Collision coll)
		{
			OnCollisionStay (coll);
		}

		void OnCollisionStay (Collision coll)
		{
			Vector3 momentum = coll.rigidbody.velocity * coll.rigidbody.mass;
			Vector3 impactMomentum = Vector3.Project(-trs.up * momentum.magnitude, momentum);
			bool enoughImpactMomentumToPress = impactMomentum.sqrMagnitude > minImpactMomentumToPressSqr;
			foreach (Generator generator in generators)
			{
				foreach (MonoBehaviour triggerable in generator.triggerables)
					triggerable.enabled = enoughImpactMomentumToPress;
			}
		}

		void OnCollisionExit (Collision coll)
		{
			foreach (Generator generator in generators)
			{
				foreach (MonoBehaviour triggerable in generator.triggerables)
					triggerable.enabled = false;
			}
		}
	}
}