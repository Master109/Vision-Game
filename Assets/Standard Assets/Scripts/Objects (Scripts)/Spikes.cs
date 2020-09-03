using UnityEngine;

namespace VisionGame
{
	public class Spikes : MonoBehaviour
	{
		void OnCollisionEnter (Collision coll)
		{
			if (coll.gameObject == GameManager.GetSingleton<Player>().gameObject)
				Destroy(coll.gameObject);
		}
	}
}