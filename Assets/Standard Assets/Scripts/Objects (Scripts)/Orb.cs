using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace VisionGame
{
	public class Orb : SpherePhysicsObject
	{
		public Transform cameraTrs;
		public new Camera camera;
		// public float checksPerUnit;
		// public RectTransform checkerRectTrs;
		// public Canvas checkerCanvas;
		public Transform capturedObjectsParent;
		public Transform oldCapturedObjectsParent;
		public LayerMask opaqueLayermask;
		public LayerMask transparentLayermask;
		public Vector2Int checksPerViewportAxis;
		public float[] checkDistances = new float[0];

#if UNITY_EDITOR
		void OnValidate ()
		{
			checkDistances = new float[(checksPerViewportAxis.x + 1) * (checksPerViewportAxis.y + 1)];
			int currentIndex = 0;
			Plane plane = new Plane(cameraTrs.forward, cameraTrs.position + cameraTrs.forward * camera.farClipPlane);
			for (float x = 0; x <= 1f; x += 1f / checksPerViewportAxis.x)
			{
				for (float y = 0; y <= 1f; y += 1f / checksPerViewportAxis.y)
				{
					Vector2 viewportPoint = new Vector2(x, y);
					Ray viewportRay = camera.ViewportPointToRay((Vector3) viewportPoint);
					float hitDistance = 0;
					plane.Raycast(viewportRay, out hitDistance);
					checkDistances[currentIndex] = hitDistance;
					currentIndex ++;
				}
			}
		}
#endif
		
		public void ReplaceObjects ()
		{
			int storedObjectsCount = capturedObjectsParent.childCount;
			for (int i = 0; i < storedObjectsCount; i ++)
				capturedObjectsParent.GetChild(0).SetParent(oldCapturedObjectsParent, true);
			List<GameObject> hitGos = new List<GameObject>();
			// for (float distance = 0; distance <= camera.farClipPlane; distance += 1f / checksPerUnit)
			// {
			// 	checkerCanvas.planeDistance = distance;
			// 	checkerCanvas.enabled = false;
			// 	checkerCanvas.enabled = true;
			// 	Collider[] _hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, opaqueLayermask);
			// 	for (int i = 0; i < _hitColliders.Length; i ++)
			// 	{
			// 		Collider hitCollider = _hitColliders[i];
			// 		if (!hitGos.Contains(hitCollider.gameObject))
			// 		{
			// 			RaycastHit hit;
			// 			Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
			// 			if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueLayermask))
			// 			{
			// 				toHitPosition = hit.point - cameraTrs.position;
			// 				if (!hitGos.Contains(hit.collider.gameObject))
			// 					hitGos.Add(hit.collider.gameObject);
			// 			}
			// 			RaycastHit[] hits = Physics.RaycastAll(cameraTrs.position, toHitPosition, toHitPosition.magnitude, transparentLayermask);
			// 			for (int i2 = 0; i2 < hits.Length; i2 ++)
			// 			{
			// 				RaycastHit hit2 = hits[i2];
			// 				if (!hitGos.Contains(hit2.collider.gameObject))
			// 					hitGos.Add(hit2.collider.gameObject);
			// 			}
			// 		}
			// 	}
			// 	_hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, transparentLayermask);
			// 	for (int i = 0; i < _hitColliders.Length; i ++)
			// 	{
			// 		Collider hitCollider = _hitColliders[i];
			// 		if (!hitGos.Contains(hitCollider.gameObject))
			// 		{
			// 			RaycastHit hit;
			// 			Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
			// 			if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueLayermask))
			// 			{
			// 				toHitPosition = hit.point - cameraTrs.position;
			// 				if (!hitGos.Contains(hit.collider.gameObject))
			// 					hitGos.Add(hit.collider.gameObject);
			// 			}
			// 			else
			// 				hitGos.Add(hitCollider.gameObject);
			// 		}
			// 	}
			// }
			int currentIndex = 0;
			for (float x = 0; x <= 1f; x += 1f / checksPerViewportAxis.x)
			{
				for (float y = 0; y <= 1f; y += 1f / checksPerViewportAxis.y)
				{
					Vector2 viewportPoint = new Vector2(x, y);
					Ray viewportRay = camera.ViewportPointToRay((Vector3) viewportPoint);
					float checkDistance = checkDistances[currentIndex];
					RaycastHit hit;
					GameObject hitGo;
					if (Physics.Raycast(viewportRay, out hit, checkDistance, opaqueLayermask))
					{
						hitGo = hit.collider.gameObject;
						if (!hitGos.Contains(hitGo))
							hitGos.Add(hitGo);
						RaycastHit[] hits = Physics.RaycastAll(viewportRay, hit.distance, transparentLayermask);
						for (int i = 0; i < hits.Length; i ++)
						{
							hit = hits[i];
							hitGo = hit.collider.gameObject;
							if (!hitGos.Contains(hitGo))
								hitGos.Add(hitGo);
						}
					}
					else
					{
						RaycastHit[] hits = Physics.RaycastAll(viewportRay, checkDistance, transparentLayermask);
						for (int i = 0; i < hits.Length; i ++)
						{
							hit = hits[i];
							hitGo = hit.collider.gameObject;
							if (!hitGos.Contains(hitGo))
								hitGos.Add(hitGo);
						}
					}
					currentIndex ++;
				}
			}
			for (int i = 0; i < hitGos.Count; i ++)
			{
				GameObject hitGo = hitGos[i];
				if (hitGo == gameObject)
					continue;
				else if (hitGo == Player.Instance.gameObject)
				{
					GameManager.Instance.ReloadActiveScene ();
					return;
				}
				IStorable storable = hitGo.GetComponentInParent<IStorable>();
				if (storable != null)
				{
					PhysicsObject physicsObject = hitGo.GetComponentInParent<PhysicsObject>();
					if (physicsObject != null)
					{
						physicsObject.velocity = physicsObject.trs.InverseTransformDirection(physicsObject.rigid.velocity);
						physicsObject.angularVelocity = physicsObject.trs.InverseTransformDirection(physicsObject.rigid.angularVelocity);
						physicsObject.trs.SetParent(capturedObjectsParent);
					}
				}
			}
			oldCapturedObjectsParent.DetachChildren();
		}
	}
}