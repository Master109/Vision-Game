using UnityEngine;
using Extensions;

namespace VisionGame
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

		public bool ViewFrustumContainsPoint (Vector3 point)
		{
			Vector2 viewportPoint = camera.WorldToViewportPoint(point);
			return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
		}

		public bool ClosestPointOnViewFrustum (Vector3 point, out Vector3 output)
		{
			Vector3 viewportPoint = camera.WorldToViewportPoint(point);
			if (viewportPoint.x == .5f && viewportPoint.y == .5f)
			{
				output = VectorExtensions.NULL3;
				return false;
			}
			else
			{
				viewportPoint = viewportPoint.ClampComponents(Vector2.zero, Vector2.one);
				if (Mathf.Abs(viewportPoint.x - .5f) > Mathf.Abs(viewportPoint.y - .5f))
					viewportPoint.x = (viewportPoint.x > .5f).GetHashCode();
				else
					viewportPoint.y = (viewportPoint.y > .5f).GetHashCode();
			}
			Ray ray = camera.ViewportPointToRay(viewportPoint);
			Vector3 localPositionOfPoint = trs.InverseTransformPoint(point);
			Vector3 forwardVector = Vector3.Project(trs.forward * localPositionOfPoint.magnitude, localPositionOfPoint.normalized);
			float distance = Vector3.Project(forwardVector, ray.direction).magnitude;
			output = ray.GetPoint(distance);
			return true;
		}
	}
}