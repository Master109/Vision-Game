﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.XR.Oculus.Input;
using UnityEngine.InputSystem;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class Player : SingletonUpdateWhileEnabled<Player>
	{
		public new static Player instance;
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
		public SphereCollider leftHandSphereSensorCollider;
		public SphereCollider rightHandSphereSensorCollider;
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
		public Vector2 headRotationSensitivity;
		public FloatRange headYRotationRange;
		public Transform headTrs;
		public float maxHeadDistance;
		public SphereCollider headSphereCollider;
		public float spikeCheckDistance;
		public float unignoreCollisionDelay;
		public float grabbedPhysicsObjectDropDistance;
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
		bool rightGrabInput;
		bool previousRightGrabInput;
		Quaternion previousRotation;
		Vector3 previousPosition;
		float timeLastGrounded;
		bool turnInput;
		bool previousTurnInput;
		bool leftGrabInput;
		bool previousLeftGrabInput;
		List<PhysicsObject> physicsObjectsTouchingLeftHand = new List<PhysicsObject>();
		List<PhysicsObject> physicsObjectsTouchingRightHand = new List<PhysicsObject>();
		bool leftCanThrow;
		bool rightCanThrow;
		float mouseScrollWheelInput;
		bool jumpInput;
		bool previousJumpInput;
		bool isGrounded;
		float headRotationY;
		Vector2 mouseMovement;

		void Awake ()
		{
			Application.wantsToQuit += () => { invulnerable = true; return true; };
		}

		public override void OnEnable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				initLeftHandLocalPosition = leftHandTrs.localPosition;
				initRightHandLocalPosition = rightHandTrs.localPosition;
				return;
			}
