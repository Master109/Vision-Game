#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PaintSphereOnMesh : EditorScript
{
	public MeshFilter meshFilter;
	public Transform trs;
    public SphereCollider sphereCollider;
	public PaintType paintType;
	public Color outsideSphereColor;
	public Color insideSphereColor;
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
		Mesh mesh = meshFilter.sharedMesh;
		Color[] meshColors = mesh.colors;
		for (int i = 0; i < mesh.vertexCount; i ++)
		{
			MeshExtensions.MeshVertex meshVertex = new MeshExtensions.MeshVertex(mesh, trs, i);
			if (sphereCollider.ClosestPoint(meshVertex.point) == meshVertex.point)
			{
				if (paintType == PaintType.Both || paintType == PaintType.Inside)
					meshColors[i] = insideSphereColor;
			}
			else if (paintType == PaintType.Both || paintType == PaintType.Outside)
				meshColors[i] = outsideSphereColor;
		}
		mesh.colors = meshColors;
	}

	public enum PaintType
	{
		Inside,
		Outside,
		Both
	}
}
#endif