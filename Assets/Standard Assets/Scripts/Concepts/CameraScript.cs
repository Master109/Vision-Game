﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace BeatKiller
{
	//[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraScript : MonoBehaviour
	{
		public Transform trs;
		public new Camera camera;
		public Vector2 viewSize;
		protected Rect normalizedScreenViewRect;
		protected float screenAspect;
		[HideInInspector]
		public Rect viewRect;
		
		public virtual void Awake ()
		{
			trs.SetParent(null);
			trs.localScale = Vector3.one;
			viewRect.size = viewSize;
			HandlePosition ();
			HandleViewSize ();
		}

		public virtual void DoUpdate ()
		{
			HandlePosition ();
		}
		
		public virtual void HandlePosition ()
		{
			viewRect.center = trs.position;
		}
		
		public virtual void HandleViewSize ()
		{
			screenAspect = (float) Screen.width / Screen.height;
			camera.aspect = viewSize.x / viewSize.y;
			camera.orthographicSize = Mathf.Max(viewSize.x / 2 / camera.aspect, viewSize.y / 2);
			normalizedScreenViewRect = new Rect();
			normalizedScreenViewRect.size = new Vector2(camera.aspect / screenAspect, Mathf.Min(1, screenAspect / camera.aspect));
			normalizedScreenViewRect.center = Vector2.one / 2;
			camera.rect = normalizedScreenViewRect;
			viewRect.size = viewSize;
		}
	}
}