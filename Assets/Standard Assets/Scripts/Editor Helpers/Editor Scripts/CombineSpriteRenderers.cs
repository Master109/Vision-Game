#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Extensions;
using UnityEditor;
using VisionGame;
using System.IO;

[RequireComponent(typeof(SpriteRenderer))]
public class CombineSpriteRenderers : EditorScript
{
	public SpriteRenderer spriteRenderer;
	public string spriteAssetPath;
	public string textureAssetPath;
	public SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];
	public bool makeAssetPathsUnique;
	public bool update;
	Texture2D[] images = new Texture2D[0];

	public virtual void Start ()
	{
		if (!Application.isPlaying)
		{
			if (spriteRenderer != null)
				spriteRenderer = GetComponent<SpriteRenderer>();
			return;
		}
	}

	public override void DoEditorUpdate ()
	{
		if (!update)
			return;
		update = false;
		Do ();
	}

	public virtual void Do ()
	{
		Texture2D image = images[0];
		Texture2D texture = new Texture2D(image.width, image.height);
		Color[] colors = image.GetPixels();
		for (int i = 1; i < images.Length; i ++)
		{
			Color[] otherColors = images[i].GetPixels();
			for (int i2 = 0; i2 < otherColors.Length; i2 ++)
			{
				Color otherColor = otherColors[i2];
				if (otherColor.a > 0)
					colors[i2] = otherColor;
			}
		}
		texture.SetPixels(colors);
		texture.Apply();
		if (makeAssetPathsUnique)
		{
			string newAssetPath = textureAssetPath;
			while (File.Exists(newAssetPath))
				newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			textureAssetPath = newAssetPath;
			AssetDatabase.CreateAsset(texture, newAssetPath);
		}
		else
			AssetDatabase.CreateAsset(texture, textureAssetPath);
		AssetDatabase.Refresh();
		Sprite sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, texture.width, texture.height), Vector2.one / 2);
		if (makeAssetPathsUnique)
		{
			string newAssetPath = spriteAssetPath;
			while (File.Exists(newAssetPath))
				newAssetPath = newAssetPath.Replace(".asset", "1.asset");
			spriteAssetPath = newAssetPath;
			AssetDatabase.CreateAsset(sprite, newAssetPath);
		}
		else
			AssetDatabase.CreateAsset(sprite, spriteAssetPath);
		AssetDatabase.Refresh();
		if (spriteRenderer != null)
			spriteRenderer.sprite = sprite;
	}
}

[CanEditMultipleObjects]
[CustomEditor(typeof(CombineSpriteRenderers))]
public class CombineSpriteRenderersEditor : EditorScriptEditor
{
}
#endif
#if !UNITY_EDITOR
public class CombineSpriteRenderers : EditorScript
{
}
#endif