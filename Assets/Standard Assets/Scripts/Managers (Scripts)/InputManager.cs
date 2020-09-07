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
		public InputDevice inputDevice;
		public static InputDevice _InputDevice
		{
			get
			{
				return GameManager.GetSingleton<InputManager>().inputDevice;
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
				if (UsingMouse)
					return Mouse.current.leftButton.isPressed;
				else
					return false;
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
		public static Vector2 MouseMovement
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.delta.ToVec2();
				else
					return Vector2.zero;
			}
		}
		public Vector2 _MouseMovement
		{
			get
			{
				return MouseMovement;
			}
		}
		public static bool SubmitInput
		{
			get
			{
				if (UsingGamepad)
					return Gamepad.current.aButton.isPressed;
				else if (UsingKeyboard)
					return Keyboard.current.enterKey.isPressed;// || Mouse.current.leftButton.isPressed;
				else
					return false;
			}
		}
		public bool _SubmitInput
		{
			get
			{
				return SubmitInput;
			}
		}
		public static bool LeftReplaceInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && leftTouchController != null && leftTouchController.trigger.ReadValue() > Settings.defaultDeadzoneMin)
					return true;
				else if (Mouse.current.leftButton.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _LeftReplaceInput
		{
			get
			{
				return LeftReplaceInput;
			}
		}
		public static bool RightReplaceInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && rightTouchController != null && rightTouchController.trigger.ReadValue() > Settings.defaultDeadzoneMin)
					return true;
				else if (Mouse.current.rightButton.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _RightReplaceInput
		{
			get
			{
				return RightReplaceInput;
			}
		}
		public static bool LeftGrabInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && leftTouchController != null && leftTouchController.grip.ReadValue() > Settings.defaultDeadzoneMin)
					return true;
				// else if (Keyboard.current.qKey.isPressed)
				else if (Keyboard.current.leftCtrlKey.isPressed)
					return true;
				else
					return false;
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
				if (_InputDevice == InputDevice.OculusRift && rightTouchController != null && rightTouchController.grip.ReadValue() > Settings.defaultDeadzoneMin)
					return true;
				// else if (Keyboard.current.eKey.isPressed)
				else if (Keyboard.current.spaceKey.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _RightGrabInput
		{
			get
			{
				return RightGrabInput;
			}
		}
		public static bool LeftRotateInput
		{
			get
			{
				if (Keyboard.current.zKey.isPressed)
				// if (Keyboard.current.qKey.isPressed)
				// if (Keyboard.current.cKey.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _LeftRotateInput
		{
			get
			{
				return LeftRotateInput;
			}
		}
		public static bool RightRotateInput
		{
			get
			{
				if (Keyboard.current.cKey.isPressed)
				// if (Keyboard.current.eKey.isPressed)
				// if (Keyboard.current.vKey.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _RightRotateInput
		{
			get
			{
				return RightRotateInput;
			}
		}
		public static bool LeftThrowInput
		{
			get
			{
				// if (Keyboard.current.zKey.isPressed)
				if (Keyboard.current.qKey.isPressed)
				// if (Keyboard.current.cKey.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _LeftThrowInput
		{
			get
			{
				return LeftThrowInput;
			}
		}
		public static bool RightThrowInput
		{
			get
			{
				// if (Keyboard.current.cKey.isPressed)
				if (Keyboard.current.eKey.isPressed)
				// if (Keyboard.current.vKey.isPressed)
					return true;
				else
					return false;
			}
		}
		public bool _RightThrowInput
		{
			get
			{
				return RightThrowInput;
			}
		}
		public static bool TurnInput
		{
			get
			{
				return _InputDevice == InputDevice.OculusRift && rightTouchController != null && rightTouchController.primaryButton.isPressed;
			}
		}
		public bool _TurnInput
		{
			get
			{
				return TurnInput;
			}
		}
		public static bool LeftOrbViewInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && leftTouchController != null && leftTouchController.secondaryButton.isPressed)
					return true;
				else if (Keyboard.current.digit1Key.isPressed)
					return true;
				return false;
			}
		}
		public bool _LeftOrbViewInput
		{
			get
			{
				return LeftOrbViewInput;
			}
		}
		public static bool RightOrbViewInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && rightTouchController != null && rightTouchController.secondaryButton.isPressed)
					return true;
				else if (Keyboard.current.digit3Key.isPressed)
					return true;
				return false;
			}
		}
		public bool _RightOrbViewInput
		{
			get
			{
				return RightOrbViewInput;
			}
		}
		public static float MouseScrollWheelInput
		{
			get
			{
				if (UsingMouse)
					return Mouse.current.scroll.y.ReadValue();
				else
					return 0;
			}
		}
		public float _MouseScrollWheelInput
		{
			get
			{
				return MouseScrollWheelInput;
			}
		}
		public static bool JumpInput
		{
			get
			{
				if (_InputDevice == InputDevice.OculusRift && ((leftTouchController != null && leftTouchController.thumbstickClicked.isPressed) || (rightTouchController != null && rightTouchController.thumbstickClicked.isPressed)))
					return true;
				else if (Keyboard.current.leftShiftKey.isPressed)
					return true;
				else
					return false;
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
			OculusRift,
			KeyboardAndMouse
		}
	}
}