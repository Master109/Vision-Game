using UnityEngine;
using System;
using Extensions;
#if UNITY_EDITOR
using UnityEditor;
using VisionGame;
#endif

[ExecuteInEditMode]
public class CombineCubes : MonoBehaviour
{
	public Transform outputTrsPrefab;
	public CombineEntry[] combineEntries = new CombineEntry[0];

	public void OnEnable ()
	{
		if (combineEntries.Length == 0)
		   return;
		Bounds[] boundsArray = new Bounds[combineEntries.Length];
		for (int i = 0; i < combineEntries.Length; i ++)
		{
			CombineEntry combineEntry = combineEntries[i];
			boundsArray[i] = combineEntry.trs.GetBounds();
			DestroyImmediate(combineEntry.trs.gameObject);
		}
		Bounds bounds = boundsArray.Combine();
		Transform outputTrs = Instantiate(outputTrsPrefab, bounds.center, default(Quaternion));
		outputTrs.localScale = bounds.size;
		combineEntries = new CombineEntry[0];
	}

#if UNITY_EDITOR
	[MenuItem("Tools/Combine Cubes")]
	public static void _CombineCubes ()
	{
		CombineEntry[] combineEntries = new CombineEntry[Selection.transforms.Length];
        for (int i = 0; i < Selection.transforms.Length; i ++)
		{
            Transform trs = Selection.transforms[i];
            combineEntries[i] = new CombineEntry(trs);
		}
		Transform outputTrsPrefab = GameManager.GetSingleton<CombineCubes>().outputTrsPrefab;
		CombineCubes combineCubes = new GameObject().AddComponent<CombineCubes>();
		combineCubes.outputTrsPrefab = outputTrsPrefab;
		combineCubes.combineEntries = combineEntries;
		combineCubes.OnEnable ();
		DestroyImmediate(combineCubes);
	}
#endif

	[Serializable]
	public struct CombineEntry
	{
		public Transform trs;

		public CombineEntry (Transform trs)
		{
			this.trs = trs;
		}
	}
}