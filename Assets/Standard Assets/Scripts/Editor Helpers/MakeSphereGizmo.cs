#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Extensions;

[ExecuteInEditMode]
public class MakeSphereGizmo : MonoBehaviour
{
    public Transform trs;
    public Color color = Color.white;

    void OnEnable ()
    {
        DrawGizmos (color);
    }

	void DrawGizmos (Color color)
	{
		GizmosManager.GizmosEntry gizmosEntry = new GizmosManager.GizmosEntry();
		gizmosEntry.setColor = true;
		gizmosEntry.color = color;
		gizmosEntry.onDrawGizmos += DrawGizmos;
		GizmosManager.gizmosEntries.Add(gizmosEntry);
	}

	void DrawGizmos (params object[] args)
	{
		Gizmos.DrawSphere(trs.position, trs.lossyScale.GetMaxComponent() / 2);
	}
}
#endif