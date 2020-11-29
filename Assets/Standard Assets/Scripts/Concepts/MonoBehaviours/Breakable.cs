using UnityEngine;
using Extensions;

public class Breakable : MonoBehaviour
{
	public float minBreakForceSqr;
	public Transform brokenVariationsParent;

	void OnCollisionEnter (Collision coll)
	{
		print(coll.GetForce() + " " + coll.GetForceSqr());
		if (coll.GetForceSqr() >= minBreakForceSqr)
			Break ();
	}

	public virtual void Break ()
	{
		Transform brokenVariation = brokenVariationsParent.GetChild(Random.Range(0, brokenVariationsParent.childCount));
		brokenVariation.SetParent(null);
		brokenVariation.gameObject.SetActive(true);
		Destroy(gameObject);
	}
}