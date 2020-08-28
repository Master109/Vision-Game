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
		}

		public override void HandlePosition ()
		{
			if (!enabled)
				return;
			InputManager.hmd = InputSystem.GetDevice<OculusHMD>();
			trs.localPosition = InputManager.hmd.devicePosition.ReadValue();
			trs.localRotation = InputManager.hmd.deviceRotation.ReadValue();
		}

		void OnDisable ()
		{
		}
	}
}