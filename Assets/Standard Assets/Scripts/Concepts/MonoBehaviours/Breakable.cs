using UnityEngine;
using Extensions;
using System;
using Random = UnityEngine.Random;

public class Breakable : MonoBehaviour
{
	public float minBreakForceSqr;
	public float maxBreakAngleChange;
	public BrokenVariation[] brokenVariations = new BrokenVariation[0];

	void OnCollisionEnter (Collision coll)
	{
		Vector3 force = coll.GetForce();
		if (force.sqrMagnitude >= minBreakForceSqr)
			Break (force);
	}

	public virtual void Break (Vector3 force)
	{
		if (brokenVariations.Length > 0)
		{
			BrokenVariation brokenVariation = brokenVariations[Random.Range(0, brokenVariations.Length)];
			brokenVariation.trs.SetParent(null);
			brokenVariation.trs.gameObject.SetActive(true);
			for (int i = 0; i < brokenVariation.rigidbodies.Length; i ++)
			{
				Rigidbody rigid = brokenVariation.rigidbodies[i];
				rigid.velocity = force.RandomRotate(0, maxBreakAngleChange);
			}
		}
		Destroy(gameObject);
	}

	[Serializable]
	public struct BrokenVariation
	{
		public Transform trs;
		public Rigidbody[] rigidbodies;
	}
}