using UnityEngine;
using Extensions;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class PhysicsObject : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public Transform trs;
		public Rigidbody rigid;
		[HideInInspector]
		public Vector3 velocity;

		void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				if (rigid == null)
					rigid = GetComponent<Rigidbody>();
				return;
			}
#endif
			rigid.velocity = trs.TransformDirection(velocity);
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			rigid.velocity += Physics.gravity * Time.deltaTime;
		}

		void OnDisable ()
		{
			velocity = trs.InverseTransformDirection(rigid.velocity);
			GameManager.updatables = GameManager.updatables.Remove(this);
		}
	}
}