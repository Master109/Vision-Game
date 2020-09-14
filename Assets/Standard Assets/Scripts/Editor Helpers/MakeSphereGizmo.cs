#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Extensions;

public class MakeSphereGizmo : MonoBehaviour
{
    public Transform trs;
    public Color color = Color.white;

    void Update ()
    {
        DrawGizmos (color);
    }

	public virtual void DrawGizmos (Color color)
	{
		GizmosManager.GizmosEntry gizmosEntry = new GizmosManager.GizmosEntry();
		gizmosEntry.setColor = true;
		gizmosEntry.color = color;
		gizmosEntry.onDrawGizmos += DrawGizmos;
		GizmosManager.gizmosEntries.Add(gizmosEntry);
	}

	public virtual void DrawGizmos (params object[] args)
	{
		Gizmos.DrawSphere(trs.position, trs.lossyScale.GetMaxComponent());
	}
}
#endif