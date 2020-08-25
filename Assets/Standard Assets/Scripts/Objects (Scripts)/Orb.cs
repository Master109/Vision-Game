using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;

namespace VisionGame
{
	public class Orb : MonoBehaviour
	{
		public Transform trs;
		public Transform cameraTrs;
		public new Camera camera;
		public float checksPerUnit;
		public RectTransform checkerRectTrs;
		public Canvas checkerCanvas;
		public Transform capturedObjectsParent;
		public Transform oldCapturedObjectsParent;
		public LayerMask opaqueWallsLayermask;
		public LayerMask transparentWallsLayermask;
		public Rigidbody rigid;
		
		public void ReplaceObjects ()
		{
			int storedObjectsCount = capturedObjectsParent.childCount;
			for (int i = 0; i < storedObjectsCount; i ++)
				capturedObjectsParent.GetChild(0).SetParent(oldCapturedObjectsParent, true);
			List<Collider> hitColliders = new List<Collider>();
			for (float distance = 0; distance <= camera.farClipPlane; distance += 1f / checksPerUnit)
			{
				checkerCanvas.planeDistance = distance;
				checkerCanvas.enabled = false;
				checkerCanvas.enabled = true;
				Collider[] _hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, opaqueWallsLayermask);
				foreach (Collider hitCollider in _hitColliders)
				{
					if (!hitColliders.Contains(hitCollider))
					{
						RaycastHit hit;
						Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
						if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueWallsLayermask))
						{
							toHitPosition = hit.point - cameraTrs.position;
							if (!hitColliders.Contains(hit.collider))
								hitColliders.Add(hit.collider);
						}
						RaycastHit[] hits = Physics.RaycastAll(cameraTrs.position, toHitPosition, toHitPosition.magnitude, transparentWallsLayermask);
						foreach (RaycastHit hit2 in hits)
						{
							if (!hitColliders.Contains(hit2.collider))
								hitColliders.Add(hit2.collider);
						}
					}
				}
				_hitColliders = Physics.OverlapBox(checkerRectTrs.position, (checkerRectTrs.sizeDelta * checkerRectTrs.localScale.x).SetZ(Physics.defaultContactOffset), checkerRectTrs.rotation, transparentWallsLayermask);
				foreach (Collider hitCollider in _hitColliders)
				{
					if (!hitColliders.Contains(hitCollider))
					{
						RaycastHit hit;
						Vector3 toHitPosition = hitCollider.ClosestPoint(cameraTrs.position) - cameraTrs.position;
						if (Physics.Raycast(cameraTrs.position, toHitPosition, out hit, toHitPosition.magnitude, opaqueWallsLayermask))
						{
							toHitPosition = hit.point - cameraTrs.position;
							if (!hitColliders.Contains(hit.collider))
								hitColliders.Add(hit.collider);
						}
						else
							hitColliders.Add(hitCollider);
					}
				}
			}
			foreach (Collider hitCollider in hitColliders)
			{
				IStorable storable = hitCollider.GetComponent<IStorable>();
				if (storable != null)
					Instantiate(storable.Trs, storable.Trs.position, storable.Trs.rotation, capturedObjectsParent);
				IDestroyable destroyable = hitCollider.GetComponent<IDestroyable>();
				if (destroyable != null)
					Destroy(destroyable.Go);
			}
			oldCapturedObjectsParent.DetachChildren();
		}
	}
}