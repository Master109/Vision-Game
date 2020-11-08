using UnityEngine;
using System.Collections;
using UnityEditor;

public class SpriteProcessor : AssetPostprocessor
{
	void OnPostprocessTexture (Texture2D texture)
	{
		if (assetPath.ToLower().Contains("/sprites/")) 
		{
			TextureImporter textureImporter = (TextureImporter) assetImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
		}
	}
}