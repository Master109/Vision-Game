using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class BoxPhysicsObject : PhysicsObject
	{
#if UNITY_EDITOR
		public bool autoSetMass;
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				childrenParent.localScale = Vector3.one.Divide(trs.lossyScale);
				BoxCollider boxCollider = collider as BoxCollider;
				boxCollider.size = trs.lossyScale;
				if (autoSetMass)
				{
					rigid.mass = trs.lossyScale.x * trs.lossyScale.y * trs.lossyScale.z;
					for (int i = 0; i < childrenParent.childCount; i ++)
					{
						Transform child = childrenParent.GetChild(i);
						rigid.mass += child.lossyScale.x * child.lossyScale.y * child.lossyScale.z;
						Piston piston = child.GetComponent<Piston>();
						if (piston != null)
						{
							piston.SetMass ();
							rigid.mass += piston.rigid.mass;
						}
					}
				}
			}
#endif
			base.OnEnable ();
		}
	}
}