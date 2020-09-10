using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace VisionGame
{
	public class Orb : PhysicsObject
	{
		public Transform cameraTrs;
		public new Camera camera;
		public float checksPerUnit;
		public RectTransform checkerRectTrs;
		public Canvas checkerCanvas;
		public Transform capturedObjectsParent;
		public Transform oldCapturedObjectsParent;
		public LayerMask opaqueLayermask;
		public LayerMask transparentLayermask;
		
		public void ReplaceObjects ()
		{
			int storedObjectsCount = capturedObjectsParent.childCount;
			for (int i = 0; i < storedObjectsCount; i ++)
				capturedObjectsParent.GetChild(0).SetParent(oldCapturedObjectsParent, true);
			List<GameObject> hitGos = new List<GameObject>();
			for (float distance = 0; distance <= camera.farClipPlane; distance += 1f / checksPerUnit)
			{
				checkerCanvas.planeDistance = distance;
				checkerCanvas.enabled = false;
				checkerCanvas.enabled = true;
				Collider[] _hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, opaqueLayermask);
				for (int i = 0; i < _hitColliders.Length; i ++)
				{
					Collider hitCollider = _hitColliders[i];
					if (!hitGos.Contains(hitCollider.gameObject))
					{
						RaycastHit hit;
						Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
						if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueLayermask))
						{
							toHitPosition = hit.point - cameraTrs.position;
							if (!hitGos.Contains(hit.collider.gameObject))
								hitGos.Add(hit.collider.gameObject);
						}
						RaycastHit[] hits = Physics.RaycastAll(cameraTrs.position, toHitPosition, toHitPosition.magnitude, transparentLayermask);
						for (int i2 = 0; i2 < hits.Length; i2 ++)
						{
							RaycastHit hit2 = hits[i2];
							if (!hitGos.Contains(hit2.collider.gameObject))
								hitGos.Add(hit2.collider.gameObject);
						}
					}
				}
				_hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, transparentLayermask);
				for (int i = 0; i < _hitColliders.Length; i ++)
				{
					Collider hitCollider = _hitColliders[i];
					if (!hitGos.Contains(hitCollider.gameObject))
					{
						RaycastHit hit;
						Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
						if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueLayermask))
						{
							toHitPosition = hit.point - cameraTrs.position;
							if (!hitGos.Contains(hit.collider.gameObject))
								hitGos.Add(hit.collider.gameObject);
						}
						else
							hitGos.Add(hitCollider.gameObject);
					}
				}
			}
			for (int i = 0; i < hitGos.Count; i ++)
			{
				GameObject hitGo = hitGos[i];
				if (hitGo == gameObject)
					continue;
				else if (hitGo == GameManager.GetSingleton<Player>().gameObject)
				{
					Destroy (GameManager.GetSingleton<Player>().gameObject);
					return;
				}
				IStorable storable = hitGo.GetComponentInParent<IStorable>();
				if (storable != null)
				{
					PhysicsObject physicsObject = hitGo.GetComponentInParent<PhysicsObject>();
					if (physicsObject != null)
					{
						physicsObject.velocity = physicsObject.trs.InverseTransformDirection(physicsObject.rigid.velocity);
						print(physicsObject.velocity);
						physicsObject.angularVelocity = physicsObject.trs.InverseTransformDirection(physicsObject.rigid.angularVelocity);
						physicsObject.trs.SetParent(capturedObjectsParent);
					}
				}
			}
			oldCapturedObjectsParent.DetachChildren();
		}
	}
}