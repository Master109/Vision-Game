using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class MeshPhysicsObject : PhysicsObject
	{
#if UNITY_EDITOR
		public MeshFilter capturedVisualizerMeshFilter;
		public bool updateMeshColliderMesh;
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				capturedVisualizerMeshFilter.sharedMesh = meshCollider.sharedMesh;
				DestroyImmediate(meshCollider);
				if (updateMeshColliderMesh)
				{
					meshCollider = collider as MeshCollider;
					if (meshCollider != null)
						meshCollider.sharedMesh = capturedVisualizerMeshFilter.sharedMesh;
				}
			}
#endif
			base.OnEnable ();
		}
	}
}