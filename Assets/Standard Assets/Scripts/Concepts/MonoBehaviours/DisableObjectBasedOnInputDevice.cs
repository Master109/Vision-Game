using UnityEngine;
using VisionGame;

public class DisableObjectBasedOnInputDevice : MonoBehaviour
{
	public bool disableIfUsing;
	public InputManager.InputDevice inputDevice;
	
	void Awake ()
	{
		gameObject.SetActive(InputManager._InputDevice == inputDevice != disableIfUsing);
	}
}