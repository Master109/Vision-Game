using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using System.Collections.Generic;
using VisionGame;
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class CombineMeshes : MonoBehaviour
{
	public MeshFilter meshFilter;
	public CombineEntry[] combineEntries = new CombineEntry[0];

	public void OnEnable ()
	{
		CombineInstance[] combineInstances = new CombineInstance[combineEntries.Length];
		for (int i = 0; i < combineEntries.Length; i ++)
		{
			CombineEntry combineEntry = combineEntries[i];
			CombineInstance combineInstance = combineInstances[i];
			combineInstance.mesh = combineEntry.meshFilter.mesh;
			combineInstance.transform = combineEntry.trs.localToWorldMatrix;
			combineInstances[i] = combineInstance;
		}
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(combineInstances);
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Combine Meshes")]
	public static void _CombineMeshes ()
	{
		List<CombineEntry> combineEntries = new List<CombineEntry>();
		foreach (Transform trs in Selection.transforms)
			combineEntries.Add(new CombineEntry(trs.GetComponent<MeshFilter>(), trs));
		GameManager.GetSingleton<CombineMeshes>().combineEntries = combineEntries.ToArray();
		GameManager.GetSingleton<CombineMeshes>().OnEnable ();
	}
#endif

	[Serializable]
	public class CombineEntry
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