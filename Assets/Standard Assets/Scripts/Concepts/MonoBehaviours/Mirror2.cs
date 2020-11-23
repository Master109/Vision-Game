using UnityEngine;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Mirror2 : MonoBehaviour
	{
        public Renderer renderer;
		
		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			renderer.material.SetFloat("_Metallic", 1);
			renderer.material.SetFloat("_Smoothness", 1);
		}
	}
}