using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreateHallwayWizard : ScriptableWizard 
{
	public Transform startTrs;
	public Transform endTrs;
	public Transform[] cornerTransforms = new Transform[0];
	public Transform wallPrefab;
	public Vector2 size = Vector2.one;
	public float wallThickness = 1;
	public WallCreateMode leftWallCreateMode;
	public WallCreateMode rightWallCreateMode;
	public WallCreateMode bottomWallCreateMode;
	public WallCreateMode topWallCreateMode;

	[MenuItem ("Tools/Create Hallway...")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<CreateHallwayWizard> ("Create Hallway", "Create");
	}

	void OnWizardCreate ()
	{
		
	}

	public enum WallCreateMode
	{
		DoesNotExtendFully = 0,
		ExtendsFullyInPositiveDirectionOnly = 1,
		ExtendsFullyInNegativeDirectionOnly = -1,
		ExtendsFullyInBothDirections = 0
	}
}