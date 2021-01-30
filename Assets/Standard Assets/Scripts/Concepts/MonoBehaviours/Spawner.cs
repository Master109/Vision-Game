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
		do
		{
			SpawnZone spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];
			Vector3 destination = spawnZone.boxCollider.bounds.RandomPointOnBounds();
			Vector3 point = spawnZone.points[Random.Range(0, spawnZone.points.Length)].position;
			Vector3 toDestination = destination - point;
			Ray ray = new Ray(point, toDestination);
			RaycastHit hit;
			float distanceToDestination = toDestination.magnitude;
			if (Physics.SphereCast(ray, prefabRadius, out hit, distanceToDestination, whatICantSpawnIn))
			{
				distanceToDestination = hit.distance;
				if (distanceToDestination == 0)
					continue;
			}
			ObjectPool.instance.SpawnComponent<ISpawnable>(prefabIndex, ray.GetPoint(distanceToDestination * Random.value), Quaternion.LookRotation(Random.onUnitSphere));
			return;
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