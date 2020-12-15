#if UNITY_EDITOR
using UnityEngine;

public class EditMesh : MonoBehaviour
{
	public MeshFilter meshFilter;
	public Transform trs;
	public bool update;

	void OnDrawGizmos ()
	{
		OnValidate ();
	}

	void OnValidate ()
	{
		Mesh mesh = Instantiate(meshFilter.sharedMesh);
		mesh.MarkDynamic();
		Ray ray = EditorScript.GetMouseRay();
	}
}
#endif