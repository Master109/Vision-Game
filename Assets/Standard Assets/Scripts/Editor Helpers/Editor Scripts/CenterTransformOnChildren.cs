#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Extensions;
using System;

public class CenterTransformOnChildren : EditorScript
{
	public Transform trs;

	public override void OnEnable ()
	{
		base.OnEnable ();
		Do ();
	}

	public virtual void Do ()
	{
		if (trs == null)
			throw new Exception(nameof(trs) + " is null");
		Transform[] children = new Transform[trs.childCount];
		Vector3[] points = new Vector3[trs.childCount];
		for (int i = 0; i < trs.childCount; i ++)
		{
			Transform child = trs.GetChild(i);
			points[i] = child.position;
			children[i] = child;
		}
		trs.DetachChildren();
		trs.position = BoundsExtensions.FromPoints(points).center;
		for (int i = 0; i < children.Length; i ++)
		{
			Transform child = children[i];
			child.SetParent(trs);
		}
	}
}

[CustomEditor(typeof(CenterTransformOnChildren))]
public class CenterTransformOnChildrenEditor : EditorScriptEditor
{
}
#else
public class CenterTransformOnChildren : EditorScript
{
}
#endif