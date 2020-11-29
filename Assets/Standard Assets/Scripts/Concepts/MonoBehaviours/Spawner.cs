using UnityEngine;
using Extensions;
using VisionGame;
using System;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
	public int prefabIndex;
	public int initSpawns;
	public SpawnZone[] spawnZones = new SpawnZone[0];
	public Timer spawnTimer;
	public float prefabRadius;
	public LayerMask whatICantSpawnIn;

	void OnEnable ()
	{
		for (int i = 0; i < initSpawns; i ++)
			Spawn ();
		spawnTimer.onFinished += Spawn;
		spawnTimer.Reset ();
		spawnTimer.Start ();
	}

	void Spawn (params object[] args)
	{
		Vector3 spawnPosition;
		do
		{
			SpawnZone spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];
			spawnPosition = spawnZone.boxCollider.bounds.RandomPoint();
			for (int i = 0; i < spawnZone.points.Length; i ++)
			{
				Transform point = spawnZone.points[i];
				Ray ray = new Ray(spawnPosition, point.position - spawnPosition);
				if (!Physics.SphereCast(ray, prefabRadius, (point.position - spawnPosition).magnitude, whatICantSpawnIn))
				{
					ObjectPool.instance.SpawnComponent<ISpawnable>(prefabIndex, spawnPosition, Quaternion.LookRotation(Random.onUnitSphere));
					return;
				}
			}
		} while (true);
	}

	void OnDisable ()
	{
		spawnTimer.Stop ();
		spawnTimer.onFinished -= Spawn;
	}

	[Serializable]
	public struct SpawnZone
	{
		public BoxCollider boxCollider;
		public Transform[] points;
	}
}