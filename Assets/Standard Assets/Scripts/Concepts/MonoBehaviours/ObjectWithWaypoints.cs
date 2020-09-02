using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class ObjectWithWaypoints : MonoBehaviour, IUpdatable, ICopyable
{
	public virtual bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public Transform trs;
	public Transform[] wayPoints = new Transform[0];
	public LineRenderer lineRenderer;
	public Transform lineTrs;
	public WaypointPath path;
#if UNITY_EDITOR
	public bool makeLineRenderer;
	public bool autoSetWaypoints;
	public Transform wayPointsParent;
	public new Collider collider;
	[SerializeField]
	Vector3 colliderSize;
	Vector3 fromPreviousPosition;
#endif

	public virtual void Start ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			if (collider == null)
				collider = GetComponent<Collider>();
			if (wayPointsParent == null)
				wayPointsParent = trs;
			EditorApplication.update += DoEditorUpdate;
			return;
		}
		else
			EditorApplication.update -= DoEditorUpdate;
#endif
		foreach (Transform waypoint in wayPoints)
			waypoint.SetParent(null);
	}

	public virtual void OnEnable ()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
		if (autoSetWaypoints)
			wayPoints = wayPointsParent.GetComponentsInChildren<Transform>().Remove(wayPointsParent);
		if (makeLineRenderer)
		{
			makeLineRenderer = false;
			MakeLineRenderer ();
		}
		colliderSize = collider.GetUnrotatedSize(trs);
#endif
	}

	public virtual void DoUpdate ()
	{
	}

#if UNITY_EDITOR
	public virtual void OnDestroy ()
	{
		if (Application.isPlaying)
			return;
		EditorApplication.update -= DoEditorUpdate;
	}
	
	public virtual void DoEditorUpdate ()
	{
		if (lineRenderer == null)
			return;
		lineRenderer.SetPosition(0, trs.position);
		for (int i = 0; i < wayPoints.Length; i ++)
		{
			if (i == 0)
				fromPreviousPosition = wayPoints[0].position - wayPoints[1].position;
			else
				fromPreviousPosition = wayPoints[i].position - wayPoints[i - 1].position;
			lineRenderer.SetPosition(i, wayPoints[i].position + (collider.GetUnrotatedSize(trs) / 2).Multiply(fromPreviousPosition.normalized));
		}
	}
	
	public virtual void MakeLineRenderer ()
	{
		if (lineRenderer != null)
			DestroyImmediate(lineRenderer.gameObject);
		lineRenderer = new GameObject().AddComponent<LineRenderer>();
		lineRenderer.positionCount = wayPoints.Length;
		for (int i = 0; i < wayPoints.Length; i ++)
		{
			if (i == 0)
				fromPreviousPosition = wayPoints[0].position - wayPoints[1].position;
			else
				fromPreviousPosition = wayPoints[i].position - wayPoints[i - 1].position;
			lineRenderer.SetPosition(i, wayPoints[i].position + (collider.GetUnrotatedSize(trs) / 2).Multiply(fromPreviousPosition.normalized));
		}
		lineRenderer.material = path.material;
		lineRenderer.startColor = path.color;
		lineRenderer.endColor = path.color;
		float lineWidth; // TODO: Make this work with a path that "turns" or "bends"
		if (wayPoints[0].position.x != wayPoints[1].position.x)
		{
			if (wayPoints[0].position.y != wayPoints[1].position.y)
			{
				lineWidth = Mathf.Max(collider.GetUnrotatedSize(trs).x, collider.GetUnrotatedSize(trs).y);
			}
			else
			{
				lineWidth = collider.GetUnrotatedSize(trs).y;
			}
		}
		else
			lineWidth = collider.GetUnrotatedSize().x;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;
		lineRenderer.sortingLayerName = path.sortingLayerName;
		lineRenderer.sortingOrder = path.sortingOrder;
		lineTrs = lineRenderer.GetComponent<Transform>();
		lineTrs.SetParent(trs);
	}
#endif

	public virtual void Copy (object copy)
	{
#if UNITY_EDITOR
		ObjectWithWaypoints objectWithWaypoints = copy as ObjectWithWaypoints;
		Transform newWaypoint;
		for (int i = 0; i < objectWithWaypoints.wayPoints.Length; i ++)
		{
			if (wayPoints.Length < objectWithWaypoints.wayPoints.Length)
			{
				newWaypoint = Instantiate(objectWithWaypoints.wayPoints[i], wayPointsParent);
				newWaypoint.position = objectWithWaypoints.wayPoints[i].position;
			}
			else if (wayPoints.Length > objectWithWaypoints.wayPoints.Length)
			{
				Destroy(wayPoints[i].gameObject);
			}
			else
			{
				wayPoints[i].position = objectWithWaypoints.wayPoints[i].position;
			}
		}
		wayPoints = wayPointsParent.GetComponentsInChildren<Transform>().Remove(wayPointsParent);
		MakeLineRenderer ();
#endif
	}
}