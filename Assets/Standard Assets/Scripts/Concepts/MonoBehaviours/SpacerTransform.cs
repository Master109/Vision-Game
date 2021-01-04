using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[ExecuteInEditMode]
public class SpacerTransform : UpdateWhileEnabled
{
	public Transform trs;
	Vector3[] previousChildScales = new Vector3[0];

	public override void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			if (trs == null)
				trs = GetComponent<Transform>();
			return;
		}
#endif
		previousChildScales = new Vector3[trs.childCount];
		for (int i = 0; i < trs.childCount; i ++)
		{
			Transform child = trs.GetChild(i);
			previousChildScales[i] = child.lossyScale;
		}
		base.OnEnable ();
	}

	public override void DoUpdate ()
	{
		if (trs.hasChanged)
		{
			trs.hasChanged = false;
			for (int i = 0; i < trs.childCount; i ++)
			{
				Transform child = trs.GetChild(i);
				child.SetWorldScale(previousChildScales[i]);
				previousChildScales[i] = child.lossyScale;
			}
		}
	}
}