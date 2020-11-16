#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using UnityEditor;

public class GridTransform : EditorScript
{
	public Transform trs;
	public float smallValue;
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
		if (trs.localScale.x % 2 != 0)
			offset.x = 0;
		else
			offset.x = -.5f + smallValue;
		if (trs.localScale.y % 2 != 0)
			offset.y = 0;
		else
			offset.y = -.5f + smallValue;
		if (trs.localScale.z % 2 != 0)
			offset.z = 0;
		else
			offset.z = -.5f + smallValue;
		Vector3 newPosition = trs.position.Snap(Vector3.one) + offset;
		if (newPosition != previousPosition)
			Undo.RegisterCompleteObjectUndo(trs, "Snap " + name + " position");
		trs.position = newPosition;
		previousPosition = trs.position;
	}
}
#else
public class GridTransform : EditorScript
{
}
#endif