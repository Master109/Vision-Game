using UnityEngine;
using Extensions;
using System.Collections.Generic;

namespace VisionGame
{
	public class Generator : MonoBehaviour
	{
		public int id;
		public MonoBehaviour[] triggerables = new MonoBehaviour[0];
		int currentlyReceivingTransmitterCount;

		void OnTriggerEnter (Collider other)
		{
			WirelessTransmitter wirelessTransmitter = other.GetComponent<WirelessTransmitter>();
			if (wirelessTransmitter.receiverIds.Contains(id))
			{
			   currentlyReceivingTransmitterCount ++;
			   foreach (MonoBehaviour triggerable in triggerables)
				   triggerable.enabled = true;
			}
		}

		void OnTriggerExit (Collider other)
		{
			WirelessTransmitter wirelessTransmitter = other.GetComponent<WirelessTransmitter>();
			if (wirelessTransmitter.receiverIds.Contains(id))
			{
			   currentlyReceivingTransmitterCount --;
			   if (currentlyReceivingTransmitterCount == 0)
			   {
					foreach (MonoBehaviour triggerable in triggerables)
						triggerable.enabled = false;
			   }
			}
		}
	}
}