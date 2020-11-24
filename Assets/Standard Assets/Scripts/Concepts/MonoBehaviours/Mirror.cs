using UnityEngine;

namespace VisionGame
{
	// [ExecuteInEditMode]
	public class Mirror : MonoBehaviour
	{
        public Renderer renderer;
		
		void OnEnable ()
		{
			renderer.material.SetFloat("_Metallic", 1);
			renderer.material.SetFloat("_Smoothness", 1);
		}
	}
}