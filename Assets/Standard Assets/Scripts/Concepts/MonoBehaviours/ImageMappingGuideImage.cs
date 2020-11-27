using UnityEngine;
using System.Collections.Generic;
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
		ColorRegion[] colorRegions = new ColorRegion[this.texture.width * this.texture.height];
		Color[] colors = texture.GetPixels();
		int colorRegionIndex = 0;
		for (int x = 0; x < texture.width; x ++)
		{
			for (int y = 0; y < texture.height; y ++)
			{
				ColorRegion colorRegion = new ColorRegion();
				colorRegion.color = colors[colorRegionIndex];
				colorRegion.topLeft = new Vector2(x - 0.5f, y + 0.5f);
				colorRegion.topRight = new Vector2(x + 0.5f, y + 0.5f);
				colorRegion.bottomLeft = new Vector2(x - 0.5f, y - 0.5f);
				colorRegion.bottomRight = new Vector2(x + 0.5f, y - 0.5f);
				colorRegions[colorRegionIndex] = colorRegion;
				colorRegionIndex ++;
			}
		}
		colors = this.texture.GetPixels();
		int colorIndex = 0;
		List<KeyValuePair<Vector2Int, Vector2Int>> movements = new List<KeyValuePair<Vector2Int, Vector2Int>>();
		for (int x = 0; x < this.texture.width; x ++)
		{
			for (int y = 0; y < this.texture.height; y ++)
			{
				Color color = colors[colorIndex];
				Vector2Int correspondingPoint = TransformPoint(texture, new Vector2Int(x, y));
				ColorRegion colorRegion = colorRegions[correspondingPoint.y * texture.width + correspondingPoint.x];
				if (color != defaultColorType.color)
				{
					int colorIndex2 = 0;
					for (int x2 = 0; x2 < this.texture.width; x2 ++)
					{
						for (int y2 = 0; y2 < this.texture.height; y2 ++)
						{
							Color color2 = colors[colorIndex2];
							Vector2Int correspondingPoint2 = TransformPoint(texture, new Vector2Int(x2, y2));
							ColorRegion colorRegion2 = colorRegions[correspondingPoint.y * texture.width + correspondingPoint.x];
							if (color2.r == color.r && color2.g == color.g && color2.b == color.b && color2.a == 1f - color.a)
							{
								KeyValuePair<Vector2Int, Vector2Int> movement;
								if (color.a < color2.a)
									movement = new KeyValuePair<Vector2Int, Vector2Int>(correspondingPoint, correspondingPoint2);
								else
									movement = new KeyValuePair<Vector2Int, Vector2Int>(correspondingPoint2, correspondingPoint);
								movements.Add(movement);
							}
							colorIndex2 ++;
						}
					}
				}
				colorRegionIndex ++;
			}
		}
		for (int x = 0; x < this.texture.width; x ++)
		{
			for (int y = 0; y < this.texture.height; y ++)
			{
				ColorRegion colorRegion = colorRegions[y * this.texture.width + x];
				Vector2 topLeftMovement = Vector2.zero;
				Vector2 topRightMovement = Vector2.zero;
				Vector2 bottomLeftMovement = Vector2.zero;
				Vector2 bottomRightMovement = Vector2.zero;
				for (int i = 0; i < movements.Count; i ++)
				{
					KeyValuePair<Vector2Int, Vector2Int> movement = movements[i];
					// if ()
						// topLeftMovement += (movement.Value - colorRegion.topLeft).normalized * (movement.Key - movement.Key).magnitude;
				}
			}
		}
		return texture.DeepCopyByExpressionTree();
	}

	Vector2Int TransformPoint (Texture2D texture, Vector2Int point)
	{
		Vector2Int borderSize = new Vector2Int((this.texture.width - texture.width) / 2, (this.texture.height - texture.height) / 2);
		return point - borderSize;
	}

	Vector2Int InverseTransformPoint (Texture2D texture, Vector2Int point)
	{
		Vector2Int borderSize = new Vector2Int((this.texture.width - texture.width) / 2, (this.texture.height - texture.height) / 2);
		return point + borderSize;
	}

	public struct ColorRegion
	{
		public Color color;
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 bottomLeft;
		public Vector2 bottomRight;
		public LineSegment2D TopBorder
		{
			get
			{
				return new LineSegment2D(topLeft, topRight);
			}
		}
		public LineSegment2D BottomBorder
		{
			get
			{
				return new LineSegment2D(bottomLeft, bottomRight);
			}
		}
		public LineSegment2D LeftBorder
		{
			get
			{
				return new LineSegment2D(topLeft, bottomLeft);
			}
		}
		public LineSegment2D RightBorder
		{
			get
			{
				return new LineSegment2D(topRight, bottomRight);
			}
		}
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
		MeshTriangle[] meshTriangles = new MeshTriangle[mesh.vertexCount - 2];
		int triangleIndex = 0;
		for (int x = 0; x < texture.width; x ++)
		{
			for (int y = 0; y < texture.height; y ++)
			{
				if (triangleIndex % 2 == 0)
				{
					// if (triangleIndex == )
					// MeshTriangle meshTriangle = new MeshTriangle(mesh, trs, 0, 1);
				}
				triangleIndex ++;
			}
		}
		Object.DestroyImmediate(trs.gameObject);
		return mesh;
	}
}