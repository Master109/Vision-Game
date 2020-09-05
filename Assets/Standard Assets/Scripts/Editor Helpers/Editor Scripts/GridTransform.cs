#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class GridTransform : EditorScript
{
	public Transform trs;
	public float smallValue;
	protected Vector3 offset;

	public virtual void Start ()
	{
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
		if (trs.localScale.x % 2 == 0)
			offset.x = -.5f + smallValue;
		else
			offset.x = 0;
		if (trs.localScale.y % 2 != 0)
			offset.y = 0;
		else
			offset.y = -.5f + smallValue;
		if (trs.localScale.z % 2 == 0)
			offset.z = -.5f + smallValue;
		else
			offset.z = 0;
		trs.position = trs.position.Snap(Vector3.one) + offset;
	}
}
#endif
#if !UNITY_EDITOR
public class GridTransform : EditorScript
{
}
#endif