using UnityEngine;
using System;
using Extensions;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using VisionGame;
#endif

[ExecuteInEditMode]
public class CombineCubes : MonoBehaviour
{
	public CombineEntry[] combineEntries = new CombineEntry[0];

	public void OnEnable ()
	{
		if (combineEntries.Length == 0)
		   return;
		KeyValuePair<Mesh, Transform>[] meshesAndTransforms = new KeyValuePair<Mesh, Transform>[combineEntries.Length];
		CombineInstance[] combineInstances = new CombineInstance[combineEntries.Length];
		for (int i = 0; i < combineEntries.Length; i ++)
		{
			CombineEntry combineEntry = combineEntries[i];
			meshesAndTransforms[i] = new KeyValuePair<Mesh, Transform>(combineEntry.meshFilter.mesh, combineEntry.trs);
			CombineInstance combineInstance = combineInstances[i];
			combineInstance.mesh = combineEntry.meshFilter.mesh;
			combineInstance.transform = combineEntry.trs.localToWorldMatrix;
			combineEntry.trs.gameObject.SetActive(false);
			combineInstances[i] = combineInstance;
		}
		for (int i = 0; i < meshesAndTransforms.Length; i ++)
		{
			KeyValuePair<Mesh, Transform> meshAndTransform = meshesAndTransforms[i];
			for (int i2 = 0; i2 < meshesAndTransforms.Length; i2 ++)
			{
				KeyValuePair<Mesh, Transform> otherMeshAndTransform = meshesAndTransforms[i2];
				Vector3 toOtherTrs = otherMeshAndTransform.Value.position - meshAndTransform.Value.position;
				if (toOtherTrs.sqrMagnitude == 1)
				{
					MeshExtensions.MeshTriangle[] meshTriangles = MeshExtensions.GetTriangles(new KeyValuePair<Mesh, Transform>[1] { meshAndTransform });
					MeshExtensions.MeshTriangle[] otherMeshTriangles = MeshExtensions.GetTriangles(new KeyValuePair<Mesh, Transform>[1] { otherMeshAndTransform });
					int toOtherTrsXSign = MathfExtensions.Sign(toOtherTrs.x);
					int toOtherTrsYSign = MathfExtensions.Sign(toOtherTrs.y);
					int toOtherTrsZSign = MathfExtensions.Sign(toOtherTrs.z);
					if (toOtherTrsXSign != 0)
					{
						for (int i3 = 0; i3 < meshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = meshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.x - meshAndTransform.Value.position.x) == toOtherTrsXSign && Mathf.Sign(meshTriangle.point2.x - meshAndTransform.Value.position.x) == toOtherTrsXSign && Mathf.Sign(meshTriangle.point3.x - meshAndTransform.Value.position.x) == toOtherTrsXSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < meshTriangles.Length; i4 ++)
								{
									if (meshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										meshTriangles[i4].triangleIndex --;
								}
							}
						}
						for (int i3 = 0; i3 < otherMeshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = otherMeshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.x - otherMeshAndTransform.Value.position.x) == -toOtherTrsXSign && Mathf.Sign(meshTriangle.point2.x - otherMeshAndTransform.Value.position.x) == -toOtherTrsXSign && Mathf.Sign(meshTriangle.point3.x - otherMeshAndTransform.Value.position.x) == -toOtherTrsXSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < otherMeshTriangles.Length; i4 ++)
								{
									if (otherMeshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										otherMeshTriangles[i4].triangleIndex --;
								}
							}
						}
					}
					else if (toOtherTrsYSign != 0)
					{
						for (int i3 = 0; i3 < meshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = meshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.y - meshAndTransform.Value.position.y) == toOtherTrsYSign && Mathf.Sign(meshTriangle.point2.y - meshAndTransform.Value.position.y) == toOtherTrsYSign && Mathf.Sign(meshTriangle.point3.y - meshAndTransform.Value.position.y) == toOtherTrsYSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < meshTriangles.Length; i4 ++)
								{
									if (meshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										meshTriangles[i4].triangleIndex --;
								}
							}
						}
						for (int i3 = 0; i3 < otherMeshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = otherMeshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.y - otherMeshAndTransform.Value.position.y) == -toOtherTrsYSign && Mathf.Sign(meshTriangle.point2.y - otherMeshAndTransform.Value.position.y) == -toOtherTrsYSign && Mathf.Sign(meshTriangle.point3.y - otherMeshAndTransform.Value.position.y) == -toOtherTrsYSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < otherMeshTriangles.Length; i4 ++)
								{
									if (otherMeshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										otherMeshTriangles[i4].triangleIndex --;
								}
							}
						}
					}
					else
					{
						for (int i3 = 0; i3 < meshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = meshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.z - meshAndTransform.Value.position.z) == toOtherTrsZSign && Mathf.Sign(meshTriangle.point2.z - meshAndTransform.Value.position.z) == toOtherTrsZSign && Mathf.Sign(meshTriangle.point3.z - meshAndTransform.Value.position.z) == toOtherTrsZSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < meshTriangles.Length; i4 ++)
								{
									if (meshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										meshTriangles[i4].triangleIndex --;
								}
							}
						}
						for (int i3 = 0; i3 < otherMeshTriangles.Length; i3 ++)
						{
							MeshExtensions.MeshTriangle meshTriangle = otherMeshTriangles[i3];
							if (Mathf.Sign(meshTriangle.point1.z - otherMeshAndTransform.Value.position.z) == -toOtherTrsZSign && Mathf.Sign(meshTriangle.point2.z - otherMeshAndTransform.Value.position.z) == -toOtherTrsZSign && Mathf.Sign(meshTriangle.point3.z - otherMeshAndTransform.Value.position.z) == -toOtherTrsZSign)
							{
								meshTriangle.RemoveFromMesh ();
								for (int i4 = 0; i4 < otherMeshTriangles.Length; i4 ++)
								{
									if (otherMeshTriangles[i4].triangleIndex > meshTriangle.triangleIndex)
										otherMeshTriangles[i4].triangleIndex --;
								}
							}
						}
					}
				}
			}
		}
		GameObject go = new GameObject();
		MeshFilter meshFilter = go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(combineInstances);
		// for (int i = 0; i < combineEntries.Length; i ++)
		// 	DestroyImmediate(combineEntries[i].trs.gameObject);
		combineEntries = new CombineEntry[0];
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Combine Cubes")]
	public static void _CombineCubes ()
	{
		CombineEntry[] combineEntries = new CombineEntry[Selection.transforms.Length];
        for (int i = 0; i < Selection.transforms.Length; i ++)
		{
            Transform trs = Selection.transforms[i];
            combineEntries[i] = new CombineEntry(trs.GetComponent<MeshFilter>(), trs);
		}
		CombineCubes combineCubes = new GameObject().AddComponent<CombineCubes>();
		combineCubes.combineEntries = combineEntries;
		combineCubes.OnEnable ();
		DestroyImmediate(combineCubes.gameObject);
	}
#endif

	[Serializable]
	public struct CombineEntry
	{
		public MeshFilter meshFilter;
		public Transform trs;

		public CombineEntry (MeshFilter meshFilter, Transform trs)
		{
			this.meshFilter = meshFilter;
			this.trs = trs;
		}
	}
}