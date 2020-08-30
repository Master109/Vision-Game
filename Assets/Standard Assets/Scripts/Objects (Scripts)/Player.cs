using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;

namespace VisionGame
{
	[ExecuteInEditMode]
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
		public Vector2 rotateRate;
		public float rollRate;
		public bool invulnerable;
		[SerializeField]
		[HideInInspector]
		Vector3 initLeftHandLocalPosition;
		[SerializeField]
		[HideInInspector]
		Vector3 initRightHandLocalPosition;
		Vector3 previousMoveInput;
		Vector3 previousLeftHandPosition;
		Vector3 previousRightHandPosition;
		Vector3 previousLeftHandEulerAngles;
		Vector3 previousRightHandEulerAngles;			
		PhysicsObject leftGrabbedPhysicsObject;
		PhysicsObject rightGrabbedPhysicsObject;
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
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				initLeftHandLocalPosition = leftHandTrs.localPosition;
				initRightHandLocalPosition = rightHandTrs.localPosition;
				return;
			}
#endif
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			InputManager.leftTouchController = (OculusTouchController) OculusTouchController.leftHand;
			if (InputManager.leftTouchController != null)
			{
				leftHandTrs.SetParent(trs);
				leftHandTrs.localPosition = InputManager.leftTouchController.devicePosition.ReadValue();
				leftHandTrs.localRotation = InputManager.leftTouchController.deviceRotation.ReadValue();
			}
			else
			{
				leftHandTrs.SetParent(GameManager.GetSingleton<GameCamera>().trs);
				leftHandTrs.localPosition = initLeftHandLocalPosition;
				leftHandTrs.localRotation = Quaternion.identity;
			}
			InputManager.rightTouchController = (OculusTouchController) OculusTouchController.rightHand;
			if (InputManager.rightTouchController != null)
			{
				rightHandTrs.SetParent(trs);
				rightHandTrs.localPosition = InputManager.rightTouchController.devicePosition.ReadValue();
				rightHandTrs.localRotation = InputManager.rightTouchController.deviceRotation.ReadValue();
			}
			else
			{
				rightHandTrs.SetParent(GameManager.GetSingleton<GameCamera>().trs);
				rightHandTrs.localPosition = initRightHandLocalPosition;
				rightHandTrs.localRotation = Quaternion.identity;
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
			HandleOrbGrabbing (InputManager.LeftGrabInput, leftHandTrs, ref leftGrabbedPhysicsObject, rightGrabbedPhysicsObject, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleOrbGrabbing (InputManager.RightGrabInput, rightHandTrs, ref rightGrabbedPhysicsObject, leftGrabbedPhysicsObject, previousRightHandPosition, previousRightHandEulerAngles);
			HandleOrbRotating (InputManager.LeftRotateInput, leftGrabbedPhysicsObject);
			HandleOrbRotating (InputManager.RightRotateInput, rightGrabbedPhysicsObject);
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

		void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (!invulnerable)
				GameManager.GetSingleton<GameManager>().ReloadActiveScene ();
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

		void HandleOrbGrabbing (bool grabInput, Transform grabbingHand, ref PhysicsObject grabbedPhysicsObject, PhysicsObject otherPhysicsObject, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (grabInput)
			{
				if (grabbedPhysicsObject == null)
				{
					for (int i = 0; i < GameManager.GetSingleton<Level>().orbs.Length; i ++)
					{
						Orb orb = GameManager.GetSingleton<Level>().orbs[i];
						if (!orb.Equals(grabbedPhysicsObject) && !orb.Equals(otherPhysicsObject) && (orb.trs.position - grabbingHand.position).sqrMagnitude <= grabRangeSqr)
						{
							orb.trs.SetParent(grabbingHand);
							orb.trs.localPosition = Vector3.zero;
							grabbedPhysicsObject = orb;
							grabbedPhysicsObject.rigid.isKinematic = true;
							if (GameManager.GetSingleton<Level>().orbs.Length == 2)
							{
								if (grabbingHand == leftHandTrs)
								{
									GameManager.GetSingleton<Level>().leftOrb = orb;
									GameManager.GetSingleton<Level>().rightOrb = GameManager.GetSingleton<Level>().orbs[1 - i];
								}
								else
								{
									GameManager.GetSingleton<Level>().leftOrb = GameManager.GetSingleton<Level>().orbs[1 - i];
									GameManager.GetSingleton<Level>().rightOrb = orb;
								}
							}
						}
					}
				}
				else
				{
					grabbedPhysicsObject.rigid.isKinematic = true;
					grabbedPhysicsObject.velocity = Vector3.zero;
					grabbedPhysicsObject.angularVelocity = Vector3.zero;
				}
			}
			else if (grabbedPhysicsObject != null)
			{
				grabbedPhysicsObject.trs.SetParent(null);
				grabbedPhysicsObject.rigid.isKinematic = false;
				grabbedPhysicsObject.rigid.velocity = (grabbingHand.position - previousHandPosition) * Time.deltaTime;
				grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), grabbingHand.rotation);
				grabbedPhysicsObject = null;
			}
		}

		void HandleOrbRotating (bool rotateInput, PhysicsObject grabbedPhysicsObject)
		{
			if (grabbedPhysicsObject == null)
				return;
			if (rotateInput)
			{
				Vector2 mouseMovement = InputManager.MouseMovement;
				grabbedPhysicsObject.trs.localEulerAngles += new Vector3(-mouseMovement.y * rotateRate.x, mouseMovement.x * rotateRate.y, 0);
				grabbedPhysicsObject.trs.RotateAround(grabbedPhysicsObject.trs.position, grabbedPhysicsObject.trs.forward, Mouse.current.scroll.y.ReadValue() * rollRate);
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
					if (slopeAngle <= controller.slopeLimit)
					{
						controller.enabled = true;
						rigid.useGravity = false;
						return;
					}
					else if (Mathf.Approximately(slopeAngle, 90))
						return;
				}
			}
		}
		
		void Jump ()
		{
			yVel += jumpSpeed * Time.deltaTime;
			move += Vector3.up * yVel;
		}

		void OnCollisionEnter (Collision coll)
		{
			goCollisions.Remove(coll.gameObject);
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