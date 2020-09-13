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
		public Rigidbody rigid;
		public new Collider collider;
		public Transform leftHandTrs;
		public Transform rightHandTrs;
		public CharacterController controller;
		public float jumpSpeed;
		public float jumpDuration;
		public float gravity;
		[HideInInspector]
		public Vector3 move;
		public float moveSpeed;
		public SphereCollider leftHandSphereCollider;
		public SphereCollider rightHandSphereCollider;
		public LayerMask whatIsGrabbable;
		public Vector2 rotateRate;
		public float rollRate;
		public bool invulnerable;
		public float maxHandDistance;
		public LineRenderer leftAimer;
		public LineRenderer rightAimer;
		public float maxAimerDistanceSqr;
		public LayerMask whatBlocksAimer;
		public FloatRange throwSpeedRange;
		public float throwSpeedChangeRate;
		public LayerMask whatICollideWith;
		public float groundCheckDistance;
		public float extraVelocityDrag;
		public float timeTillApplyExtraVelocityDrag;
		float timeExtraVelocityWasSet;
		Vector3 extraVelocity;
		[SerializeField]
		[HideInInspector]
		Vector3 initLeftHandLocalPosition;
		[SerializeField]
		[HideInInspector]
		Vector3 initRightHandLocalPosition;
		float currentThrowSpeed;
		Vector3 previousMoveInput;
		Vector3 leftHandPosition;
		Vector3 rightHandPosition;
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
		bool leftThrowInput;
		bool previousLeftThrowInput;
		bool rightThrowInput;
		bool previousRightThrowInput;
		bool leftGrabInput;
		bool previousLeftGrabInput;
		bool rightGrabInput;
		bool previousRightGrabInput;
		Quaternion previousRotation;
		Vector3 previousPosition;
		float timeLastGrounded;
		bool turnInput;
		bool previousTurnInput;
		List<PhysicsObject> physicsObjectsTouchingLeftHand = new List<PhysicsObject>();
		List<PhysicsObject> physicsObjectsTouchingRightHand = new List<PhysicsObject>();
		bool leftCanThrow;
		bool rightCanThrow;
		float mouseScrollWheelInput;
		bool jumpInput;
		bool previousJumpInput;
		bool isGrounded;

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
			currentThrowSpeed = throwSpeedRange.max;
			Application.wantsToQuit += () => { invulnerable = true; return true; };
			GameManager.updatables = GameManager.updatables.Add(this);
		}

		public void DoUpdate ()
		{
			mouseScrollWheelInput = InputManager.MouseScrollWheelInput;
			InputManager.leftTouchController = (OculusTouchController) OculusTouchController.leftHand;
			InputManager.rightTouchController = (OculusTouchController) OculusTouchController.rightHand;
			leftHandPosition = leftHandTrs.position;
			rightHandPosition = rightHandTrs.position;
			HandleHandOrientation ();
			turnInput = InputManager.TurnInput;
			HandleFacing ();
			leftGrabInput = InputManager.LeftGrabInput;
			rightGrabInput = InputManager.RightGrabInput;
			leftThrowInput = InputManager.LeftThrowInput;
			rightThrowInput = InputManager.RightThrowInput;
			HandleGrabbing ();
			HandleAiming ();
			HandleThrowing ();
			HandleRotatingGrabbedObjects ();
			HandleOrbViewing ();
			leftReplaceInput = InputManager.LeftReplaceInput;
			rightReplaceInput = InputManager.RightReplaceInput;
			HandleReplacement ();
			jumpInput = InputManager.JumpInput;
			HandleVelocity ();
			previousLeftReplaceInput = leftReplaceInput;
			previousRightReplaceInput = rightReplaceInput;
			previousTurnInput = turnInput;
			previousLeftHandPosition = leftHandPosition;
			previousRightHandPosition = rightHandPosition;
			previousLeftHandEulerAngles = leftHandTrs.eulerAngles;
			previousRightHandEulerAngles = rightHandTrs.eulerAngles;
			previousMoveInput = move;
			previousLeftThrowInput = leftThrowInput;
			previousRightThrowInput = rightThrowInput;
			previousLeftGrabInput = leftGrabInput;
			previousRightGrabInput = rightGrabInput;
			previousJumpInput = jumpInput;
		}

		void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void HandleVelocity ()
		{
			move.x = 0;
			move.z = 0;
			isGrounded = controller.isGrounded;
			if (!isGrounded)
			{
				RaycastHit hit;
				if (Physics.Raycast(trs.position, Vector3.down, out hit, groundCheckDistance, whatICollideWith))
				{
					isGrounded = true;
					if (hit.rigidbody != null)
						HandleBeingPushed (hit.rigidbody);
				}
			}
			if (isGrounded)
				timeLastGrounded = Time.time;
			Move ();
			HandleGravity ();
			HandleJump ();
			if (controller.enabled)
			{
				rigid.velocity = Vector3.zero;
				controller.Move((move + extraVelocity) * Time.deltaTime);
			}
			else
				rigid.velocity = move + extraVelocity;
			if (Time.time - timeExtraVelocityWasSet >= timeTillApplyExtraVelocityDrag)
				extraVelocity *= (1f - Time.deltaTime * extraVelocityDrag);
		}

		void HandleHandOrientation ()
		{
			HandleHandOrientation (leftHandTrs, InputManager.leftTouchController, initLeftHandLocalPosition);
			HandleHandOrientation (rightHandTrs, InputManager.rightTouchController, initRightHandLocalPosition);
		}

		void HandleHandOrientation (Transform handTrs, OculusTouchController oculusTouchController, Vector3 initLocalPosition)
		{
			if (oculusTouchController != null)
			{
				handTrs.SetParent(trs);
				handTrs.localPosition = Vector3.ClampMagnitude(oculusTouchController.devicePosition.ReadValue(), maxHandDistance);
				handTrs.localRotation = oculusTouchController.deviceRotation.ReadValue();
			}
			else
			{
				handTrs.SetParent(GameManager.GetSingleton<GameCamera>().trs);
				handTrs.localPosition = initLocalPosition;
				handTrs.localRotation = Quaternion.identity;
			}
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
			HandleGrabbing (leftGrabInput, previousLeftGrabInput, leftThrowInput, leftHandTrs, physicsObjectsTouchingLeftHand.ToArray(), ref leftGrabbedPhysicsObject, rightGrabbedPhysicsObject, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleGrabbing (rightGrabInput, previousRightGrabInput, rightThrowInput, rightHandTrs, physicsObjectsTouchingRightHand.ToArray(), ref rightGrabbedPhysicsObject, leftGrabbedPhysicsObject, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleGrabbing (bool grabInput, bool previousGrabInput, bool throwInput, Transform handTrs, PhysicsObject[] touchingPhysicsObjects, ref PhysicsObject grabbedPhysicsObject, PhysicsObject otherGrabbedPhysicsObject, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (grabInput)
			{
				if (!previousGrabInput && !throwInput && grabbedPhysicsObject == null)
				{
					for (int i = 0; i < touchingPhysicsObjects.Length; i ++)
					{
						PhysicsObject physicsObject = touchingPhysicsObjects[i];
						if (!physicsObject.Equals(grabbedPhysicsObject) && !physicsObject.Equals(otherGrabbedPhysicsObject))
						{
							// Physics.IgnoreCollision(physicsObject.collider, collider, true);
							// Physics.IgnoreCollision(physicsObject.collider, controller, true);
							physicsObject.trs.SetParent(handTrs);
							grabbedPhysicsObject = physicsObject;
							physicsObject.trs.position = GetGrabPosition(grabbedPhysicsObject);
							grabbedPhysicsObject.rigid.isKinematic = true;
							Orb orb = physicsObject.GetComponent<Orb>();
							if (orb != null && GameManager.GetSingleton<Level>().orbs.Length == 2)
							{
								if ((orb == GameManager.GetSingleton<Level>().orbs[0]) == (handTrs == leftHandTrs))
								{
									GameManager.GetSingleton<Level>().leftOrb = orb;
									GameManager.GetSingleton<Level>().rightOrb = GameManager.GetSingleton<Level>().orbs[1];
								}
								else
								{
									GameManager.GetSingleton<Level>().leftOrb = GameManager.GetSingleton<Level>().orbs[1];
									GameManager.GetSingleton<Level>().rightOrb = orb;
								}
							}
						}
					}
				}
			}
			else if (grabbedPhysicsObject != null)
			{
				grabbedPhysicsObject.trs.SetParent(null);
				grabbedPhysicsObject.rigid.isKinematic = false;
				grabbedPhysicsObject.rigid.velocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
				grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
				// Physics.IgnoreCollision(grabbedPhysicsObject.collider, collider, false);
				// Physics.IgnoreCollision(grabbedPhysicsObject.collider, controller, false);
				grabbedPhysicsObject = null;
			}
		}

		Vector3 GetGrabPosition (PhysicsObject physicsObject)
		{
			Vector3 grabPosition = physicsObject.trs.position;
			Vector3 closestPointOnMe = collider.ClosestPoint(grabPosition);
			Vector3 previousGrabPosition = grabPosition;
			if (Physics.ClosestPoint(closestPointOnMe, physicsObject.collider, grabPosition, physicsObject.trs.rotation) != closestPointOnMe)
			{
				do
				{
					previousGrabPosition = grabPosition;
					grabPosition += (closestPointOnMe - grabPosition).normalized * Physics.defaultContactOffset;
				} while (Physics.ClosestPoint(closestPointOnMe, physicsObject.collider, grabPosition, physicsObject.trs.rotation) != closestPointOnMe);
				grabPosition = previousGrabPosition;
			}
			else
			{
				do
				{
					grabPosition -= (closestPointOnMe - grabPosition).normalized * Physics.defaultContactOffset;
				} while (Physics.ClosestPoint(closestPointOnMe, physicsObject.collider, grabPosition, physicsObject.trs.rotation) == closestPointOnMe);
			}
			return grabPosition;
		}

		void HandleAiming ()
		{
			HandleAiming (leftThrowInput, previousLeftThrowInput, ref leftCanThrow, leftHandTrs, leftGrabbedPhysicsObject, leftAimer, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleAiming (rightThrowInput, previousRightThrowInput, ref rightCanThrow, rightHandTrs, rightGrabbedPhysicsObject, rightAimer, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleAiming (bool throwInput, bool previousThrowInput, ref bool canThrow, Transform handTrs, PhysicsObject grabbedPhysicsObject, LineRenderer aimer, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (!canThrow)
			{
				if (throwInput && !previousThrowInput)
				{
					canThrow = true;
				}
				else
				{
					aimer.enabled = false;
					return;
				}
			}
			if (throwInput && grabbedPhysicsObject != null)
			{
				currentThrowSpeed = Mathf.Clamp(currentThrowSpeed + mouseScrollWheelInput * throwSpeedChangeRate * Time.deltaTime, throwSpeedRange.min, throwSpeedRange.max);
				Vector3 currentVelocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
				if (InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
					currentVelocity += handTrs.forward * currentThrowSpeed;
				float currentDrag = grabbedPhysicsObject.rigid.drag;
				Vector3 currentPosition = handTrs.position;
				List<Vector3> positions = new List<Vector3>();
				positions.Add(currentPosition);
				Vector3 previousPosition = currentPosition;
				do
				{
					currentVelocity += Physics.gravity * Time.fixedDeltaTime;
					currentVelocity *= (1f - Time.fixedDeltaTime * currentDrag);
					currentPosition += currentVelocity * Time.fixedDeltaTime;
					RaycastHit hit;
					if (Physics.CapsuleCast(previousPosition, currentPosition, aimer.startWidth, currentPosition - previousPosition, out hit, 0, whatBlocksAimer) && hit.rigidbody != grabbedPhysicsObject.rigid)
					{
						positions.Add(hit.point);
						break;
					}
					if (positions.Contains(currentPosition))
						break;
					positions.Add(currentPosition);
					previousPosition = currentPosition;
				} while ((handTrs.position - currentPosition).sqrMagnitude < maxAimerDistanceSqr);
				aimer.positionCount = positions.Count;
				aimer.SetPositions(positions.ToArray());
				if (!previousThrowInput)
					aimer.enabled = true;
			}
			else
				aimer.enabled = false;
		}

		void HandleThrowing ()
		{
			HandleThrowing (leftThrowInput, previousLeftThrowInput, leftCanThrow, leftHandTrs, ref leftGrabbedPhysicsObject, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleThrowing (rightThrowInput, previousRightThrowInput, rightCanThrow, rightHandTrs, ref rightGrabbedPhysicsObject, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleThrowing (bool throwInput, bool previousThrowInput, bool canThrow, Transform handTrs, ref PhysicsObject grabbedPhysicsObject, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (!canThrow)
				return;
			if (!throwInput && previousThrowInput && grabbedPhysicsObject != null)
			{
				grabbedPhysicsObject.trs.SetParent(null);
				grabbedPhysicsObject.rigid.isKinematic = false;
				Vector3 throwVelocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
				if (InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
					throwVelocity += handTrs.forward * currentThrowSpeed;
				grabbedPhysicsObject.rigid.velocity = throwVelocity;
				grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
				Physics.IgnoreCollision(grabbedPhysicsObject.collider, collider, false);
				Physics.IgnoreCollision(grabbedPhysicsObject.collider, controller, false);
				grabbedPhysicsObject = null;
			}
		}

		void HandleRotatingGrabbedObjects ()
		{
			HandleRotatingGrabbedObject (InputManager.LeftRotateInput, ref leftCanThrow, leftGrabbedPhysicsObject);
			HandleRotatingGrabbedObject (InputManager.RightRotateInput, ref rightCanThrow, rightGrabbedPhysicsObject);
		}

		void HandleRotatingGrabbedObject (bool rotateInput, ref bool canThrow, PhysicsObject grabbedPhysicsObject)
		{
			bool _canThrow = canThrow;
			if (grabbedPhysicsObject == null)
			{
				canThrow = true;
				return;
			}
			if (rotateInput)
			{
				_canThrow = false;
				Vector2 mouseMovement = InputManager.MouseMovement;
				grabbedPhysicsObject.trs.localEulerAngles += new Vector3(-mouseMovement.y * rotateRate.x, mouseMovement.x * rotateRate.y) * Time.deltaTime;
				grabbedPhysicsObject.trs.RotateAround(grabbedPhysicsObject.trs.position, grabbedPhysicsObject.trs.forward, mouseScrollWheelInput * rollRate * Time.deltaTime);
			}
			canThrow = _canThrow;
		}

		void HandleOrbViewing ()
		{
			if (InputManager.LeftOrbViewInput)
			{
				// GameManager.GetSingleton<Level>().leftOrb.camera.depth = 1;
				GameManager.GetSingleton<Level>().leftOrb.camera.enabled = true;
				return;
			}
			else
				// GameManager.GetSingleton<Level>().leftOrb.camera.depth = -1;
				GameManager.GetSingleton<Level>().leftOrb.camera.enabled = false;
			if (InputManager.RightOrbViewInput)
				// GameManager.GetSingleton<Level>().rightOrb.camera.depth = 1;
				GameManager.GetSingleton<Level>().rightOrb.camera.enabled = true;
			else
				// GameManager.GetSingleton<Level>().rightOrb.camera.depth = -1;
				GameManager.GetSingleton<Level>().rightOrb.camera.enabled = false;
		}
		
		void HandleGravity ()
		{
			if (!isGrounded)
				move.y -= gravity * Time.deltaTime;
			else
			{
				if (controller.enabled)
					move.y = controller.velocity.y;
				else
					move.y = rigid.velocity.y;
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
			if (isGrounded)
				move += GetMoveInput() * moveSpeed;
			else
				move = previousMoveInput.SetY(move.y);
		}
		
		void HandleJump ()
		{
			if (jumpInput && Time.time - timeLastGrounded < jumpDuration)
			{
				if (!previousJumpInput)
				{
					if (controller.enabled)
						move.y = controller.velocity.y;
					else
						move.y = rigid.velocity.y;
				}
				Jump ();
			}
			else if (move.y > 0)
				move.y = 0;
		}

		void HandleSlopes ()
		{
			RaycastHit hit;
			if (Physics.Raycast(collider.bounds.center + Vector3.down * collider.bounds.extents.y, Vector3.down, out hit, 1, whatICollideWith))
			{
				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
				if (slopeAngle <= controller.slopeLimit)
				{
					controller.enabled = true;
					rigid.useGravity = false;
					rigid.velocity = Vector2.zero;
					return;
				}
				// else
				// 	return;
			}
			else
				return;
			if (controller.enabled)
				rigid.velocity = controller.velocity;
			controller.enabled = false;
			rigid.useGravity = true;
		}

		void HandleBeingPushed (Rigidbody rigid)
		{
			extraVelocity = rigid.velocity;
			timeExtraVelocityWasSet = Time.time;
		}
		
		void Jump ()
		{
			// if (!previousJumpInput)
				move.y = jumpSpeed;
		}

		void OnCollisionEnter (Collision coll)
		{
			if (coll.rigidbody != null)
				HandleBeingPushed (coll.rigidbody);
			HandleSlopes ();
		}

		void OnCollisionStay (Collision coll)
		{
			if (coll.rigidbody != null)
				HandleBeingPushed (coll.rigidbody);
			HandleSlopes ();
		}

		// void OnControllerColliderHit (ControllerColliderHit hit)
		// {
		// 	if (hit.rigidbody != null)
		// 		HandleBeingPushed (hit.rigidbody);
		// }

		void OnTriggerEnter (Collider other)
		{
			PhysicsObject physicsObject = other.GetComponentInParent<PhysicsObject>();
			if (physicsObject != null)
			{
				Collider[] hits = Physics.OverlapSphere(leftHandTrs.position, leftHandSphereCollider.radius, whatIsGrabbable);
				if (hits.Contains(physicsObject.collider))
				{
					if (!physicsObjectsTouchingLeftHand.Contains(physicsObject))
						physicsObjectsTouchingLeftHand.Add(physicsObject);
				}
				else if (!physicsObjectsTouchingRightHand.Contains(physicsObject))
					physicsObjectsTouchingRightHand.Add(physicsObject);
			}
		}

		void OnTriggerExit (Collider other)
		{
			PhysicsObject physicsObject = other.GetComponentInParent<PhysicsObject>();
			if (physicsObject != null)
			{
				Collider[] hits = Physics.OverlapSphere(leftHandTrs.position, leftHandSphereCollider.radius, whatIsGrabbable);
				if (hits.Contains(physicsObject.collider))
					physicsObjectsTouchingRightHand.Remove(physicsObject);
				else
				{
					physicsObjectsTouchingLeftHand.Remove(physicsObject);
					hits = Physics.OverlapSphere(rightHandTrs.position, rightHandSphereCollider.radius, whatIsGrabbable);
					if (!hits.Contains(physicsObject.collider))
						physicsObjectsTouchingRightHand.Remove(physicsObject);
				}
			}
		}
	}
}