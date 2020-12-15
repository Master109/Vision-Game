#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Events;
using Extensions;

[ExecuteInEditMode]
public class EditorScript : MonoBehaviour
{
	public static InputEvent inputEvent = new InputEvent();
	public Hotkey[] hotkeys = new Hotkey[0];

	public virtual void OnEnable ()
	{
		if (Application.isPlaying)
		{
			EditorApplication.update -= DoEditorUpdate;
			return;
		}
		else
			EditorApplication.update += DoEditorUpdate;
	}

	public virtual void OnDisable ()
	{
		if (Application.isPlaying)
			return;
		EditorApplication.update -= DoEditorUpdate;
	}

	public virtual void DoEditorUpdate ()
	{
	}

	public virtual void UpdateHotkeys ()
	{
		if (hotkeys.Length > 0)
		{
			for (int i = 0; i < hotkeys.Length; i ++)
			{
				Hotkey hotkey = hotkeys[i];
				if (Event.current != null)
				{
					bool shouldBreak = false;
					inputEvent.mousePosition = Event.current.mousePosition.ToVec2Int();
					inputEvent.type = Event.current.type;
					foreach (Hotkey.Button button in hotkey.buttons)
					{
						if (Event.current.keyCode == button.key)
						{
							if (Event.current.type == EventType.KeyDown)
							{
								inputEvent.keys = inputEvent.keys.Add(Event.current.keyCode);
								button.isPressing = true;
								if (hotkey.downType == Hotkey.DownType.All)
								{
									foreach (Hotkey.Button button2 in hotkey.buttons)
									{
										if (!button2.isPressing)
										{
											shouldBreak = true;
											break;
										}
									}
									if (shouldBreak)
										break;
								}
								hotkey.downAction.Invoke();
							}
							else if (Event.current.type == EventType.KeyUp)
							{
								inputEvent.keys = inputEvent.keys.Remove(Event.current.keyCode);
								button.isPressing = false;
								if (hotkey.upType == Hotkey.UpType.All)
								{
									foreach (Hotkey.Button button2 in hotkey.buttons)
									{
										if (button2.isPressing)
										{
											shouldBreak = true;
											break;
										}
									}
									if (shouldBreak)
										break;
								}
								hotkey.upAction.Invoke();
							}
						}
					}
				}
				inputEvent.previousKeys = (KeyCode[]) inputEvent.keys.Clone();
			}
		}
		else if (Event.current != null)
		{
			inputEvent.mousePosition = Event.current.mousePosition.ToVec2Int();
			inputEvent.type = Event.current.type;
		}
	}

	public static Vector2 GetMousePosition ()
	{
		Camera camera = SceneView.lastActiveSceneView.camera;
		if (camera == null)
			camera = SceneView.currentDrawingSceneView.camera;
		return GetMousePosition(camera);
	}

	public static Vector2 GetMousePosition (Camera camera)
	{
		Vector2 output = inputEvent.mousePosition;
		output.y = camera.ViewportToScreenPoint(Vector2.one).y - camera.ViewportToScreenPoint(Vector2.zero).y - output.y;
		return output;
	}

	public static Vector2 GetMousePositionInWorld ()
	{
		Camera camera = SceneView.lastActiveSceneView.camera;
		if (camera == null)
			camera = SceneView.currentDrawingSceneView.camera;
		return GetMousePositionInWorld(camera);
	}

	public static Vector2 GetMousePositionInWorld (Camera camera)
	{
		return camera.ScreenToWorldPoint(GetMousePosition(camera));
	}

	public static Ray GetMouseRay ()
	{
		Camera camera = SceneView.lastActiveSceneView.camera;
		if (camera == null)
			camera = SceneView.currentDrawingSceneView.camera;
		return GetMouseRay(camera);
	}

	public static Ray GetMouseRay (Camera camera)
	{
		return camera.ScreenPointToRay(GetMousePosition(camera));
	}

	[Serializable]
	public struct Hotkey
	{
		public string name;
		public Button[] buttons;
		public DownType downType;
		public UpType upType;
		public UnityEvent downAction;
		public UnityEvent upAction;

		public enum DownType
		{
			All,
			Any
		}

		public enum UpType
		{
			All
		}

		[Serializable]
		public class Button
		{
			public KeyCode key;
			public bool isPressing;
		}
	}

	public class InputEvent
	{
		public Vector2Int mousePosition;
		public EventType type;
		public KeyCode[] keys = new KeyCode[0];
		public KeyCode[] previousKeys = new KeyCode[0];
	}
}

[CustomEditor(typeof(EditorScript))]
public class EditorScriptEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}

	public virtual void OnSceneGUI ()
	{
		EditorScript editorScript = (EditorScript) target;
		editorScript.UpdateHotkeys ();
	}
}
#else
using UnityEngine;

public class EditorScript : MonoBehaviour
{
}
#endif