#endif
			instance = this;
			if (InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
			{
				leftHandTrs.SetParent(headTrs);
				rightHandTrs.SetParent(headTrs);
			}
			currentThrowSpeed = throwSpeedRange.max;
			base.OnEnable ();
		}

		public override void DoUpdate ()
		{
			mouseScrollWheelInput = InputManager.MouseScrollWheelInput;
			rightHandPosition = rightHandTrs.position;
			mouseMovement = InputManager.MouseMovement;
			turnInput = InputManager.TurnInput;
			HandleFacing ();
			leftThrowInput = InputManager.LeftThrowInput;
			rightThrowInput = InputManager.RightThrowInput;
			leftGrabInput = InputManager.LeftGrabInput;
			rightGrabInput = InputManager.RightGrabInput;
			HandleGrabbing ();
			HandleRotatingGrabbedObjects ();
			HandleAiming ();
			HandleThrowing ();
			leftReplaceInput = InputManager.LeftReplaceInput;
			rightReplaceInput = InputManager.RightReplaceInput;
			HandleReplacement ();
			jumpInput = InputManager.JumpInput;
			HandleVelocity ();
			HandleHandOrientation ();
			HandleHeadOrientation ();
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
			previousJumpInput = jumpInput;
			previousLeftGrabInput = leftGrabInput;
			previousRightGrabInput = rightGrabInput;
		}

		public override void OnDisable ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			base.OnDisable ();
			if (!invulnerable)
				GameManager.instance.ReloadActiveScene ();
		}

		void HandleVelocity ()
		{
			move.x = 0;
			move.z = 0;
			isGrounded = controller.isGrounded;
			if (isGrounded)
				timeLastGrounded = Time.time;
			Move ();
			RaycastHit hit;
			if (Physics.Raycast(trs.position, move, out hit, collider.bounds.extents.x + move.magnitude * Time.deltaTime, whatICollideWith))
			{
				PhysicsObject physicsObject = hit.collider.GetComponentInParent<PhysicsObject>();
				if (leftGrabbedPhysicsObject != physicsObject && rightGrabbedPhysicsObject != physicsObject)
					move = Vector3.up * move.y;
			}
			HandleGravity ();
			HandleJump ();
			if (controller.enabled)
			{
				rigid.velocity = Vector3.zero;
				controller.Move(move * Time.deltaTime);
			}
			else
				rigid.velocity = move;
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
				handTrs.localPosition = Vector3.ClampMagnitude(oculusTouchController.devicePosition.ReadValue(), maxHandDistance);
				handTrs.localRotation = oculusTouchController.deviceRotation.ReadValue();
			}
			else
			{
				handTrs.localPosition = initLocalPosition;
				handTrs.localRotation = Quaternion.identity;
			}
			Ray ray = new Ray(trs.position, handTrs.position - trs.position);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, (handTrs.position - trs.position).magnitude, whatICollideWith))
				handTrs.position = hit.point;
		}

		void HandleHeadOrientation ()
		{
			if (InputManager.hmd == null || InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
			{
				headTrs.localPosition = Vector3.zero;
				if (!InputManager.RightRotateInput && !InputManager.LeftRotateInput)
				{
					float headRotationX = headTrs.localEulerAngles.y + mouseMovement.x * headRotationSensitivity.x;
					headRotationY += mouseMovement.y * headRotationSensitivity.y;
					headRotationY = Mathf.Clamp(headRotationY, headYRotationRange.min, headYRotationRange.max);
					headTrs.localEulerAngles = new Vector3(-headRotationY, headRotationX, 0);
				}
			}
			else
			{
				Vector3 newHeadLocalPosition = Vector3.ClampMagnitude(InputManager.hmd.devicePosition.ReadValue(), maxHeadDistance);
				if (!Physics.CheckSphere(headTrs.TransformPoint(newHeadLocalPosition), headSphereCollider.bounds.extents.x, whatICollideWith))
					headTrs.localPosition = newHeadLocalPosition;
				headTrs.localRotation = InputManager.hmd.deviceRotation.ReadValue();
			}
		}

		void HandleReplacement ()
		{
			if (leftReplaceInput && !previousLeftReplaceInput)
				Level.Instance.leftOrb.ReplaceObjects ();
			if (rightReplaceInput && !previousRightReplaceInput)
				Level.Instance.rightOrb.ReplaceObjects ();
		}

		void HandleFacing ()
		{
			if (turnInput && !previousTurnInput)
				trs.forward = GameCamera.Instance.trs.forward.SetY(0);
		}

		void HandleGrabbing ()
		{
			HandleGrabbing (leftGrabInput, ref leftCanThrow, leftHandTrs, physicsObjectsTouchingLeftHand.ToArray(), ref leftGrabbedPhysicsObject, rightGrabbedPhysicsObject, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleGrabbing (rightGrabInput, ref rightCanThrow, rightHandTrs, physicsObjectsTouchingRightHand.ToArray(), ref rightGrabbedPhysicsObject, leftGrabbedPhysicsObject, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleGrabbing (bool grabInput, ref bool canThrow, Transform handTrs, PhysicsObject[] touchingPhysicsObjects, ref PhysicsObject grabbedPhysicsObject, PhysicsObject otherGrabbedPhysicsObject, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (grabInput)
			{
				if (canThrow)
				{
					if (grabbedPhysicsObject == null)
					{
						for (int i = 0; i < touchingPhysicsObjects.Length; i ++)
						{
							PhysicsObject physicsObject = touchingPhysicsObjects[i];
							if (physicsObject.isGrabbable && physicsObject != grabbedPhysicsObject && physicsObject != otherGrabbedPhysicsObject)
							{
								StopCoroutine(UnignoreCollisionDelayRoutine (physicsObject.collider));
								IgnoreCollision (physicsObject.collider, true);
								physicsObject.trs.SetParent(handTrs);
								grabbedPhysicsObject = physicsObject;
								SetGrabPosition (grabbedPhysicsObject);
								grabbedPhysicsObject.rigid.isKinematic = true;
								Orb orb = physicsObject.GetComponent<Orb>();
								if (orb != null && Level.Instance.orbs.Length == 2)
								{
									if ((orb == Level.instance.orbs[0]) == (handTrs == leftHandTrs))
									{
										Level.instance.leftOrb = orb;
										Level.instance.rightOrb = Level.instance.orbs[1];
									}
									else
									{
										Level.instance.leftOrb = Level.instance.orbs[1];
										Level.instance.rightOrb = orb;
									}
								}
							}
						}
					}
					else
					{
						if (grabbedPhysicsObject.trs.parent == handTrs)
							SetGrabPosition (grabbedPhysicsObject);
						else
						{
							StopCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
							StartCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
							grabbedPhysicsObject.rigid.isKinematic = false;
							grabbedPhysicsObject = null;
						}
					}
				}
			}
			else
			{
				canThrow = true;
				if (grabbedPhysicsObject != null)
				{
					if (grabbedPhysicsObject.trs.parent == handTrs)
					{
						grabbedPhysicsObject.trs.SetParent(null);
						grabbedPhysicsObject.trs.position = trs.position + GameCamera.Instance.trs.forward.SetY(0).normalized * grabbedPhysicsObjectDropDistance;
						grabbedPhysicsObject.rigid.velocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
						grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
					}
					StopCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
					StartCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
					grabbedPhysicsObject.rigid.isKinematic = false;
					grabbedPhysicsObject = null;
				}
			}
		}

		void SetGrabPosition (PhysicsObject physicsObject)
		{
			List<Collider> overlappedColliders = new List<Collider>(physicsObject.OverlapCollider());
			overlappedColliders.Remove(collider);
			overlappedColliders.Remove(controller);
			overlappedColliders.Remove(headSphereCollider);
			if (overlappedColliders.Count > 0)
			{
				physicsObject.rigid.useGravity = false;
				physicsObject.rigid.isKinematic = false;
				physicsObject.rigid.WakeUp();
				Physics.Simulate(Time.fixedDeltaTime);
				physicsObject.rigid.isKinematic = true;
				physicsObject.rigid.useGravity = true;
			}
			else
			{
				physicsObject.trs.localPosition = Vector3.zero;
				overlappedColliders = new List<Collider>(physicsObject.OverlapCollider());
				if (overlappedColliders.Count > 0)
				{
					physicsObject.rigid.useGravity = false;
					physicsObject.rigid.isKinematic = false;
					physicsObject.rigid.WakeUp();
					Physics.Simulate(Time.fixedDeltaTime);
					physicsObject.rigid.isKinematic = true;
					physicsObject.rigid.useGravity = true;
				}
			}
		}

		void HandleAiming ()
		{
			HandleAiming (leftThrowInput, previousLeftThrowInput, leftCanThrow, leftHandTrs, leftGrabbedPhysicsObject, leftAimer, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleAiming (rightThrowInput, previousRightThrowInput, rightCanThrow, rightHandTrs, rightGrabbedPhysicsObject, rightAimer, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleAiming (bool throwInput, bool previousThrowInput, bool canThrow, Transform handTrs, PhysicsObject grabbedPhysicsObject, LineRenderer aimer, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (!canThrow)
				aimer.enabled = false;
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
			HandleThrowing (leftThrowInput, previousLeftThrowInput, ref leftCanThrow, leftHandTrs, ref leftGrabbedPhysicsObject, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleThrowing (rightThrowInput, previousRightThrowInput, ref rightCanThrow, rightHandTrs, ref rightGrabbedPhysicsObject, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleThrowing (bool throwInput, bool previousThrowInput, ref bool canThrow, Transform handTrs, ref PhysicsObject grabbedPhysicsObject, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			if (grabbedPhysicsObject != null)
			{
				if (!throwInput && previousThrowInput && canThrow)
				{
					canThrow = false;
					grabbedPhysicsObject.trs.SetParent(null);
					grabbedPhysicsObject.trs.position = trs.position + GameCamera.Instance.trs.forward.SetY(0).normalized * grabbedPhysicsObjectDropDistance;
					Vector3 throwVelocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
					if (InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
						throwVelocity += handTrs.forward * currentThrowSpeed;
					grabbedPhysicsObject.rigid.velocity = throwVelocity;
					grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
					StopCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
					StartCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
					grabbedPhysicsObject.rigid.isKinematic = false;
					grabbedPhysicsObject = null;
				}
				else if (throwInput && !previousThrowInput)
					canThrow = true;
			}
		}

		void HandleRotatingGrabbedObjects ()
		{
			HandleRotatingGrabbedObject (InputManager.LeftRotateInput, ref leftCanThrow, leftGrabbedPhysicsObject);
			HandleRotatingGrabbedObject (InputManager.RightRotateInput, ref rightCanThrow, rightGrabbedPhysicsObject);
		}

		void HandleRotatingGrabbedObject (bool rotateInput, ref bool canThrow, PhysicsObject grabbedPhysicsObject)
		{
			if (grabbedPhysicsObject == null)
			{
				canThrow = true;
				return;
			}
			if (rotateInput)
			{
				canThrow = false;
				grabbedPhysicsObject.trs.localEulerAngles += new Vector3(-mouseMovement.y * rotateRate.x, mouseMovement.x * rotateRate.y) * Time.deltaTime;
				grabbedPhysicsObject.trs.RotateAround(grabbedPhysicsObject.trs.position, grabbedPhysicsObject.trs.forward, mouseScrollWheelInput * rollRate * Time.deltaTime);
			}
			else if (grabbedPhysicsObject is Orb && InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
				grabbedPhysicsObject.trs.rotation = headTrs.rotation;
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
			moveInput = Quaternion.Euler(Vector3.up * GameCamera.Instance.trs.eulerAngles.y) * moveInput;
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
				RaycastHit hit;
				if (Physics.Raycast(collider.bounds.center + Vector3.down * collider.bounds.extents.y, Vector3.down, out hit, spikeCheckDistance, whatICollideWith) && hit.collider.GetComponent<Spikes>() != null)
					GameManager.Instance.ReloadActiveScene ();
				Jump ();
			}
			else if (move.y > 0)
				move.y = 0;
		}

		void HandleSlopes ()
		{
			RaycastHit hit;
			if (Physics.Raycast(collider.bounds.center + Vector3.down * collider.bounds.extents.y, Vector3.down, out hit, groundCheckDistance, whatICollideWith))
			{
				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
				if (slopeAngle <= controller.slopeLimit)
				{
					controller.enabled = true;
					rigid.useGravity = false;
					rigid.isKinematic = true;
					rigid.velocity = Vector2.zero;
					return;
				}
				// else
				// 	return;
			}
			else
				return;
			rigid.useGravity = true;
			rigid.isKinematic = false;
			if (controller.enabled)
				rigid.velocity = controller.velocity;
			controller.enabled = false;
		}
		
		void Jump ()
		{
			// if (!previousJumpInput)
				move.y = jumpSpeed;
		}

		void OnCollisionEnter (Collision coll)
		{
			HandleSlopes ();
			if (coll.gameObject.GetComponent<Spikes>() != null)
				GameManager.Instance.ReloadActiveScene ();
		}

		void OnCollisionStay (Collision coll)
		{
			HandleSlopes ();
		}

		void OnControllerColliderHit (ControllerColliderHit hit)
		{
			HandleSlopes ();
			if (hit.gameObject.GetComponent<Spikes>() != null)
				GameManager.Instance.ReloadActiveScene ();
		}

		void OnTriggerEnter (Collider other)
		{
			PhysicsObject physicsObject = other.GetComponentInParent<PhysicsObject>();
			if (physicsObject != null)
			{
				Collider[] hits = Physics.OverlapSphere(leftHandTrs.position, leftHandSphereSensorCollider.bounds.extents.x, whatIsGrabbable);
				if (hits.Contains(physicsObject.collider))
				{
					if (!physicsObjectsTouchingLeftHand.Contains(physicsObject))
						physicsObjectsTouchingLeftHand.Add(physicsObject);
					hits = Physics.OverlapSphere(rightHandTrs.position, rightHandSphereSensorCollider.bounds.extents.x, whatIsGrabbable);
					if (hits.Contains(physicsObject.collider))
						physicsObjectsTouchingRightHand.Add(physicsObject);
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
				Collider[] hits = Physics.OverlapSphere(leftHandTrs.position, leftHandSphereSensorCollider.bounds.extents.x, whatIsGrabbable);
				if (hits.Contains(physicsObject.collider))
					physicsObjectsTouchingRightHand.Remove(physicsObject);
				else
				{
					physicsObjectsTouchingLeftHand.Remove(physicsObject);
					hits = Physics.OverlapSphere(rightHandTrs.position, rightHandSphereSensorCollider.bounds.extents.x, whatIsGrabbable);
					if (!hits.Contains(physicsObject.collider))
						physicsObjectsTouchingRightHand.Remove(physicsObject);
				}
			}
		}

		IEnumerator UnignoreCollisionDelayRoutine (Collider collider)
		{
			yield return new WaitForSeconds(unignoreCollisionDelay);
			IgnoreCollision (collider, false);
		}

		void IgnoreCollision (Collider collider, bool ignore)
		{
			Physics.IgnoreCollision(collider, this.collider, ignore);
			Physics.IgnoreCollision(collider, controller, ignore);
			Physics.IgnoreCollision(collider, headSphereCollider, ignore);
		}
	}
}