﻿using UnityEngine;
using Extensions;
using UnityEngine.InputSystem;
using Unity.XR.Oculus.Input;

namespace VisionGame
{
	public class GameCamera : CameraScript
	{
		public Vector2 sensitivity;
		public FloatRange yRange;
		float rotationY;
		Vector2 previousMousePosition;

		public override void Awake ()
		{
		}

		public override void HandlePosition ()
		{
			InputManager.hmd = InputSystem.GetDevice<OculusHMD>();
			if (InputManager.hmd == null || InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
			{
				Vector2 mousePositionDelta = Mouse.current.delta.ToVec2();
				float rotationX = trs.localEulerAngles.y + mousePositionDelta.x * sensitivity.x;
				rotationY += mousePositionDelta.y * sensitivity.y;
				rotationY = Mathf.Clamp(rotationY, yRange.min, yRange.max);
				trs.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			}
			else
			{
				trs.localPosition = InputManager.hmd.devicePosition.ReadValue();
				trs.localRotation = InputManager.hmd.deviceRotation.ReadValue();
			}
		}
	}
}