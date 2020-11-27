using UnityEngine;
using MeshVertex = Extensions.MeshExtensions.MeshVertex;
using MeshTriangle = Extensions.MeshExtensions.MeshTriangle;

public class ImageMappingGuideImage : GuideImage
{
	public ColorType resistMovementColorType;

	public override bool Apply (object obj)
	{
		Texture2D texture = obj as Texture2D;
		if (texture != null)
			return Apply(texture);
		return false;
	}

	bool Apply (Texture2D texture)
	{
		Vector2Int borderSize = new Vector2Int((this.texture.width - texture.width) / 2, (this.texture.height - texture.height) / 2);
		
		return true;
	}

    public override object GetResultOfApply (object obj)
    {
		Texture2D texture = obj as Texture2D;
		if (texture != null)
        	return GetResultOfApply(texture);
		return obj.DeepCopyByExpressionTree();
    }

	Texture2D GetResultOfApply (Texture2D texture)
	{
		Vector2Int borderSize = new Vector2Int((this.texture.width - texture.width) / 2, (this.texture.height - texture.height) / 2);
		
		return texture.DeepCopyByExpressionTree();
	}

	Mesh MeshFromImage (Texture2D texture)
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[texture.width * texture.height];
		mesh.triangles = new int[mesh.vertexCount * 3];
		Transform trs = new GameObject().AddComponent<Transform>();
		// MeshVertex[] meshVertices = new MeshVertex[mesh.vertexCount];
		// int vertexIndex = 0;
		// for (int x = 0; x < texture.width; x ++)
		// {
		// 	for (int y = 0; y < texture.height; y ++)
		// 	{
		// 		meshVertices[vertexIndex] = new MeshVertex(mesh, trs, 0, new Vector3(x, y));
		// 		vertexIndex ++;
		// 	}
		// }
		Object.DestroyImmediate(trs.gameObject);
		return mesh;
	}
}