using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;

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
                    if (hitGos.Contains(hitCollider.gameObject))
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
                IStorable storable = hitGo.GetComponent<IStorable>();
				if (storable != null)
					Instantiate(storable.Trs, storable.Trs.position, storable.Trs.rotation, capturedObjectsParent);
				IDestroyable destroyable = hitGo.GetComponent<IDestroyable>();
				if (destroyable != null)
					Destroy(hitGo);
			}
			oldCapturedObjectsParent.DetachChildren();
		}
	}
}