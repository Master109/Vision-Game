using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using Extensions;
using UnityEditor;

public class MakeHallwayWizard : ScriptableWizard 
{
	public Transform wallPrefab;
	public Transform startTrs;
	public Transform endTrs;
	public Transform[] cornerTransforms = new Transform[0];
	public Transform parent;
	public Vector2 size = Vector2.one;
	public Vector2 wallThickness = Vector2.one;
	public WallMakeMode leftWallMakeMode;
	public WallMakeMode rightWallMakeMode;
	public WallMakeMode bottomWallMakeMode;
	public WallMakeMode topWallMakeMode;
	public Cap startCap;
	public Cap endCap;

	[MenuItem ("Tools/Make Hallway...")]
	static void MakeWizard ()
	{
		ScriptableWizard.DisplayWizard<MakeHallwayWizard> ("Make Hallway", "Make");
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
			Vector3 toNextTrs = nextPosition - currentPosition;
			Quaternion rotation = Quaternion.LookRotation(toNextTrs);
			
			Vector3 localScale = new Vector3(size.x + wallThickness.x * 2, wallThickness.y, toNextTrs.magnitude);
			Vector3 position = nextPosition - (toNextTrs / 2);
			Transform topWall = Instantiate(wallPrefab, position, rotation, parent);
			topWall.position += topWall.up * (size.y / 2 + wallThickness.y / 2);
			topWall.localScale = localScale;
			Undo.RegisterCreatedObjectUndo(topWall.gameObject, "Make Hallway");
			
			localScale = new Vector3(size.x + wallThickness.x * 2, wallThickness.y, toNextTrs.magnitude);
			position = nextPosition - (toNextTrs / 2);
			Transform bottomWall = Instantiate(wallPrefab, position, rotation, parent);
			bottomWall.position -= bottomWall.up * (size.y / 2 + wallThickness.y / 2);
			bottomWall.localScale = localScale;
			Undo.RegisterCreatedObjectUndo(bottomWall.gameObject, "Make Hallway");

			localScale = new Vector3(wallThickness.x, size.y + wallThickness.y * 2, toNextTrs.magnitude);
			position = nextPosition - (toNextTrs / 2);
			Transform rightWall = Instantiate(wallPrefab, position, rotation, parent);
			rightWall.position += rightWall.right * (size.x / 2 + wallThickness.x / 2);
			rightWall.localScale = localScale;
			Undo.RegisterCreatedObjectUndo(rightWall.gameObject, "Make Hallway");
			
			localScale = new Vector3(wallThickness.x, size.y + wallThickness.y * 2, toNextTrs.magnitude);
			position = nextPosition - (toNextTrs / 2);
			Transform leftWall = Instantiate(wallPrefab, position, rotation, parent);
			leftWall.position -= leftWall.right * (size.x / 2 + wallThickness.x / 2);
			leftWall.localScale = localScale;
			Undo.RegisterCreatedObjectUndo(rightWall.gameObject, "Make Hallway");

			currentPosition = nextPosition;
			nextTransforms.RemoveAt(0);
		} while (nextTransforms.Count > 0);
	}

	public enum WallMakeMode
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