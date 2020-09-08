#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class PaintSphereOnMesh : EditorScript
{
	public MeshFilter meshFilter;
	public Transform trs;
    public SphereCollider sphereCollider;
	public PaintType paintType;
	public Color outsideSphereColor;
	public Color insideSphereColor;
	public Color defaultColor;
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
		Color[] meshColors = mesh.colors;
		if (meshColors == null || meshColors.Length != mesh.vertexCount)
		{
			meshColors = new Color[mesh.vertexCount];
			for (int i = 0; i < mesh.vertexCount; i ++)
				meshColors[i] = defaultColor;
		}
		for (int i = 0; i < mesh.vertexCount; i ++)
		{
			MeshExtensions.MeshVertex meshVertex = new MeshExtensions.MeshVertex(mesh, trs, i, null);
			if (sphereCollider.ClosestPoint(meshVertex.point) == meshVertex.point)
			{
				if (paintType == PaintType.Both || paintType == PaintType.Inside)
					meshColors[i] = insideSphereColor;
			}
			else if (paintType == PaintType.Both || paintType == PaintType.Outside)
				meshColors[i] = outsideSphereColor;
		}
		mesh.colors = meshColors;
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

	public enum PaintType
	{
		Inside,
		Outside,
		Both
	}
}
#endif