#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PaintViewFrustumOnMesh : EditorScript
{
	public MeshFilter meshFilter;
	public Transform trs;
	public new Camera camera;
	public PaintType paintType;
	public Color outsideFrustumColor;
	public Color insideFrustumColor;
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
		BoundingFrustum boundingFrustum = new BoundingFrustum();
		boundingFrustum.Update (camera, camera.GetComponent<Transform>());
		Color[] meshColors = mesh.colors;
		for (int i = 0; i < mesh.vertexCount; i ++)
		{
			MeshExtensions.MeshVertex meshVertex = new MeshExtensions.MeshVertex(mesh, trs, i);
			if (boundingFrustum.Contains(ref meshVertex.point))
			{
				if (paintType == PaintType.Both || paintType == PaintType.Inside)
					meshColors[i] = insideFrustumColor;
			}
			else if (paintType == PaintType.Both || paintType == PaintType.Outside)
				meshColors[i] = outsideFrustumColor;
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