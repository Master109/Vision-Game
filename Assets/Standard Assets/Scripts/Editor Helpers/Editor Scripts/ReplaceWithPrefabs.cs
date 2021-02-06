#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Extensions;

public class ReplaceWithPrefabs : EditorScript
{
	public Transform[] replace;
	public Transform prefab;
	public bool copyMesh;

	public override void OnEnable ()
	{
		base.OnEnable ();
		Do ();
	}
	
	void Do ()
	{
		for (int i = 0; i < replace.Length; i ++)
		{
			Transform trs = replace[i];
			Transform clone = PrefabUtilityExtensions.ClonePrefabInstance(prefab.gameObject).GetComponent<Transform>();
			clone.position = trs.position;
			clone.rotation = trs.rotation;
			clone.SetParent(trs.parent);
			clone.localScale = trs.localScale;
			if (copyMesh)
			{
				MeshFilter meshFilter = trs.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					MeshFilter cloneMeshFilter = clone.GetComponent<MeshFilter>();
					if (cloneMeshFilter != null)
						cloneMeshFilter.mesh = meshFilter.mesh;
				}
			}
			DestroyImmediate(trs.gameObject);
		}
	}
}

[CustomEditor(typeof(ReplaceWithPrefabs))]
public class ReplaceWithPrefabsEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}
}
#endif