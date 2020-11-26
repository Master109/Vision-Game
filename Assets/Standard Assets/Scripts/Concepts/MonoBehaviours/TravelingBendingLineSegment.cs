using UnityEngine;
using Extensions;
using VisionGame;

public class TravelingBendingLineSegment : Spawnable, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public float moveSpeed;
	public float turnDegrees;
	public LayerMask whatICrashInto;
	public TrailRenderer trailRenderer;

	void OnEnable ()
	{
		trailRenderer.startColor = ColorExtensions.RandomColor().SetAlpha(trailRenderer.startColor.a);
		trailRenderer.endColor = trailRenderer.startColor;
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	public void DoUpdate ()
	{
		if (Physics.Raycast(trs.position, trs.forward, moveSpeed * Time.deltaTime, whatICrashInto))
		{
			if (Random.value < 0.5f)
			{
				if (!HandleTurning (trs.right))
				{
					if (!HandleTurning (trs.up))
						ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
				}
			}
			else if (!HandleTurning (trs.up))
			{
				if (!HandleTurning (trs.right))
					ObjectPool.instance.Despawn (prefabIndex, gameObject, trs);
			}
		}
		trs.position += trs.forward * moveSpeed * Time.deltaTime;
	}

	bool HandleTurning (Vector3 rotationAxis)
	{
		trs.RotateAround(trs.position, rotationAxis, turnDegrees);
		for (float angle = turnDegrees; angle < 360; angle += turnDegrees)
		{
			if (Physics.Raycast(trs.position, trs.forward, moveSpeed * Time.deltaTime, whatICrashInto))
				trs.RotateAround(trs.position, rotationAxis, turnDegrees);
			else
				return true;
		}
		return false;
	}

	void OnDisable ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}
}