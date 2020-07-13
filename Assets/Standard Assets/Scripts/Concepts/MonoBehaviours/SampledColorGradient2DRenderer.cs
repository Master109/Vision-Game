using UnityEngine;

namespace BeatKiller
{
	[ExecuteInEditMode]
	public class SampledColorGradient2DRenderer : MonoBehaviour
	{
		public bool generateRandom;
		public bool update;
		public SampledColorGradient2D sampledColorGradient;
		public SpriteRenderer spriteRenderer;
		public Vector2Int textureSize;
		public float pixelsPerUnit;
		public int colorAreaCount;
		public int blurRadius;

#if UNITY_EDITOR
		public virtual void Update ()
		{
			if (Application.isPlaying)
				return;
			if (generateRandom)
			{
				generateRandom = false;
				GenerateRandom ();
			}
			if (update)
			{
				update = false;
				UpdateRenderer ();
			}
		}
#endif

		public virtual void GenerateRandom ()
		{
			sampledColorGradient = SampledColorGradient2D.GenerateRandom(textureSize, colorAreaCount, blurRadius);
		}

		public virtual void UpdateRenderer ()
		{
			spriteRenderer.sprite = sampledColorGradient.MakeSprite(textureSize, pixelsPerUnit);
		}
	}
}