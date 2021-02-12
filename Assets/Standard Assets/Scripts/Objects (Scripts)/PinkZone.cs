using UnityEngine;

namespace VisionGame
{
	public class PinkZone : MonoBehaviour
	{
		void OnTriggerStay (Collider other)
		{
			Orb orb = other.GetComponent<Orb>();
			if (orb != null)
			{
				for (int i = 0; i < orb.capturedObjectsParent.childCount; i ++)
					Destroy(orb.capturedObjectsParent.GetChild(i).gameObject);
				Destroy(orb.capturedObjectVisualizersParent.gameObject);
			}
		}
	}
}