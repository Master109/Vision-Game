using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;
using Unity.XR.Oculus.Input;

namespace VisionGame
{
	public class GameCamera : CameraScript
	{
		public override void Awake ()
		{
			// trs.SetParent(null);
		}

		public override void HandlePosition ()
		{
			InputManager.hmd = InputSystem.GetDevice<OculusHMD>();
			trs.localPosition = InputManager.hmd.devicePosition.ReadValue();
			trs.localRotation = InputManager.hmd.deviceRotation.ReadValue();
		}
	}
}