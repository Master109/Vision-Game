using VisionGame;

public class DespawnAfterTime : Spawnable
{
	public float time;
	ObjectPool.DelayedDespawn delayedDespawn;

	void OnEnable ()
	{
		ObjectPool.instance.DelayDespawn (prefabIndex, gameObject, trs, time);
	}

	void OnDestroy ()
	{
		ObjectPool.instance.CancelDelayedDespawn (delayedDespawn);
	}
}