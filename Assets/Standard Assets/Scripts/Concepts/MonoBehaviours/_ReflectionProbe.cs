using UnityEngine;
using VisionGame;

public class _ReflectionProbe : UpdateWhileEnabled
{
	public Transform trs;
	public ReflectionProbe reflectionProbe;
	public float maxAnlgeToBlock;

	public override void DoUpdate ()
	{
		if (Vector3.Angle(Player.instance.headTrs.position - trs.position, Player.instance.headTrs.forward) <= maxAnlgeToBlock)
			reflectionProbe.RenderProbe ();
	}
}