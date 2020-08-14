using UnityEngine;

public class RotateLocalPositionKeyframes : AnimationOperation
{
    public Vector3 rotate;

    public override void Do ()
    {
        for (int i = 0; i < anim.FrameCount; i ++)
            anim.keyFrames[i].localPosition = Quaternion.Euler(rotate) * anim.keyFrames[i].localPosition;
    }
}