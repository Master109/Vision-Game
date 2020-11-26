#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using UnityEditor;
using System.IO;
using VisionGame;

[ExecuteInEditMode]
public class SubdivideMesh : EditorScript
{
	public MeshFilter meshFilter;
    public int subdivideCount;
	public bool makeAsset;
	public string saveAssetAtPath;
	public bool autoNameAssetPath;
	public bool update;

	public override void DoEditorUpdate ()
	{
		if (!update)
			return;
		update = false;
		Do ();
	}

	public void Do ()
	{
		Mesh mesh = Instantiate(meshFilter.sharedMesh);
        for (int i = 0; i < subdivideCount; i ++)
			mesh.Subdivide ();
		meshFilter.sharedMesh = mesh;
		if (makeAsset)
		{
			if (autoNameAssetPath)
			{
				string newAssetPath = saveAssetAtPath;
				while (File.Exists(newAssetPath))
					newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			}
			AssetDatabase.CreateAsset(meshFilter.sharedMesh, saveAssetAtPath);
			AssetDatabase.SaveAssets();
		}
	}

	[MenuItem("Tools/Subdivide Mesh")]
	static void _SubdivideMesh ()
	{
		SubdivideMesh _subdivideMesh = GameManager.GetSingleton<SubdivideMesh>();
		SubdivideMesh subdivideMesh = new GameObject().AddComponent<SubdivideMesh>();
		for (int i = 0; i < Selection.transforms.Length; i ++)
		{
			Transform trs = Selection.transforms[i];
			MeshFilter meshFilter = trs.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				subdivideMesh.meshFilter = meshFilter;
				break;
			}
		}
		if (subdivideMesh.meshFilter != null)
		{
			subdivideMesh.subdivideCount = 1;
			subdivideMesh.makeAsset = _subdivideMesh.makeAsset;
			subdivideMesh.saveAssetAtPath = _subdivideMesh.saveAssetAtPath;
			subdivideMesh.autoNameAssetPath = _subdivideMesh.autoNameAssetPath;
			subdivideMesh.Do ();
		}
		DestroyImmediate(subdivideMesh.gameObject);
	}
}
#endif