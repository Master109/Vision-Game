﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;
#if UNITY_2017_2_OR_NEWER
using InputTracking = UnityEngine.XR.InputTracking;
using Node = UnityEngine.XR.XRNode;
#else
using InputTracking = UnityEngine.VR.InputTracking;
using Node = UnityEngine.VR.VRNode;
#endif

namespace VisionGame
{
	public class Player : MonoBehaviour, IUpdatable
	{
		public bool PauseWhileUnfocused
		{
			get
			{
				return false;
			}
		}
		public Transform trs;
		public Transform leftHandTrs;
		public Transform rightHandTrs;
		public CharacterController controller;
		public float jumpSpeed;
		public float jumpDuration;
		public float gravity;
		[HideInInspector]
		public Vector3 move;
		public float moveSpeed;
		public Rigidbody rigid;
		public float grabRangeSqr;
		Vector3 previousLeftHandPosition;
		Vector3 previousRightHandPosition;
		Vector3 previousLeftHandEulerAngles;
		Vector3 previousRightHandEulerAngles;
		Rigidbody leftGrabbedRigid;
		Rigidbody rightGrabbedRigid;
		bool replaceInput;
		bool previousReplaceInput;
		Quaternion previousRotation;
		float yVel;
		Vector3 previousPosition;
		bool canJump;
		float timeLastGrounded;
		bool turnInput;
		bool previousTurnInput;

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			InputManager.leftTouchController = (OculusTouchController) OculusTouchController.leftHand;
			leftHandTrs.position = InputManager.leftTouchController.devicePosition.ReadValue();
			leftHandTrs.rotation = InputManager.leftTouchController.deviceRotation.ReadValue();
			InputManager.rightTouchController = (OculusTouchController) OculusTouchController.rightHand;
			rightHandTrs.position = InputManager.rightTouchController.devicePosition.ReadValue();
			rightHandTrs.rotation = InputManager.rightTouchController.deviceRotation.ReadValue();
			replaceInput = InputManager.ReplaceInput;
			turnInput = InputManager.TurnInput;
			if (replaceInput && !previousReplaceInput)
				GameManager.GetSingleton<Orb>().ReplaceObjects ();
			move = Vector3.zero;
			if (controller.isGrounded)
			{
				timeLastGrounded = Time.time;
				canJump = true;
			}
			HandleFacing ();
			HandleGrabbing ();
			Move ();
			HandleGravity ();
			HandleJump ();
			controller.Move(move * Time.deltaTime);
			previousReplaceInput = replaceInput;
			previousTurnInput = turnInput;
			previousLeftHandPosition = leftHandTrs.position;
			previousRightHandPosition = rightHandTrs.position;
			previousLeftHandEulerAngles = leftHandTrs.eulerAngles;
			previousRightHandEulerAngles = rightHandTrs.eulerAngles;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void HandleFacing ()
		{
			if (turnInput && !previousTurnInput)
				trs.forward = GameManager.GetSingleton<GameCamera>().trs.forward.SetY(0);
		}

		void HandleGrabbing ()
		{
			if (InputManager.LeftGrabInput && leftGrabbedRigid == null)
			{
				if (rightGrabbedRigid != GameManager.GetSingleton<Orb>().rigid && (GameManager.GetSingleton<Orb>().trs.position - leftHandTrs.position).sqrMagnitude <= grabRangeSqr)
				{
					GameManager.GetSingleton<Orb>().trs.SetParent(leftHandTrs);
					GameManager.GetSingleton<Orb>().trs.localPosition = Vector3.zero;
					leftGrabbedRigid = GameManager.GetSingleton<Orb>().rigid;
				}
			}
			else if (leftGrabbedRigid != null)
			{
				leftGrabbedRigid.velocity = (leftHandTrs.position - previousLeftHandPosition) * Time.deltaTime;
				leftGrabbedRigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousLeftHandEulerAngles), leftHandTrs.rotation);
				leftGrabbedRigid = null;
			}
			if (InputManager.RightGrabInput && rightGrabbedRigid == null)
			{
				if (leftGrabbedRigid != GameManager.GetSingleton<Orb>().rigid && (GameManager.GetSingleton<Orb>().trs.position - rightHandTrs.position).sqrMagnitude <= grabRangeSqr)
				{
					GameManager.GetSingleton<Orb>().trs.SetParent(rightHandTrs);
					GameManager.GetSingleton<Orb>().trs.localPosition = Vector3.zero;
					rightGrabbedRigid = GameManager.GetSingleton<Orb>().rigid;
				}
			}
			else if (rightGrabbedRigid != null)
			{
				rightGrabbedRigid.velocity = (rightHandTrs.position - previousRightHandPosition) * Time.deltaTime;
				rightGrabbedRigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousRightHandEulerAngles), rightHandTrs.rotation);
				rightGrabbedRigid = null;
			}
		}

		void HandleOrbViewing ()
		{
			if (InputManager.OrbViewInput)
				GameManager.GetSingleton<Orb>().camera.depth = 1;
			else
				GameManager.GetSingleton<Orb>().camera.depth = -1;
		}
		
		void HandleGravity ()
		{
			if (!controller.isGrounded)
			{
				yVel -= gravity * Time.deltaTime;
				move += Vector3.up * yVel;
			}
		}
	
		Vector3 GetMoveInput ()
		{
			Vector3 moveInput = new Vector3();
			if (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.KeyboardAndMouse)
				moveInput = InputManager.GetAxis2D(Keyboard.current.dKey, Keyboard.current.aKey, Keyboard.current.wKey, Keyboard.current.sKey);
			else if (GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.OculusRift)
				moveInput = InputManager.leftTouchController.thumbstick.ToVec2() + InputManager.rightTouchController.thumbstick.ToVec2();
			if (moveInput != Vector3.zero)
				moveInput = moveInput.XYToXZ();
			moveInput = Vector3.ClampMagnitude(moveInput, 1);
			moveInput = Quaternion.Euler(Vector3.up * GameManager.GetSingleton<GameCamera>().trs.eulerAngles.y) * moveInput;
			moveInput.y = 0;
			return moveInput;
		}
		
		void Move ()
		{
			if (controller.enabled && controller.isGrounded)
				move += GetMoveInput() * moveSpeed;
		}
		
		void HandleJump ()
		{
			if (canJump && InputManager.JumpInput && Time.time - timeLastGrounded < jumpDuration)
			{
				if (controller.isGrounded)
					yVel = 0;
				Jump ();
			}
			else
			{
				if (yVel > 0)
					yVel = 0;
				canJump = false;
			}
		}
		
		void Jump ()
		{
			yVel += jumpSpeed * Time.deltaTime;
			move += Vector3.up * yVel;
		}

		void OnCollisionStay (Collision coll)
		{
			bool onSteepSlope = true;
			for (int i = 0; i < coll.contactCount; i ++)
			{
				ContactPoint contactPoint = coll.GetContact(i);
				float slopeAngle = Vector3.Angle(contactPoint.normal, Vector3.up);
				if (slopeAngle <= controller.slopeLimit || slopeAngle >= 90)
				{
					onSteepSlope = false;
					break;
				}
			}
			controller.enabled = !onSteepSlope;
			rigid.useGravity = onSteepSlope;
		}
	}
}