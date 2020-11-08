using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using Extensions;

public class CreateHallwayWizard : ScriptableWizard 
{
	public Transform startTrs;
	public Transform endTrs;
	public Transform[] cornerTransforms = new Transform[0];
	public Transform wallPrefab;
	public Transform parent;
	public Vector2 size = Vector2.one;
	public Vector2 wallThickness = Vector2.one;
	public WallCreateMode leftWallCreateMode;
	public WallCreateMode rightWallCreateMode;
	public WallCreateMode bottomWallCreateMode;
	public WallCreateMode topWallCreateMode;
	public Cap startCap;
	public Cap endCap;

	[MenuItem ("Tools/Create Hallway...")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<CreateHallwayWizard> ("Create Hallway", "Create");
	}

	void OnWizardCreate ()
	{
		Vector3 currentPosition = startTrs.position;
		List<Transform> nextTransforms = new List<Transform>(cornerTransforms);
		nextTransforms.Add(endTrs);
		do
		{
			Transform nextTrs = nextTransforms[0];
			Vector3 nextPosition = nextTrs.position;
			Transform topWall = Instantiate(wallPrefab, nextPosition + Vector3.up * wallThickness.y, Quaternion.identity, parent);
			Vector3 toNextTrs = nextPosition - currentPosition;
			Quaternion rotation = Quaternion.LookRotation(nextPosition - currentPosition);
			topWall.SetWorldScale (new Vector3(size.x, size.y + wallThickness.y, toNextTrs.magnitude));
			currentPosition = nextPosition;
			nextTransforms.RemoveAt(0);
		} while (nextTransforms.Count > 0);
	}

	public enum WallCreateMode
	{
		DoesNotExtendFully = 0,
		ExtendsFullyInPositiveDirectionOnly = 1,
		ExtendsFullyInNegativeDirectionOnly = -1,
		ExtendsFullyInBothDirections = 0
	}

	[Serializable]
	public struct Cap
	{
		public Transform capPrefab;
		public bool extendsLeftFully;
		public bool extendsRightFully;
		public bool extendsDownFully;
		public bool extendsUpFully;
	}
}