using UnityEngine;
 
public class CreatePlane : MonoBehaviour
{
	public static GameObject CreateGameObject (Vector3 position, Vector2Int segments, Vector2 size, Orientation orientation, bool isTwoSided, Vector2 anchorOffset = default(Vector2), string name = default(string))
	{
		GameObject plane = new GameObject();
		if (!string.IsNullOrEmpty(name))
			plane.name = name;
		else
			plane.name = "Plane";
		plane.GetComponent<Transform>().position = position;
		MeshFilter meshFilter = (MeshFilter) plane.AddComponent(typeof(MeshFilter));
		plane.AddComponent(typeof(MeshRenderer));
		Mesh mesh = CreateMesh(new Vector2Int(segments.x, segments.y), new Vector2(size.x, size.y), orientation, isTwoSided, anchorOffset);
		mesh.name = plane.name;
		meshFilter.sharedMesh = mesh;
		mesh.RecalculateBounds();
		return plane;
	}

	public static Mesh CreateMesh (Vector2Int segments, Vector2 size, Orientation orientation, bool isTwoSided, Vector2 anchorOffset = default(Vector2))
	{
		Mesh mesh = new Mesh();
		int hCount2 = segments.x + 1;
		int vCount2 = segments.y + 1;
		int numTriangles = segments.x * segments.y * 6;
		if (isTwoSided)
			numTriangles *= 2;
		int numVertices = hCount2 * vCount2;
		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] triangles = new int[numTriangles];
		Vector4[] tangents = new Vector4[numVertices];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		int index = 0;
		float uvFactorX = 1.0f / segments.x;
		float uvFactorY = 1.0f / segments.y;
		float scaleX = size.x / segments.x;
		float scaleY = size.y / segments.y;
		for (float y = 0.0f; y < vCount2; y ++)
		{
			for (float x = 0.0f; x < hCount2; x ++)
			{
				if (orientation == Orientation.Horizontal)
					vertices[index] = new Vector3(x * scaleX - size.x / 2 - anchorOffset.x, 0, y * scaleY - size.y / 2 - anchorOffset.y);
				else
					vertices[index] = new Vector3(x * scaleX - size.x / 2 - anchorOffset.x, y * scaleY - size.y / 2 - anchorOffset.y, 0);
				tangents[index] = tangent;
				uvs[index ++] = new Vector2(x * uvFactorX, y * uvFactorY);
			}
		}
		index = 0;
		for (int y = 0; y < segments.y; y ++)
		{
			for (int x = 0; x < segments.x; x ++)
			{
				triangles[index] = (y * hCount2) + x;
				triangles[index + 1] = ((y + 1) * hCount2) + x;
				triangles[index + 2] = (y * hCount2) + x + 1;

				triangles[index + 3] = ((y + 1) * hCount2) + x;
				triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
				triangles[index + 5] = (y * hCount2) + x + 1;
				index += 6;
			}
			if (isTwoSided)
			{
				// Same tri vertices with order reversed, so normals point in the opposite direction
				for (int x = 0; x < segments.x; x ++)
				{
					triangles[index] = (y * hCount2) + x;
					triangles[index + 1] = (y * hCount2) + x + 1;
					triangles[index + 2] = ((y + 1) * hCount2) + x;

					triangles[index + 3] = ((y + 1) * hCount2) + x;
					triangles[index + 4] = (y * hCount2) + x + 1;
					triangles[index + 5] = ((y + 1) * hCount2) + x + 1;
					index += 6;
				}
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.tangents = tangents;
		mesh.RecalculateNormals();
		return mesh;
	}
	
	public enum Orientation
	{
		Horizontal,
		Vertical
	}
 
	public enum AnchorPoint
	{
		TopLeft,
		TopHalf,
		TopRight,
		RightHalf,
		BottomRight,
		BottomHalf,
		BottomLeft,
		LeftHalf,
		Center
	}
}