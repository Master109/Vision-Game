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

namespace BeatKiller
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
		public Vector2Int checks;
		public Transform capturedObjectsParent;
		public Transform oldCapturedObjectsParent;
		public CharacterController controller;
		public float jumpSpeed;
		public float jumpDuration;
		public float gravity;
		public float multiplyRigidMoveSpeed;
		[HideInInspector]
		public Vector3 move;
		public Rigidbody rigid;
		public float moveSpeed;
		bool replaceInput;
		bool previousReplaceInput;
		Quaternion previousRotation;
		float yVel;
		Vector3 previousPosition;
		bool canJump;
		float timeLastGrounded;

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
			if (replaceInput && !previousReplaceInput)
				ReplaceObjects ();
			move = Vector3.zero;
			if (controller.isGrounded)
			{
				timeLastGrounded = Time.time;
				canJump = true;
			}
			HandleFacing ();
			Move ();
			HandleGravity ();
			HandleJump ();
			controller.Move(move * Time.deltaTime);
			previousReplaceInput = replaceInput;
		}
		
		void ReplaceObjects ()
		{
			int storedObjectsCount = capturedObjectsParent.childCount;
			for (int i = 0; i < storedObjectsCount; i ++)
				capturedObjectsParent.GetChild(0).SetParent(oldCapturedObjectsParent, true);
			List<Collider> hitColliders = new List<Collider>();
			for (float x = 0; x <= 1; x += 1f / checks.x)
			{
				for (float y = 0; y <= 1; y += 1f / checks.y)
				{
					RaycastHit hit;
					if (Physics.Raycast(GameManager.GetSingleton<GameCamera>().camera.ViewportPointToRay(new Vector2(x, y)), out hit) && !hitColliders.Contains(hit.collider))
						hitColliders.Add(hit.collider);
				}
			}
			foreach (Collider hitCollider in hitColliders)
			{
				IStorable storable = hitCollider.GetComponent<IStorable>();
				if (storable != null)
					Instantiate(storable.Trs, storable.Trs.position, storable.Trs.rotation, capturedObjectsParent);
				IDestroyable destroyable = hitCollider.GetComponent<IDestroyable>();
				if (destroyable != null)
					Destroy(destroyable.Go);
			}
			oldCapturedObjectsParent.DetachChildren();
		}

		void OnDisable ()
		{
			GameManager.updatables = GameManager.updatables.Remove(this);
		}

		void HandleFacing ()
		{
			trs.forward = GameManager.GetSingleton<GameCamera>().trs.forward.SetY(0);
		}
		
		void HandleGravity ()
		{
			if (controller.enabled && !controller.isGrounded)
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
			move += GetMoveInput();
			if (controller.enabled)
				move *= moveSpeed;
			else
				rigid.velocity = (move * moveSpeed * multiplyRigidMoveSpeed).SetY(rigid.velocity.y);
		}
		
		void HandleJump ()
		{
			bool hasJumpInput = GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.KeyboardAndMouse && Keyboard.current.leftShiftKey.isPressed;
			hasJumpInput |= GameManager.GetSingleton<InputManager>().inputDevice == InputManager.InputDevice.OculusRift && (InputManager.leftTouchController.primaryButton.isPressed || InputManager.leftTouchController.secondaryButton.isPressed || InputManager.rightTouchController.primaryButton.isPressed || InputManager.rightTouchController.secondaryButton.isPressed);
			if (canJump && hasJumpInput && Time.time - timeLastGrounded < jumpDuration)
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
			if (controller.enabled)
			{
				yVel += jumpSpeed * Time.deltaTime;
				move += Vector3.up * yVel;
			}
		}
	}
}