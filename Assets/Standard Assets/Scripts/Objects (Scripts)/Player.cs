using UnityEngine;
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
		Vector3 previousMoveInput;
		Vector3 previousLeftHandPosition;
		Vector3 previousRightHandPosition;
		Vector3 previousLeftHandEulerAngles;
		Vector3 previousRightHandEulerAngles;
		Rigidbody leftGrabbedRigid;
		Rigidbody rightGrabbedRigid;
		bool leftReplaceInput;
		bool previousLeftReplaceInput;
		bool rightReplaceInput;
		bool previousRightReplaceInput;
		Quaternion previousRotation;
		float yVel;
		Vector3 previousPosition;
		bool canJump;
		float timeLastGrounded;
		bool turnInput;
		bool previousTurnInput;
		Dictionary<GameObject, Collision> goCollisions = new Dictionary<GameObject, Collision>();

		void OnEnable ()
		{
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			InputManager.leftTouchController = (OculusTouchController) OculusTouchController.leftHand;
			if (InputManager.leftTouchController != null)
			{
				leftHandTrs.localPosition = InputManager.leftTouchController.devicePosition.ReadValue();
				leftHandTrs.localRotation = InputManager.leftTouchController.deviceRotation.ReadValue();
			}
			InputManager.rightTouchController = (OculusTouchController) OculusTouchController.rightHand;
			if (InputManager.rightTouchController != null)
			{
				rightHandTrs.localPosition = InputManager.rightTouchController.devicePosition.ReadValue();
				rightHandTrs.localRotation = InputManager.rightTouchController.deviceRotation.ReadValue();
			}
			leftReplaceInput = InputManager.LeftReplaceInput;
			rightReplaceInput = InputManager.RightReplaceInput;
			turnInput = InputManager.TurnInput;
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
			HandleReplacement ();
			controller.Move(move * Time.deltaTime);
			previousLeftReplaceInput = leftReplaceInput;
			previousRightReplaceInput = rightReplaceInput;
			previousTurnInput = turnInput;
			previousLeftHandPosition = leftHandTrs.position;
			previousRightHandPosition = rightHandTrs.position;
			previousLeftHandEulerAngles = leftHandTrs.eulerAngles;
			previousRightHandEulerAngles = rightHandTrs.eulerAngles;
			previousMoveInput = move;
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void HandleReplacement ()
		{
			if (leftReplaceInput && !previousLeftReplaceInput)
				GameManager.GetSingleton<Level>().leftOrb.ReplaceObjects ();
			if (rightReplaceInput && !previousRightReplaceInput)
				GameManager.GetSingleton<Level>().rightOrb.ReplaceObjects ();
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
                for (int i = 0; i < GameManager.GetSingleton<Level>().orbs.Length; i ++)
				{
                    Orb orb = GameManager.GetSingleton<Level>().orbs[i];
                    if (rightGrabbedRigid != orb.rigid && (orb.trs.position - leftHandTrs.position).sqrMagnitude <= grabRangeSqr)
					{
						orb.trs.SetParent(leftHandTrs);
						orb.trs.localPosition = Vector3.zero;
						leftGrabbedRigid = orb.rigid;
						if (GameManager.GetSingleton<Level>().orbs.Length == 2)
						{
							GameManager.GetSingleton<Level>().leftOrb = orb;
							GameManager.GetSingleton<Level>().rightOrb = GameManager.GetSingleton<Level>().orbs[1 - i];
						}
					}
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
				for (int i = 0; i < GameManager.GetSingleton<Level>().orbs.Length; i ++)
				{
                    Orb orb = GameManager.GetSingleton<Level>().orbs[i];
					if (leftGrabbedRigid != orb.rigid && (orb.trs.position - rightHandTrs.position).sqrMagnitude <= grabRangeSqr)
					{
						orb.trs.SetParent(rightHandTrs);
						orb.trs.localPosition = Vector3.zero;
						rightGrabbedRigid = orb.rigid;
						if (GameManager.GetSingleton<Level>().orbs.Length == 2)
						{
							GameManager.GetSingleton<Level>().leftOrb = GameManager.GetSingleton<Level>().orbs[1 - i];
							GameManager.GetSingleton<Level>().rightOrb = orb;
						}
					}
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
			if (InputManager.LeftOrbViewInput)
			{
				GameManager.GetSingleton<Level>().leftOrb.camera.depth = 1;
				return;
			}
			else
				GameManager.GetSingleton<Level>().leftOrb.camera.depth = -1;
			if (InputManager.RightOrbViewInput)
				GameManager.GetSingleton<Level>().rightOrb.camera.depth = 1;
			else
				GameManager.GetSingleton<Level>().rightOrb.camera.depth = -1;
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
			if (InputManager.leftTouchController != null && InputManager.rightTouchController != null)
				moveInput = InputManager.leftTouchController.thumbstick.ToVec2() + InputManager.rightTouchController.thumbstick.ToVec2();
			else
				moveInput = InputManager.GetAxis2D(Keyboard.current.dKey, Keyboard.current.aKey, Keyboard.current.wKey, Keyboard.current.sKey);
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
			else
				move = previousMoveInput.SetY(0);
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

		void HandleCollisions ()
		{
			foreach (Collision coll in goCollisions.Values)
			{
				for (int i = 0; i < coll.contactCount; i ++)
				{
					ContactPoint contactPoint = coll.GetContact(i);
					float slopeAngle = Vector3.Angle(contactPoint.normal, Vector3.up);
					if (slopeAngle <= controller.slopeLimit || slopeAngle >= 90)
					{
						controller.enabled = true;
						rigid.useGravity = false;
						return;
					}
				}
				controller.enabled = false;
				rigid.useGravity = true;
			}
		}
		
		void Jump ()
		{
			yVel += jumpSpeed * Time.deltaTime;
			move += Vector3.up * yVel;
		}

		void OnCollisionEnter (Collision coll)
		{
			goCollisions.Add(coll.gameObject, coll);
			HandleCollisions ();
		}

		void OnCollisionStay (Collision coll)
		{
			goCollisions[coll.gameObject] = coll;
			HandleCollisions ();
		}

		void OnCollisionExit (Collision coll)
		{
			goCollisions.Remove(coll.gameObject);
		}
	}
}