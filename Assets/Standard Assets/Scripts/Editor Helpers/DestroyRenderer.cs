using UnityEngine;

namespace VisionGame
{
	public class DestroyRenderer : MonoBehaviour
	{
		public virtual void Start ()
		{
			if (Application.isPlaying)
			{
				Destroy(GetComponent<Renderer>());
				Destroy(this);
			}
		}
	}
}