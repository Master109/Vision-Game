using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;
using Extensions;

namespace VisionGame
{
	public class InputManager : MonoBehaviour
	{
		public InputSettings settings;
		public static InputSettings Settings
		{
			get
			{
				return GameManager.GetSingleton<InputManager>().settings;
			}
		}
		public static bool UsingGamepad
		{
			get
			{
				return Gamepad.current != null;
			}
		}
		public static bool UsingMouse
		{
			get
			{
				return Mouse.current != null;
			}
		}
		public static bool UsingKeyboard
		{
			get
			{
				return Keyboard.current != null;
			}
		}
		public static bool LeftClickInput
		{
			get
			{
				if (UsingGamepad)
					return false;
				else
					return Mouse.current.leftButton.isPressed;
			}
		}
		public bool _LeftClickInput
		{
			get
			{
				return LeftClickInput;
			}
		}
		public static Vector2 MousePosition
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.position.ToVec2();
				else
					return VectorExtensions.NULL3;
			}
		}
		public Vector2 _MousePosition
		{
			get
			{
				return MousePosition;
			}
		}
		public static bool SubmitInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.aButton.isPressed;
				else
					return Keyboard.current.enterKey.isPressed;// || Mouse.current.leftButton.isPressed;
			}
		}
		public bool _SubmitInput
		{
			get
			{
				return SubmitInput;
			}
		}
		public static bool ReplaceInput
		{
			get
			{
				return leftTouchController.trigger.ReadValue() > Settings.defaultDeadzoneMin || rightTouchController.trigger.ReadValue() > Settings.defaultDeadzoneMin;
			}
		}
		public bool _ReplaceInput
		{
			get
			{
				return ReplaceInput;
			}
		}
		public static bool LeftGrabInput
		{
			get
			{
				return leftTouchController.grip.ReadValue() > Settings.defaultDeadzoneMin;
			}
		}
		public bool _LeftGrabInput
		{
			get
			{
				return LeftGrabInput;
			}
		}
		public static bool RightGrabInput
		{
			get
			{
				return rightTouchController.grip.ReadValue() > Settings.defaultDeadzoneMin;
			}
		}
		public bool _RightGrabInput
		{
			get
			{
				return RightGrabInput;
			}
		}
		public static bool TurnInput
		{
			get
			{
				return rightTouchController.primaryButton.isPressed;
			}
		}
		public bool _TurnInput
		{
			get
			{
				return TurnInput;
			}
		}
		public static bool OrbViewInput
		{
			get
			{
				return rightTouchController.secondaryButton.isPressed;
			}
		}
		public bool _OrbViewInput
		{
			get
			{
				return OrbViewInput;
			}
		}
		public static bool JumpInput
		{
			get
			{
				if (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.KeyboardAndMouse)
					return Keyboard.current.leftShiftKey.isPressed;
				else// if (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.OculusRift)
					return leftTouchController.thumbstickClicked.isPressed || rightTouchController.thumbstickClicked.isPressed;
			}
		}
		public bool _JumpInput
		{
			get
			{
				return JumpInput;
			}
		}
		public static Vector2 UIMovementInput
		{
			get
			{
				if (UsingGamepad)
					return Vector2.ClampMagnitude(Gamepad.current.leftStick.ToVec2(), 1);
				else
				{
					int x = 0;
					if (Keyboard.current.dKey.isPressed)
						x ++;
					if (Keyboard.current.aKey.isPressed)
						x --;
					int y = 0;
					if (Keyboard.current.wKey.isPressed)
						y ++;
					if (Keyboard.current.sKey.isPressed)
						y --;
					return Vector2.ClampMagnitude(new Vector2(x, y), 1);
				}
			}
		}
		public Vector2 _UIMovementInput
		{
			get
			{
				return UIMovementInput;
			}
		}
		public static OculusHMD hmd;
		public static OculusTouchController leftTouchController;
		public static OculusTouchController rightTouchController;
		public InputDevice inputDevice;
		
		// IEnumerator Start ()
		// {
		// 	do
		// 	{
		// 		hmd = InputSystem.GetDevice<OculusHMD>();
		// 		yield return new WaitForEndOfFrame();
		// 	} while (hmd == null);
		// 	do
		// 	{
		// 		leftTouchController = (OculusTouchController) OculusTouchController.leftHand;
		// 		yield return new WaitForEndOfFrame();
		// 	} while (leftTouchController == null);
		// 	do
		// 	{
		// 		rightTouchController = (OculusTouchController) OculusTouchController.rightHand;
		// 		yield return new WaitForEndOfFrame();
		// 	} while (rightTouchController == null);
		// 	yield break;
		// }

		public static float GetAxis (InputControl<float> positiveButton, InputControl<float> negativeButton)
		{
			return positiveButton.ReadValue() - negativeButton.ReadValue();
		}

		public static Vector2 GetAxis2D (InputControl<float> positiveXButton, InputControl<float> negativeXButton, InputControl<float> positiveYButton, InputControl<float> negativeYButton)
		{
			Vector2 output = new Vector2();
			output.x = positiveXButton.ReadValue() - negativeXButton.ReadValue();
			output.y = positiveYButton.ReadValue() - negativeYButton.ReadValue();
			output = Vector2.ClampMagnitude(output, 1);
			return output;
		}
		
		public enum InputDevice
		{
			OculusGo,
			OculusRift,
			KeyboardAndMouse
		}
	}
}