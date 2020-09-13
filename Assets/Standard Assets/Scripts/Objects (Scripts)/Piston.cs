using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Piston : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		public Transform trs;
		public Transform axelParent;
		public Transform axelTrs;
		public float axelOffset;
		public float extendAxel;
		public float childObjectsOffset;
		public Transform childObjectsParent;
		public IgnoreCollisionEntry[] ignoreCollisionEntries = new IgnoreCollisionEntry[0];
		public Rigidbody rigid;
		public float moveDistance;
		public float moveSpeed;
		public bool repeat;
		[HideInInspector]
		public bool moveTowardsEnd = true;
#if UNITY_EDITOR
		public float distanceToAxelEnd;
#endif
		LineSegment3D lineSegment;

		void Awake ()
		{
			for (int i = 0; i < ignoreCollisionEntries.Length; i ++)
			{
				IgnoreCollisionEntry ignoreCollisionEntry = ignoreCollisionEntries[i];
				Physics.IgnoreCollision(ignoreCollisionEntry.collider, ignoreCollisionEntry.otherCollider, true);
			}
			lineSegment = new LineSegment3D(axelParent.position, axelParent.position + (axelParent.forward * moveDistance));
		}

		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		IEnumerator Start ()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			axelTrs.SetParent(axelParent);
			rigid.isKinematic = false;
		}

#if UNITY_EDITOR
		void OnValidate ()
		{
			lineSegment = new LineSegment3D(trs.position + (trs.forward * (-Mathf.Abs(moveDistance) / 2 - axelOffset - extendAxel / 2 + distanceToAxelEnd)), trs.position + trs.forward * (Mathf.Abs(moveDistance) / 2 - axelOffset + extendAxel / 2 + distanceToAxelEnd));
			axelParent.position = lineSegment.start;
			axelParent.localScale = axelParent.localScale.SetZ(lineSegment.GetLength());
			childObjectsParent.position = trs.position + (lineSegment.GetDirection() * (childObjectsOffset + extendAxel + distanceToAxelEnd));
			childObjectsParent.SetWorldScale(Vector3.one);
		}
#endif

		public void DoUpdate ()
		{
			axelTrs.position = lineSegment.ClosestPoint(axelTrs.position);
			if (axelTrs.position == lineSegment.start || axelTrs.position == lineSegment.end)
			{
				if (repeat)
					moveTowardsEnd = !moveTowardsEnd;
				else
				{
					GameManager.updatables = GameManager.updatables.Remove(this);
					return;
				}
			}
			if (moveTowardsEnd)
				rigid.velocity = lineSegment.GetDirection() * moveSpeed;
			else
				rigid.velocity = -lineSegment.GetDirection() * moveSpeed;
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		[Serializable]
		public class IgnoreCollisionEntry
		{
			public Collider collider;
			public Collider otherCollider;
		}
	}
}