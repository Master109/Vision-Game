#if UNITY_EDITOR
using UnityEngine;
using Extensions;
using VisionGame;

public class GridTransform : EditorScript
{
	public Transform trs;
	public float smallValue;
	public bool affectedByDistanceScale;
	protected Vector3 previousPosition;
	protected Vector3 offset;

	public virtual void Start ()
	{
		previousPosition = trs.position;
		if (!Application.isPlaying)
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			return;
		}
	}

	public override void DoEditorUpdate ()
	{
		trs.SetWorldScale(trs.lossyScale.Snap(Vector3.one));
		if (trs.lossyScale.x % 2 != 0)
			offset.x = 0;
		else
			offset.x = -.5f + smallValue;
		if (trs.lossyScale.y % 2 != 0)
			offset.y = 0;
		else
			offset.y = -.5f + smallValue;
		if (trs.lossyScale.z % 2 != 0)
			offset.z = 0;
		else
			offset.z = -.5f + smallValue;
		Vector3 newPosition;
		if (affectedByDistanceScale)
		{
			offset *= GameManager.Instance.distanceScale;
			newPosition = trs.position.Snap(Vector3.one * GameManager.Instance.distanceScale) + offset;
		}
		else
			newPosition = trs.position.Snap(Vector3.one) + offset;
		// if (newPosition != previousPosition)
		// 	Undo.RegisterCompleteObjectUndo(trs, "Snap " + name + " position");
		trs.position = newPosition;
		previousPosition = trs.position;
	}
}
#else
public class GridTransform : EditorScript
{
}
#endif