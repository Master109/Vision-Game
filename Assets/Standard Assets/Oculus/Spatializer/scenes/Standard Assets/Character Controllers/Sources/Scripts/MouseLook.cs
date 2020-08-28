using UnityEngine;
using System.Collections;
using VisionGame;
using Extensions;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour, IUpdatable
{
	public bool PauseWhileUnfocused
	{
		get
		{
			return true;
		}
	}
	public Transform trs;
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public Vector2 sensitivity;
	public FloatRange yRange;
	float rotationY;
	Vector2 previousMousePosition;

	void OnEnable ()
	{
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	void OnDisable ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}

	public void DoUpdate ()
	{
		Vector2 mousePosition = InputManager.MousePosition;
		Vector2 mousePositionDelta = mousePosition - previousMousePosition;
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = trs.localEulerAngles.y + mousePositionDelta.x * sensitivity.x;
			rotationY += mousePositionDelta.y * sensitivity.y;
			rotationY = Mathf.Clamp(rotationY, yRange.min, yRange.max);
			trs.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			trs.Rotate(0, mousePositionDelta.x * sensitivity.x, 0);
		}
		else
		{
			rotationY += mousePositionDelta.y * sensitivity.y;
			rotationY = Mathf.Clamp(rotationY, yRange.min, yRange.max);
			trs.localEulerAngles = new Vector3(-rotationY, trs.localEulerAngles.y, 0);
		}
		previousMousePosition = mousePosition;
	}

	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}
}
