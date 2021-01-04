using UnityEngine;
using Extensions;
using UnityEngine.ProBuilder;
using ProBuilder2;
using System.Reflection;

namespace VisionGame
{
	public class MeshPhysicsObject : PhysicsObject
	{
// 		public MeshFilter meshFilter;
// 		public Mesh mesh;

// 		public override void OnEnable ()
// 		{
// #if UNITY_EDITOR
// 			if (!Application.isPlaying)
// 			{
// 				if (meshFilter != null)
// 					mesh = meshFilter.mesh;
// 				else
// 				{
// 					ProBuilderMesh proBuilderMesh = GetComponent<ProBuilderMesh>();
// 					Assembly assembly = proBuilderMesh.GetType().Assembly;
// 					foreach (Module module in assembly.Modules)
// 					{
// 						print(module.GetType());
// 						foreach (FieldInfo fieldInfo in module.GetFields())
// 							print(fieldInfo);
// 						foreach (MethodInfo methodInfo in module.GetMethods())
// 							print(methodInfo);
// 					}
// 					mesh = proBuilderMesh.GetMember<Mesh>("mesh");
// 				}
// 				MeshCollider meshCollider = collider as MeshCollider;
// 				if (meshCollider != null)
// 					meshCollider.sharedMesh = mesh;
// 			}
// #endif
// 			base.OnEnable ();
// 		}
	}
}