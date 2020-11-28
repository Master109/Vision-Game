using UnityEngine;

[ExecuteInEditMode]
public class ApplyImageMappingGuideImage : MonoBehaviour
{
	public ImageMappingGuideImage imageMappingGuideImage;
	public Texture2D texture;
	public Texture2D outputTexture;

	void OnEnable ()
	{
		outputTexture = (Texture2D) imageMappingGuideImage.GetResultOfApply(texture);
	}
}