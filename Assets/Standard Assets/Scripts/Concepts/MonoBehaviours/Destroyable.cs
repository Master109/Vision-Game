using UnityEngine;

namespace VisionGame
{
	public class Destroyable : MonoBehaviour, IDestroyable
	{
		public GameObject Go
		{
			get
			{
				return go;
			}
		}
		public GameObject go;
	}
}