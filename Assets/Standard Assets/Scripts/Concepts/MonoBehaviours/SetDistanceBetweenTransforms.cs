using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

[ExecuteInEditMode]
public class SetDistanceBetweenTransforms : UpdateWhileEnabled
{
	public Transform startTrs;
	public Transform endTrs;
    public float distance;
    public SetDistanceMode setDistanceMode;

	public override void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif
		base.OnEnable ();
	}

	public override void DoUpdate ()
	{
		if (startTrs.hasChanged || endTrs.hasChanged)
		{
            startTrs.hasChanged = false;
            endTrs.hasChanged = false;
            if (setDistanceMode == SetDistanceMode.MoveStart)
            {
                startTrs.position = endTrs.position + (startTrs.position - endTrs.position).normalized * (distance - (startTrs.position - endTrs.position).magnitude);
            }
            else if (setDistanceMode == SetDistanceMode.MoveEnd)
            {
                endTrs.position = startTrs.position.normalized * (distance - (startTrs.position - endTrs.position).magnitude);
            }
            else// if (setDistanceMode == SetDistanceMode.MoveBothEqually)
            {
                
            }
		}
	}

    public enum SetDistanceMode
    {
        MoveStart,
        MoveEnd,
        MoveBothEqually
    }
}