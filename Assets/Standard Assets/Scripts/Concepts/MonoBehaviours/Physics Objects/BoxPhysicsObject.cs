using UnityEngine;
using Extensions;

namespace VisionGame
{
	public class BoxPhysicsObject : MeshPhysicsObject
	{
#if UNITY_EDITOR
		public bool autoSetMass;
		public bool autoSetBoxCollider;
#endif

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				childrenParent.localScale = Vector3.one.Divide(trs.lossyScale);
				BoxCollider boxCollider = collider as BoxCollider;
				if (autoSetBoxCollider && boxCollider != null)
				{
					BoxCollider _boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.center = _boxCollider.center;
					boxCollider.size = _boxCollider.size;
					DestroyImmediate(_boxCollider);
				}
				if (autoSetMass && rigid != null)
				{
					Bounds bounds = GetComponent<MeshRenderer>().bounds;
					rigid.mass = bounds.size.x * bounds.size.y * bounds.size.z;
					for (int i = 0; i < childrenParent.childCount; i ++)
					{
						Transform child = childrenParent.GetChild(i);
						bounds = child.GetComponent<MeshRenderer>().bounds;
						rigid.mass += bounds.size.x * bounds.size.y * bounds.size.z;
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