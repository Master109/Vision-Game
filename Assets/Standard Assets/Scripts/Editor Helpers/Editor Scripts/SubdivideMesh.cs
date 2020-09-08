#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using UnityEditor;
using System.IO;

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
}
#endif