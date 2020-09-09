using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace VisionGame
{
	public class Generator : MonoBehaviour
	{
		public int id;
		public Transform trs;
		public MonoBehaviour[] triggerables = new MonoBehaviour[0];
		public LayerMask whatBlocksTransmission;
		List<WirelessTransmitter> currentlyReceivingTransmitters = new List<WirelessTransmitter>();

		void OnTriggerEnter (Collider other)
		{
			WirelessTransmitter wirelessTransmitter = other.GetComponent<WirelessTransmitter>();
			if (wirelessTransmitter != null && wirelessTransmitter.receiverIds.Contains(id))
			{
				RaycastHit hit;
				Vector3 rayStart = wirelessTransmitter.collider.ClosestPoint(trs.position);
				Vector3 rayEnd = trs.position;
				rayStart += (rayEnd - rayStart).normalized * Physics.defaultContactOffset;
				Vector3 rayVector = rayEnd - rayStart;
				if (Physics.Raycast(rayStart, rayVector, out hit, rayVector.magnitude, whatBlocksTransmission) && hit.collider.gameObject == gameObject)
				{
					currentlyReceivingTransmitters.Add(wirelessTransmitter);
					foreach (MonoBehaviour triggerable in triggerables)
						triggerable.enabled = true;
				}
			}
		}

		void OnTriggerStay (Collider other)
		{
			WirelessTransmitter wirelessTransmitter = other.GetComponent<WirelessTransmitter>();
			if (wirelessTransmitter != null && wirelessTransmitter.receiverIds.Contains(id))
			{
				RaycastHit hit;
				Vector3 rayStart = wirelessTransmitter.collider.ClosestPoint(trs.position);
				Vector3 rayEnd = trs.position;
				rayStart += (rayEnd - rayStart).normalized * Physics.defaultContactOffset;
				Vector3 rayVector = rayEnd - rayStart;
				if (!currentlyReceivingTransmitters.Contains(wirelessTransmitter))
				{
					if (Physics.Raycast(rayStart, rayVector, out hit, rayVector.magnitude, whatBlocksTransmission) && hit.collider.gameObject == gameObject)
					{
						currentlyReceivingTransmitters.Add(wirelessTransmitter);
						foreach (MonoBehaviour triggerable in triggerables)
							triggerable.enabled = true;
					}
				}
				else
				{
					if (Physics.Raycast(rayStart, rayVector, out hit, rayVector.magnitude, whatBlocksTransmission) && hit.collider.gameObject != gameObject)
					{
						currentlyReceivingTransmitters.Remove(wirelessTransmitter);
						if (currentlyReceivingTransmitters.Count == 0)
						{
							foreach (MonoBehaviour triggerable in triggerables)
								triggerable.enabled = false;
						}
					}
				}
			}
		}

		void OnTriggerExit (Collider other)
		{
			WirelessTransmitter wirelessTransmitter = other.GetComponent<WirelessTransmitter>();
			if (wirelessTransmitter != null && wirelessTransmitter.receiverIds.Contains(id))
			{
				currentlyReceivingTransmitters.Remove(wirelessTransmitter);
				if (currentlyReceivingTransmitters.Count == 0)
				{
					foreach (MonoBehaviour triggerable in triggerables)
						triggerable.enabled = false;
				}
			}
		}
	}
}