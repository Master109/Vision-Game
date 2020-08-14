using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CombineMeshes : MonoBehaviour
{
	public CombineEntry[] combineEntries = new CombineEntry[0];

	public void OnEnable ()
	{
		if (combineEntries.Length == 0)
			return;
		CombineInstance[] combineInstances = new CombineInstance[combineEntries.Length];
		for (int i = 0; i < combineEntries.Length; i ++)
		{
			CombineEntry combineEntry = combineEntries[i];
			CombineInstance combineInstance = combineInstances[i];
			combineInstance.mesh = combineEntry.meshFilter.mesh;
			combineInstance.transform = combineEntry.trs.localToWorldMatrix;
			combineInstances[i] = combineInstance;
		}
		GameObject go = new GameObject();
		MeshFilter meshFilter = go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(combineInstances);
		combineEntries = new CombineEntry[0];
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Combine Meshes")]
	public static void _CombineMeshes ()
	{
		CombineEntry[] combineEntries = new CombineEntry[Selection.transforms.Length];
        for (int i = 0; i < Selection.transforms.Length; i ++)
		{
            Transform trs = Selection.transforms[i];
            combineEntries[i] = new CombineEntry(trs.GetComponent<MeshFilter>(), trs);
		}
		CombineMeshes combineMeshes = new GameObject().AddComponent<CombineMeshes>();
		combineMeshes.combineEntries = combineEntries;
		combineMeshes.OnEnable ();
		DestroyImmediate(combineMeshes);
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
	
	public enum CombineMode
	{
		Default,
		DestroySharedVertices
	}
}