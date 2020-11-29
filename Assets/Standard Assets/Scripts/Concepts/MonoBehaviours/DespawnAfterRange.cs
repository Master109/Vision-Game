using VisionGame;

public class DespawnAfterRange : Spawnable
{
	public float range;
	ObjectPool.RangedDespawn rangedDespawn;

	void OnEnable ()
	{
		ObjectPool.instance.RangeDespawn (prefabIndex, gameObject, trs, range);
	}

	void OnDestroy ()
	{
		ObjectPool.instance.CancelRangedDespawn (rangedDespawn);
	}
}