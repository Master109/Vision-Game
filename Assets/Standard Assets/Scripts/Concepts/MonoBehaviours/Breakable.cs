using UnityEngine;
using Extensions;
using System;
using Random = UnityEngine.Random;

public class Breakable : MonoBehaviour
{
	// public Rigidbody rigid;
	public float minBreakForce;
	public float maxBreakAngleChange;
	public BrokenVariation[] brokenVariations = new BrokenVariation[0];

	// void OnCollisionEnter (Collision coll)
	// {
	// 	float damage = coll.GetDamage(rigid);
	// 	if (damage >= minBreakForce)
	// 		Break (coll.relativeVelocity.normalized * damage);
	// }

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