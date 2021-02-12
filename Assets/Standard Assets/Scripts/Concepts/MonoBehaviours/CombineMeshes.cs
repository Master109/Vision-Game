using UnityEngine;
using System;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
using VisionGame;
using System.IO;
#endif

[ExecuteInEditMode]
public class CombineMeshes : SingletonMonoBehaviour<CombineMeshes>
{
	public GameObject outputGo;
	public CombineEntry[] combineEntries = new CombineEntry[0];
	public bool autoSetMaterial;
	public bool autoSetColliders;
	public CombineMode combineMode;
#if UNITY_EDITOR
	public bool shouldMakeAsset;
	public string saveAssetAtPath;
	public bool autoRenameAssetPath;
#endif

	public void OnEnable ()
	{
		if (combineEntries.Length == 0)
			return;
		if (combineMode == CombineMode.Default)
		{
			CombineInstance[] combineInstances = new CombineInstance[combineEntries.Length];
			for (int i = 0; i < combineEntries.Length; i ++)
			{
				CombineEntry combineEntry = combineEntries[i];
				CombineInstance combineInstance = combineInstances[i];
				combineInstance.mesh = Instantiate(combineEntry.meshFilter.sharedMesh);
				combineInstance.transform = combineEntry.trs.localToWorldMatrix;
				combineInstances[i] = combineInstance;
			}
			if (outputGo == null)
				outputGo = new GameObject();
			MeshFilter meshFilter = outputGo.GetComponent<MeshFilter>();
			if (meshFilter == null)
				meshFilter = outputGo.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = outputGo.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
				meshRenderer = outputGo.AddComponent<MeshRenderer>();
			meshFilter.sharedMesh = new Mesh();
			meshFilter.sharedMesh.CombineMeshes(combineInstances);
			if (autoSetMaterial)
				meshRenderer.sharedMaterial = combineEntries[0].meshFilter.GetComponent<MeshRenderer>().sharedMaterial;
			if (autoSetColliders)
			{
				Collider[] colliders = outputGo.GetComponents<Collider>();
				for (int i2 = 0; i2 < colliders.Length; i2 ++)
				{
					Collider collider = colliders[i2];
					DestroyImmediate(collider);
				}
				for (int i = 0; i < combineEntries.Length; i ++)
				{
					CombineEntry combineEntry = combineEntries[i];
					BoxCollider boxCollider = outputGo.AddComponent<BoxCollider>();
					Transform outputTrs = outputGo.GetComponent<Transform>();
					boxCollider.center = outputTrs.InverseTransformPoint(combineEntry.trs.position);
					boxCollider.size = outputTrs.InverseTransformPoint(combineEntry.trs.lossyScale);
				}
			}
#if UNITY_EDITOR
		if (shouldMakeAsset)
		{
			if (autoRenameAssetPath)
			{
				string newAssetPath = saveAssetAtPath;
				while (File.Exists(newAssetPath))
					newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			}
			AssetDatabase.CreateAsset(meshFilter.sharedMesh, saveAssetAtPath);
			AssetDatabase.SaveAssets();
		}
#endif
		}
		combineEntries = new CombineEntry[0];
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Combine Meshes %#m")]
	public static void _CombineMeshes ()
	{
		CombineEntry[] combineEntries = new CombineEntry[Selection.transforms.Length];
        for (int i = 0; i < Selection.transforms.Length; i ++)
		{
            Transform trs = Selection.transforms[i];
            combineEntries[i] = new CombineEntry(trs.GetComponent<MeshFilter>(), trs);
		}
		GameObject outputGo = CombineMeshes.Instance.outputGo;
		CombineMode combineMode = CombineMeshes.Instance.combineMode;
		bool autoSetMaterial = CombineMeshes.Instance.autoSetMaterial;
		bool autoSetColliders = CombineMeshes.Instance.autoSetColliders;
		bool shouldMakeAsset = CombineMeshes.Instance.shouldMakeAsset;
		string saveAssetAtPath = CombineMeshes.Instance.saveAssetAtPath;
		CombineMeshes combineMeshes = new GameObject().AddComponent<CombineMeshes>();
		combineMeshes.outputGo = outputGo;
		combineMeshes.combineMode = combineMode;
		combineMeshes.autoSetMaterial = autoSetMaterial;
		combineMeshes.autoSetColliders = autoSetColliders;
		combineMeshes.shouldMakeAsset = shouldMakeAsset;
		combineMeshes.saveAssetAtPath = saveAssetAtPath;
		combineMeshes.combineEntries = combineEntries;
		combineMeshes.OnEnable ();
		DestroyImmediate(combineMeshes.gameObject);
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
		Default
	}
}