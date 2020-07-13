using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;
using Unity.XR.Oculus.Input;

namespace BeatKiller
{
	public class GameCamera : CameraScript
	{
		public override void Awake ()
		{
			trs.SetParent(null);
		}

		public override void HandlePosition ()
		{
			InputManager.hmd = InputSystem.GetDevice<OculusHMD>();
			trs.position = InputManager.hmd.devicePosition.ReadValue();
			trs.rotation = InputManager.hmd.deviceRotation.ReadValue();
		}
	}
}