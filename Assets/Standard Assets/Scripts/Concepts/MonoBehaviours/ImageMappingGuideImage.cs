using UnityEngine;
using System.Collections.Generic;
using Extensions;
using System;

[Serializable]
public class ImageMappingGuideImage : GuideImage
{
	// public ColorType resistMovementColorType;

	// public override bool Apply (object obj)
	// {
	// 	Texture2D texture = obj as Texture2D;
	// 	if (texture != null)
	// 		return Apply(texture);
	// 	return false;
	// }

	// bool Apply (Texture2D texture)
	// {
	// 	Vector2Int borderSize = new Vector2Int((this.texture.width - texture.width) / 2, (this.texture.height - texture.height) / 2);
		
	// 	return true;
	// }

    public override object GetResultOfApply (object obj)
    {
		Texture2D texture = obj as Texture2D;
		if (texture != null)
        	return GetResultOfApply(texture);
		return obj.DeepCopyByExpressionTree();
    }

	Texture2D GetResultOfApply (Texture2D texture)
	{
		Color[] colors = this.texture.GetPixels();
		int colorIndex = 0;
		List<KeyValuePair<int, Vector2Int>> movements = new List<KeyValuePair<int, Vector2Int>>();
		for (int x = 0; x < this.texture.width; x ++)
		{
			for (int y = 0; y < this.texture.height; y ++)
			{
				Color color = colors[colorIndex];
				Vector2Int correspondingPoint = TransformPoint(texture, new Vector2Int(x, y));
				if (color != defaultColorType.color)
				{
					int colorIndex2 = 0;
					for (int x2 = 0; x2 < this.texture.width; x2 ++)
					{
						for (int y2 = 0; y2 < this.texture.height; y2 ++)
						{
							Color color2 = colors[colorIndex2];
							Vector2Int correspondingPoint2 = TransformPoint(texture, new Vector2Int(x2, y2));
							if (color2.r == color.r && color2.g == color.g && color2.b == color.b && color2.a == 1f - color.a)
							{
								KeyValuePair<int, Vector2Int> movement;
								if (color.a < color2.a)
									movement = new KeyValuePair<int, Vector2Int>(colorIndex, correspondingPoint2);
								else
									movement = new KeyValuePair<int, Vector2Int>(colorIndex2, correspondingPoint);
								movements.Add(movement);
							}
							colorIndex2 ++;
						}
					}
				}
				colorIndex ++;
			}
		}
		Mesh mesh = MeshFromImage(texture);
		for (int x = 0; x < this.texture.width; x ++)
		{
			for (int y = 0; y < this.texture.height; y ++)
			{
				for (int i = 0; i < movements.Count; i ++)
				{
					KeyValuePair<int, Vector2Int> movement = movements[i];
					mesh.vertices[movement.Key] += movement.Value.ToVec3();
					// if ()
						// topLeftMovement += (movement.Value - colorRegion.topLeft).normalized * (movement.Key - movement.Key).magnitude;
				}
			}
		}
		return texture.DeepCopyByExpressionTree();
	}

	int GetIndexOfLeftPixel (Texture2D texture, int pixelIndex)
	{
		if (pixelIndex % texture.width != 0)
			return pixelIndex - 1;
		else
			return -1;
	}

	int GetIndexOfRightPixel (Texture2D texture, int pixelIndex)
	{
		if (pixelIndex % texture.width != texture.width - 1)
			return pixelIndex + 1;
		else
			return -1;
	}

	int GetIndexOfUpperPixel (Texture2D texture, int pixelIndex)
	{
		if (pixelIndex < (texture.height - 1) * texture.width)
			return pixelIndex + texture.width;
		else
			return -1;
	}

	int GetIndexOfLowerPixel (Texture2D texture, int pixelIndex)
	{
		if (pixelIndex > texture.width - 1)
			return pixelIndex - texture.width;
		else
			return -1;
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
		// Mesh mesh = CreatePlane.CreateMesh(new Vector2Int(texture.width, texture.height), new Vector2(texture.width, texture.height), CreatePlane.Orientation.Vertical, true);
		Mesh mesh = CreatePlane.CreateGameObject(Vector3.zero, new Vector2Int(texture.width, texture.height), new Vector2(texture.width, texture.height), CreatePlane.Orientation.Vertical, true).GetComponent<MeshFilter>().sharedMesh;
		mesh.colors = new Color[mesh.vertexCount];
		Color[] colors = texture.GetPixels();
		int vertexIndex = 0;
		for (int x = 0; x < texture.width; x ++)
		{
			for (int y = 0; y < texture.height; y ++)
			{
				mesh.colors[vertexIndex] = colors[y * texture.width + x];
				vertexIndex ++;
			}
		}
		return mesh;
	}
}