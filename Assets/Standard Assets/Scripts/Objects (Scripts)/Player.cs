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
	public class Player : SingletonMonoBehaviour<Player>, IUpdatable
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
		public float extraVelocityDrag;
		public Vector2 headRotationSensitivity;
		public FloatRange headYRotationRange;
		public float timeTillApplyExtraVelocityDrag;
		public Transform headTrs;
		public float maxHeadDistance;
		public SphereCollider headSphereCollider;
		public SphereCollider leftHandSphereCollider;
		public SphereCollider rightHandSphereCollider;
		public float spikeCheckDistance;
		public float unignoreCollisionDelay;
		Vector3 extraVelocity;
		[SerializeField]
		[HideInInspector]
		Vector3 initLeftHandLocalPosition;
		[SerializeField]
		[HideInInspector]
		Vector3 initRightHandLocalPosition;
		float timeExtraVelocityWasSet;
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
			rightHandPosition = rightHandTrs.position;
			mouseMovement = InputManager.MouseMovement;
			HandleHandOrientation ();
			HandleHeadOrientation ();
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
				handTrs.localPosition = Vector3.ClampMagnitude(oculusTouchController.devicePosition.ReadValue(), maxHandDistance);
				handTrs.localRotation = oculusTouchController.deviceRotation.ReadValue();
			}
			else
			{
				handTrs.localPosition = initLocalPosition;
				handTrs.localRotation = Quaternion.identity;
			}
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
							if (physicsObject.isGrabbable && !physicsObject.Equals(grabbedPhysicsObject) && !physicsObject.Equals(otherGrabbedPhysicsObject))
							{
								IgnoreCollision (physicsObject.collider, true);
								physicsObject.trs.SetParent(handTrs);
								grabbedPhysicsObject = physicsObject;
								SetGrabPosition (grabbedPhysicsObject);
								grabbedPhysicsObject.rigid.isKinematic = true;
								Orb orb = physicsObject.GetComponent<Orb>();
								if (orb != null && Level.Instance.orbs.Length == 2)
								{
									if ((orb == Level.Instance.orbs[0]) == (handTrs == leftHandTrs))
									{
										Level.Instance.leftOrb = orb;
										Level.Instance.rightOrb = Level.Instance.orbs[1];
									}
									else
									{
										Level.Instance.leftOrb = Level.Instance.orbs[1];
										Level.Instance.rightOrb = orb;
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
						grabbedPhysicsObject.rigid.velocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
						grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
					}
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
			overlappedColliders.Remove(leftHandSphereCollider);
			overlappedColliders.Remove(rightHandSphereCollider);
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
			HandleAiming (leftThrowInput, previousLeftThrowInput, ref leftCanThrow, leftHandTrs, leftGrabbedPhysicsObject, leftAimer, previousLeftHandPosition, previousLeftHandEulerAngles);
			HandleAiming (rightThrowInput, previousRightThrowInput, ref rightCanThrow, rightHandTrs, rightGrabbedPhysicsObject, rightAimer, previousRightHandPosition, previousRightHandEulerAngles);
		}

		void HandleAiming (bool throwInput, bool previousThrowInput, ref bool canThrow, Transform handTrs, PhysicsObject grabbedPhysicsObject, LineRenderer aimer, Vector3 previousHandPosition, Vector3 previousHandEulerAngles)
		{
			// if (!canThrow)
			// {
			// 	aimer.enabled = false;
				// if (throwInput && !previousThrowInput)
				// {
				// }
				// else
				// {
				// 	aimer.enabled = false;
					// return;
				// }
			// }
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
			if (!canThrow)
				return;
			if (!throwInput && previousThrowInput && grabbedPhysicsObject != null)
			{
				canThrow = false;
				grabbedPhysicsObject.trs.SetParent(null);
				Vector3 throwVelocity = (handTrs.position - previousHandPosition) / Time.deltaTime;
				if (InputManager._InputDevice == InputManager.InputDevice.KeyboardAndMouse)
					throwVelocity += handTrs.forward * currentThrowSpeed;
				grabbedPhysicsObject.rigid.velocity = throwVelocity;
				grabbedPhysicsObject.rigid.angularVelocity = QuaternionExtensions.GetAngularVelocity(Quaternion.Euler(previousHandEulerAngles), handTrs.rotation);
				StartCoroutine(UnignoreCollisionDelayRoutine (grabbedPhysicsObject.collider));
				grabbedPhysicsObject.rigid.isKinematic = false;
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
			else
				canThrow = true;
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
			if (coll.gameObject.GetComponent<Spikes>() != null)
				GameManager.Instance.ReloadActiveScene ();
		}

		void OnCollisionStay (Collision coll)
		{
			if (coll.rigidbody != null)
				HandleBeingPushed (coll.rigidbody);
			HandleSlopes ();
		}

		void OnControllerColliderHit (ControllerColliderHit hit)
		{
			if (hit.rigidbody != null)
				HandleBeingPushed (hit.rigidbody);
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
			Physics.IgnoreCollision(collider, leftHandSphereCollider, ignore);
			Physics.IgnoreCollision(collider, rightHandSphereCollider, ignore);
		}
	}
}