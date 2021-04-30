#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using UnityEditor;
using System.IO;
using VisionGame;

[ExecuteInEditMode]
public class SubdivideMesh : EditorScript
{
	public static SubdivideMesh instance;
	public static SubdivideMesh Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<SubdivideMesh>();
			return instance;
		}
	}
	public MeshFilter meshFilter;
    public int subdivideCount = 1;
	public bool makeAsset;
	public string saveAssetAtPath;
	public bool autoNameAssetPath;

	public override void Do ()
	{
		Do (meshFilter, subdivideCount, makeAsset, autoNameAssetPath, saveAssetAtPath);
	}

	public static void Do (MeshFilter meshFilter, int subdivideCount = 1, bool makeAsset = false, bool autoNameAssetPath = false, string saveAssetAtPath = "")
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
				AssetDatabase.CreateAsset(meshFilter.sharedMesh, newAssetPath);
			}
			else
				AssetDatabase.CreateAsset(meshFilter.sharedMesh, saveAssetAtPath);
			AssetDatabase.SaveAssets();
		}
	}

	[MenuItem("Tools/Subdivide Mesh")]
	static void _SubdivideMesh ()
	{
		SubdivideMesh subdivideMesh = SubdivideMesh.Instance;
		for (int i = 0; i < Selection.transforms.Length; i ++)
		{
			Transform trs = Selection.transforms[i];
			MeshFilter meshFilter = trs.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				if (subdivideMesh == null)
					Do (meshFilter);
				else
					Do (meshFilter, 1, subdivideMesh.makeAsset, subdivideMesh.autoNameAssetPath, subdivideMesh.saveAssetAtPath);
				break;
			}
		}
	}
}
#endif