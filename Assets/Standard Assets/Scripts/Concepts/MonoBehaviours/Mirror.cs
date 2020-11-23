using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Mirror : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Sprite sprite;
		public Camera camera;
		public Transform trs;
		public Transform cameraTrs;
		public Transform viewerTrs;
		public Vector2Int viewPointCount;
		public Transform[] viewPoints = new Transform[0];
		Texture2D texture;
		
		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				for (int i = 1; i < trs.childCount; i ++)
				{
					DestroyImmediate(trs.GetChild(1).gameObject);
					i --;
				}
				viewPointCount.x = Mathf.CeilToInt(Mathf.Abs(trs.lossyScale.x) * sprite.pixelsPerUnit);
				viewPointCount.y = Mathf.CeilToInt(Mathf.Abs(trs.lossyScale.y) * sprite.pixelsPerUnit);
				viewPoints = new Transform[viewPointCount.x * viewPointCount.y];
				int viewPointIndex = 0;
				float xIncrement = 1f / (viewPointCount.x + 1);
				float yIncrement = 1f / (viewPointCount.y + 1);
				for (float x = -0.5f + xIncrement / 2; x <= 0.5f - xIncrement / 2; x += xIncrement)
				{
					for (float y = -0.5f + yIncrement / 2; y <= 0.5f - yIncrement / 2; y += yIncrement)
					{
						Transform viewPoint = new GameObject().GetComponent<Transform>();
						viewPoint.SetParent(trs);
						viewPoint.localPosition = new Vector2(x, y);
						viewPoints[viewPointIndex] = viewPoint;
						viewPointIndex ++;
					}
				}
				return;
			}
#endif
			sprite.texture.Resize(viewPointCount.x, viewPointCount.y);
			sprite.texture.Apply();
			texture = new Texture2D(1, 1);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			RenderTexture.active = camera.targetTexture;
			Color[] colors = new Color[viewPointCount.x * viewPointCount.y];
			int viewPointIndex = 0;
			for (int x = 0; x < viewPointCount.x; x ++)
			{
				for (int y = 0; y < viewPointCount.y; y ++)
				{
					Transform viewPoint = viewPoints[viewPointIndex];
					Vector3 forward = Vector3.Reflect(viewPoint.position - viewerTrs.position, -trs.forward);
					cameraTrs.forward = forward;
					camera.Render();
					texture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
					colors[y * viewPointCount.x + x] = texture.GetPixel(0, 0);
					viewPointIndex ++;
				}
			}
			sprite.texture.SetPixels(colors);
			sprite.texture.Apply();
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}