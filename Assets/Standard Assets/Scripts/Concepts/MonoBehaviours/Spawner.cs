using UnityEngine;
using Extensions;
using VisionGame;

public class Spawner : MonoBehaviour
{
    public int prefabIndex;
    public BoxCollider[] spawnBoxes = new BoxCollider[0];
    public Timer spawnTimer;
    public float prefabRadius;
	public LayerMask whatICantSpawnIn;

	void OnEnable ()
	{
        spawnTimer.onFinished += Spawn;
        spawnTimer.Reset ();
        spawnTimer.Start ();
	}

    void Spawn (params object[] args)
    {
        Vector3 spawnPosition;
        do
        {
            BoxCollider spawnBox = spawnBoxes[Random.Range(0, spawnBoxes.Length)];
            spawnPosition = spawnBox.bounds.RandomPoint();
        } while (Physics.CheckSphere(spawnPosition, prefabRadius, whatICantSpawnIn));
        ObjectPool.instance.SpawnComponent<ISpawnable>(prefabIndex, spawnPosition, Quaternion.LookRotation(Random.onUnitSphere));
    }

	void OnDisable ()
	{
        spawnTimer.Stop ();
        spawnTimer.onFinished -= Spawn;
	}
}