using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace VisionGame
{
	public class Orb : SpherePhysicsObject
	{
		public Transform cameraTrs;
		public new Camera camera;
		public Transform capturedObjectsParent;
		public Transform oldCapturedObjectsParent;
		public Transform capturedObjectVisualizersParent;
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
			if (capturedObjectsParent == null)
				return;
			int storedObjectsCount = capturedObjectsParent.childCount;
			for (int i = 0; i < storedObjectsCount; i ++)
				capturedObjectsParent.GetChild(0).SetParent(oldCapturedObjectsParent);
			List<GameObject> hitGos = new List<GameObject>();
			int currentIndex = 0;
			for (float x = 0; x <= 1f; x += 1f / checksPerViewportAxis.x)
			{
				for (float y = 0; y <= 1f; y += 1f / checksPerViewportAxis.y)
				{
					Vector2 viewportPoint = new Vector2(x, y);
					Ray viewportRay = camera.ViewportPointToRay((Vector3) viewportPoint);
					float checkDistance = checkDistances[currentIndex];
					float checkDistanceRemaining = checkDistance;
					RaycastHit hit;
					GameObject hitGo;
					if (Physics.Raycast(viewportRay, out hit, checkDistanceRemaining, opaqueLayermask))
					{
						hitGo = hit.collider.gameObject;
						checkDistanceRemaining -= hit.distance;
						if (!hitGos.Contains(hitGo))
							hitGos.Add(hitGo);
						HandleHitGo (hitGo, viewportRay.direction, hit, checkDistanceRemaining, ref hitGos);
					}
					RaycastHit[] hits = Physics.RaycastAll(viewportRay, checkDistanceRemaining, transparentLayermask);
					for (int i = 0; i < hits.Length; i ++)
					{
						hit = hits[i];
						hitGo = hit.collider.gameObject;
						if (!hitGos.Contains(hitGo))
							hitGos.Add(hitGo);
						HandleHitGo (hitGo, viewportRay.direction, hit, checkDistance - hit.distance, ref hitGos);
					}
					currentIndex ++;
				}
			}
			Destroy(capturedObjectVisualizersParent.gameObject);
			capturedObjectVisualizersParent = new GameObject().GetComponent<Transform>();
			capturedObjectVisualizersParent.SetParent(cameraTrs);
			for (int i = 0; i < hitGos.Count; i ++)
			{
				GameObject hitGo = hitGos[i];
				if (hitGo == gameObject)
				{
					Destroy(gameObject);
					continue;
				}
				else if (hitGo == Player.instance.gameObject)
				{
					Destroy(Player.instance.gameObject);
					continue;
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
						Transform capturedVisualizerTrs = Instantiate(physicsObject.capturedVisualizerTrs, physicsObject.capturedVisualizerTrs.position, physicsObject.capturedVisualizerTrs.rotation, capturedObjectVisualizersParent);
						capturedVisualizerTrs.SetWorldScale (physicsObject.capturedVisualizerTrs.lossyScale);
						capturedVisualizerTrs.gameObject.SetActive(true);
					}
				}
			}
			oldCapturedObjectsParent.DetachChildren();
		}

		void HandleHitGo (GameObject go, Vector3 rayDirection, RaycastHit hit, float remainingDistance, ref List<GameObject> hitGos)
		{
			Mirror mirror = go.GetComponentInParent<Mirror>();
			if (mirror != null)
			{
				rayDirection = Vector3.Reflect(rayDirection, hit.normal);
				Ray ray = new Ray(hit.point, rayDirection);
				GameObject hitGo;
				float previousRemainingDistance = remainingDistance;
				if (Physics.Raycast(ray, out hit, remainingDistance, opaqueLayermask))
				{
					hitGo = hit.collider.gameObject;
					remainingDistance -= hit.distance;
					if (!hitGos.Contains(hitGo))
						hitGos.Add(hitGo);
					HandleHitGo (hitGo, rayDirection, hit, remainingDistance, ref hitGos);
				}
				RaycastHit[] hits = Physics.RaycastAll(ray, remainingDistance, transparentLayermask);
				for (int i = 0; i < hits.Length; i ++)
				{
					hit = hits[i];
					hitGo = hit.collider.gameObject;
					if (!hitGos.Contains(hitGo))
						hitGos.Add(hitGo);
					HandleHitGo (hitGo, rayDirection, hit, previousRemainingDistance - hit.distance, ref hitGos);
				}
			}
		}
	}
}