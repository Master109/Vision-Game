using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Extensions;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;

namespace VisionGame
{
	[ExecuteInEditMode]
	public class GameManager : SingletonMonoBehaviour<GameManager>, ISaveableAndLoadable
	{
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		public int uniqueId;
		public int UniqueId
		{
			get
			{
				return uniqueId;
			}
			set
			{
				uniqueId = value;
			}
		}
		public static bool paused;
		public GameObject[] registeredGos = new GameObject[0];
		[SaveAndLoadValue(false)]
		public static string enabledGosString = "";
		[SaveAndLoadValue(false)]
		public static string disabledGosString = "";
		public const string STRING_SEPERATOR = "|";
		public float timeScale;
		public static IUpdatable[] updatables = new IUpdatable[0];
		public static IUpdatable[] pausedUpdatables = new IUpdatable[0];
		public static Dictionary<Type, object> singletons = new Dictionary<Type, object>();
		public const char UNIQUE_ID_SEPERATOR = ',';
#if UNITY_EDITOR
		public static int[] UniqueIds
		{
			get
			{
				int[] output = new int[0];
				string[] uniqueIdsString = EditorPrefs.GetString("Unique ids").Split(UNIQUE_ID_SEPERATOR);
				int uniqueIdParsed;
				foreach (string uniqueIdString in uniqueIdsString)
				{
					if (int.TryParse(uniqueIdString, out uniqueIdParsed))
						output = output.Add(uniqueIdParsed);
				}
				return output;
			}
			set
			{
				string uniqueIdString = "";
				foreach (int uniqueId in value)
					uniqueIdString += uniqueId + UNIQUE_ID_SEPERATOR;
				EditorPrefs.SetString("Unique ids", uniqueIdString);
			}
		}
		public bool doEditorUpdates;
#endif
		public static int framesSinceLoadedScene;
		public const int LAG_FRAMES_AFTER_LOAD_SCENE = 2;
		public static float UnscaledDeltaTime
		{
			get
			{
				if (paused || framesSinceLoadedScene <= LAG_FRAMES_AFTER_LOAD_SCENE)
					return 0;
				else
					return Time.unscaledDeltaTime;
			}
		}
		public Animator screenEffectAnimator;
		public CursorEntry[] cursorEntries;
		public static Dictionary<string, CursorEntry> cursorEntriesDict = new Dictionary<string, CursorEntry>();
		public static CursorEntry activeCursorEntry;
		public RectTransform cursorCanvas;
		public GameModifier[] gameModifiers;
		public static Dictionary<string, GameModifier> gameModifierDict = new Dictionary<string, GameModifier>();
		public Timer hideCursorTimer;
		public GameScene[] gameScenes;
		// public Canvas[] canvases = new Canvas[0];
		Vector2 moveInput;
		public static Vector2 previousMousePosition;
		public delegate void OnGameScenesLoaded();
		public static event OnGameScenesLoaded onGameScenesLoaded;
		public GameObject emptyGoPrefab;
		public TemporaryActiveText notificationText;
		public static bool initialized;
		public static bool HasPlayedBefore
		{
			get
			{
				return PlayerPrefs.GetInt("Has played before ", 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("Has played before ", value.GetHashCode());
			}
		}
		// public static int GameplaySession
		// {
		// 	get
		// 	{
		// 		return PlayerPrefs.GetInt("Gameplay session", 0);
		// 	}
		// 	set
		// 	{
		// 		PlayerPrefs.SetInt("Gameplay session", value);
		// 	}
		// }
		public static bool isFocused;
		public GameObject textPanelGo;
		public _Text textPanelText;
		public float distanceScale = 1;
		bool leftGameplayMenuInput;
		bool previousLeftGameplayMenuInput;
		bool rightGameplayMenuInput;
		bool previousRightGameplayMenuInput;
		bool gameplayMenuInput;
		bool previousGameplayMenuInput;

		public override void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				framesSinceLoadedScene = 0;
				Transform[] transforms = FindObjectsOfType<Transform>();
				IIdentifiable[] identifiables = new IIdentifiable[0];
				foreach (Transform trs in transforms)
				{
					identifiables = trs.GetComponents<IIdentifiable>();
					foreach (IIdentifiable identifiable in identifiables)
					{
						if (!UniqueIds.Contains(identifiable.UniqueId))
							UniqueIds = UniqueIds.Add(identifiable.UniqueId);
					}
				}
				return;
			}
			// else
			// {
			// 	for (int i = 0; i < gameScenes.Length; i ++)
			// 	{
			// 		if (!gameScenes[i].use)
			// 		{
			// 			gameScenes = gameScenes.RemoveAt(i);
			// 			i --;
			// 		}
			// 	}
			// }
#endif
			base.Awake ();
			singletons.Remove(GetType());
			singletons.Add(GetType(), this);
			// InitCursor ();
			AccountManager.lastUsedAccountIndex = 0;
			if (SceneManager.GetActiveScene().name == "Init")
				LoadGameScenes ();
			else if (GameCamera.Instance != null)
				StartCoroutine(OnGameSceneLoadedRoutine ());
		}

		void Init ()
		{
			initialized = true;
		}
		
		IEnumerator OnGameSceneLoadedRoutine ()
		{
			gameModifierDict.Clear();
			foreach (GameModifier gameModifier in gameModifiers)
				gameModifierDict.Add(gameModifier.name, gameModifier);
			hideCursorTimer.onFinished += HideCursor;
			if (screenEffectAnimator != null)
				screenEffectAnimator.Play("None");
			// GetSingleton<PauseMenu>().Hide ();
			if (AccountManager.lastUsedAccountIndex != -1)
			{
				// GetSingleton<AccountSelectMenu>().gameObject.SetActive(false);
				PauseGame (false);
			}
			// canvases = FindObjectsOfType<Canvas>();
			// foreach (Canvas canvas in canvases)
			// 	canvas.worldCamera = GetSingleton<GameCamera>().camera;
			if (onGameScenesLoaded != null)
			{
				onGameScenesLoaded ();
				onGameScenesLoaded = null;
			}
			yield return StartCoroutine(LoadRoutine ());
			yield break;
		}

		void Update ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
				if (!initialized)
					return;
			// try
			// {
				InputSystem.Update ();
				leftGameplayMenuInput = InputManager.LeftGameplayMenuInput;
				rightGameplayMenuInput = InputManager.RightGameplayMenuInput;
				gameplayMenuInput = InputManager.GameplayMenuInput;
				HandleGameplayMenu ();
				for (int i = 0; i < updatables.Length; i ++)
				{
					IUpdatable updatable = updatables[i];
					updatable.DoUpdate ();
				}
				Physics.Simulate(Time.deltaTime);
				ObjectPool.instance.DoUpdate ();
				// GameCamera.Instance.DoUpdate ();
				framesSinceLoadedScene ++;
				previousMousePosition = InputManager.MousePosition;
				previousLeftGameplayMenuInput = leftGameplayMenuInput;
				previousRightGameplayMenuInput = rightGameplayMenuInput;
				previousGameplayMenuInput = gameplayMenuInput;
			// }
			// catch (Exception e)
			// {
			// 	Debug.Log(e.Message + "\n" + e.StackTrace);
			// }
		}

		// public Vector3 startDirection;
		// public Vector3 endDirection;
		// public int stepCount;
		// public float turnRadius;
		// public Color gizmosColor;
		// public float gizmosRadius;
		// void OnDrawGizmos ()
		// {
		// 	Vector3[] positionsForTurn = VectorExtensions.GetMovementsForTurn(startDirection, endDirection, stepCount, turnRadius);
		// 	Gizmos.color = gizmosColor;
		// 	Gizmos.matrix = Matrix4x4.identity;
		// 	foreach (Vector3 positionForTurn in positionsForTurn)
		// 		Gizmos.DrawSphere(positionForTurn, gizmosRadius);
		// }
		
		// public Transform wallPrefab;
		// public Transform startTrs;
		// public Transform endTrs;
		// public Transform[] cornerTransforms = new Transform[0];
		// public Transform parent;
		// public Vector2 size = Vector2.one;
		// public Vector2 wallThickness = Vector2.one;
		// public int wallsPerCorner;
		// public WallMakeMode leftWallMakeMode;
		// public WallMakeMode rightWallMakeMode;
		// public WallMakeMode bottomWallMakeMode;
		// public WallMakeMode topWallMakeMode;
		// public Cap startCap;
		// public Cap endCap;
		// public Color gizmosMeshColor;
		// public Color gizmosStartColor;
		// public Color gizmosEndColor;

		// void OnDrawGizmos ()
		// {
		// 	Vector3 currentPosition = startTrs.position;
		// 	List<Transform> nextTransforms = new List<Transform>(cornerTransforms);
		// 	nextTransforms.Add(endTrs);
		// 	do
		// 	{
		// 		Transform nextTrs = nextTransforms[0];
		// 		Vector3 _currentPosition = currentPosition;
		// 		Vector3 nextPosition = nextTrs.position;
		// 		Vector3 _nextPosition = nextPosition;
		// 		Vector3 toNextTrs = nextPosition - currentPosition;
		// 		Vector3 position = nextPosition - (toNextTrs / 2);
		// 		Quaternion rotation = Quaternion.LookRotation(toNextTrs);
		// 		Vector3 localScale = new Vector3(size.x + wallThickness.x * 2, wallThickness.y, toNextTrs.magnitude);
		// 		Gizmos.color = gizmosMeshColor;

		// 		// Transform topWall = Instantiate(wallPrefab, position, rotation, parent);
		// 		// topWall.position += topWall.up * (size.y / 2 + wallThickness.y / 2);
		// 		// topWall.localScale = localScale;
		// 		// Gizmos.DrawMesh(topWall.GetComponent<MeshFilter>().sharedMesh, topWall.position, rotation, localScale);
		// 		// DestroyImmediate(topWall.gameObject);
				
		// 		// Transform bottomWall = Instantiate(wallPrefab, position, rotation, parent);
		// 		// bottomWall.position -= bottomWall.up * (size.y / 2 + wallThickness.y / 2);
		// 		// bottomWall.localScale = localScale;
		// 		// Gizmos.DrawMesh(bottomWall.GetComponent<MeshFilter>().sharedMesh, bottomWall.position, rotation, localScale);
		// 		// DestroyImmediate(bottomWall.gameObject);

		// 		localScale = new Vector3(wallThickness.x, size.y + wallThickness.y * 2, toNextTrs.magnitude);

		// 		Transform rightWall = Instantiate(wallPrefab, position, rotation, parent);
		// 		rightWall.position += rightWall.right * (size.x / 2 + wallThickness.x / 2);
		// 		rightWall.localScale = localScale;
		// 		Gizmos.DrawMesh(rightWall.GetComponent<MeshFilter>().sharedMesh, rightWall.position, rotation, localScale);
		// 		DestroyImmediate(rightWall.gameObject);
				
		// 		Transform leftWall = Instantiate(wallPrefab, position, rotation, parent);
		// 		leftWall.position -= leftWall.right * (size.x / 2 + wallThickness.x / 2);
		// 		leftWall.localScale = localScale;
		// 		Gizmos.DrawMesh(leftWall.GetComponent<MeshFilter>().sharedMesh, leftWall.position, rotation, localScale);
		// 		DestroyImmediate(leftWall.gameObject);

		// 		currentPosition = nextPosition;
		// 		nextTransforms.RemoveAt(0);
		// 		if (nextTransforms.Count > 0)
		// 		{
		// 			nextTrs = nextTransforms[0];
		// 			nextPosition = nextTrs.position;
		// 			Gizmos.color = gizmosStartColor;
		// 			Debug.DrawLine(_currentPosition, _nextPosition);
		// 			Gizmos.color = gizmosEndColor;
		// 			Debug.DrawLine(currentPosition, nextPosition);
		// 			Vector3[] movementsForTurn = VectorExtensions.GetMovementsForTurn(toNextTrs, nextPosition - currentPosition, wallsPerCorner, size.x / 2 + wallThickness.x / 2);
		// 			for (int i = 0; i < wallsPerCorner; i ++)
		// 			{
		// 				Vector3 movementForTurn = movementsForTurn[i];
		// 				position = currentPosition;
		// 				rotation = Quaternion.LookRotation(movementForTurn);
		// 				localScale = new Vector3(wallThickness.x, size.y + wallThickness.y * 2, size.x);
		// 				Gizmos.color = gizmosMeshColor;
						
		// 				Transform cornerWall = Instantiate(wallPrefab, position, rotation, parent);
		// 				cornerWall.position += cornerWall.right * (size.x / 2 + wallThickness.x / 2);
		// 				cornerWall.localScale = localScale;
		// 				Gizmos.DrawMesh(cornerWall.GetComponent<MeshFilter>().sharedMesh, cornerWall.position, rotation, localScale);
		// 				DestroyImmediate(cornerWall.gameObject);

		// 				cornerWall = Instantiate(wallPrefab, position, rotation, parent);
		// 				cornerWall.position -= cornerWall.right * (size.x / 2 + wallThickness.x / 2);
		// 				cornerWall.localScale = localScale;
		// 				Gizmos.DrawMesh(cornerWall.GetComponent<MeshFilter>().sharedMesh, cornerWall.position, rotation, localScale);
		// 				DestroyImmediate(cornerWall.gameObject);
		// 			}
		// 		}
		// 	} while (nextTransforms.Count > 0);
		// }

		// public enum WallMakeMode
		// {
		// 	DoesNotExtendFully = 0,
		// 	ExtendsFullyInPositiveDirectionOnly = 1,
		// 	ExtendsFullyInNegativeDirectionOnly = -1,
		// 	ExtendsFullyInBothDirections = 0
		// }

		// [Serializable]
		// public struct Cap
		// {
		// 	public Transform capPrefab;
		// 	public bool extendsLeftFully;
		// 	public bool extendsRightFully;
		// 	public bool extendsDownFully;
		// 	public bool extendsUpFully;
		// }

		void HandleGameplayMenu ()
		{
			if (GameplayMenu.instance.gameObject.activeSelf || !GameplayMenu.instance.interactive)
				return;
			if (leftGameplayMenuInput && !previousLeftGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = Player.instance.leftHandTrs;
			else if (rightGameplayMenuInput && !previousRightGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = Player.instance.rightHandTrs;
			else if (gameplayMenuInput && !previousGameplayMenuInput)
				GameplayMenu.instance.selectorTrs = Player.instance.headTrs;
			else
				return;
			GameplayMenu.instance.trs.position = GameCamera.Instance.trs.position + (GameCamera.instance.trs.forward * GameplayMenu.instance.distanceFromCamera);
			GameplayMenu.instance.trs.rotation = GameCamera.Instance.trs.rotation;
			GameplayMenu.instance.gameObject.SetActive(true);
		}

		void InitCursor ()
		{
			cursorEntriesDict.Clear();
			foreach (CursorEntry cursorEntry in cursorEntries)
			{
				cursorEntriesDict.Add(cursorEntry.name, cursorEntry);
				cursorEntry.rectTrs.gameObject.SetActive(false);
			}
			// Cursor.visible = false;
			activeCursorEntry = null;
			// cursorEntriesDict["Default"].SetAsActive ();
		}

		IEnumerator LoadRoutine ()
		{
			yield return new WaitForEndOfFrame();
			SaveAndLoadManager.Instance.Setup ();
			if (!HasPlayedBefore)
			{
				SaveAndLoadManager.Instance.DeleteAll ();
				HasPlayedBefore = true;
				SaveAndLoadManager.Instance.OnLoaded ();
			}
			else
				SaveAndLoadManager.Instance.LoadMostRecent ();
			// GetSingleton<AdsManager>().ShowAd ();
			Init ();
			yield break;
		}

		void HideCursor (params object[] args)
		{
			activeCursorEntry.rectTrs.gameObject.SetActive(false);
		}

		public virtual void LoadScene (string name)
		{
			if (GameManager.Instance != this)
			{
				GameManager.Instance.LoadScene (name);
				return;
			}
			framesSinceLoadedScene = 0;
			SceneManager.LoadScene(name);
		}

		public virtual void LoadSceneAdditive (string name)
		{
			if (GameManager.Instance != this)
			{
				GameManager.Instance.LoadSceneAdditive (name);
				return;
			}
			SceneManager.LoadScene(name, LoadSceneMode.Additive);
		}

		public virtual void LoadScene (int index)
		{
			// LoadScene (SceneManager.GetSceneByBuildIndex(index).name);
			if (GameManager.Instance != this)
			{
				GameManager.Instance.LoadScene (index);
				return;
			}
			framesSinceLoadedScene = 0;
			SceneManager.LoadScene(index);
		}

		public virtual void UnloadScene (string name)
		{
			AsyncOperation unloadGameScene = SceneManager.UnloadSceneAsync(name);
			unloadGameScene.completed += OnGameSceneUnloaded;
		}

		public virtual void OnGameSceneUnloaded (AsyncOperation unloadGameScene)
		{
			unloadGameScene.completed -= OnGameSceneUnloaded;
		}

		public virtual void ReloadActiveScene ()
		{
			LoadScene (SceneManager.GetActiveScene().name);
		}

		public virtual void LoadGameScenes ()
		{
			if (GameManager.Instance != this)
			{
				GameManager.Instance.LoadGameScenes ();
				return;
			}
			initialized = false;
			StopAllCoroutines ();
			if (SceneManager.GetSceneByName(gameScenes[0].name).isLoaded)
			{
				// UnloadScene ("Game");
				// LoadSceneAdditive ("Game");
				return;
			}
			LoadScene (gameScenes[0].name);
			GameScene gameScene;
			for (int i = 1; i < gameScenes.Length; i ++)
			{
				gameScene = gameScenes[i];
				if (gameScene.use)
					LoadSceneAdditive (gameScene.name);
			}
		}

		public virtual void PauseGame (bool pause)
		{
			paused = pause;
			Time.timeScale = timeScale * (1 - paused.GetHashCode());
			// AudioListener.pause = paused;
		}

		public virtual void Quit ()
		{
			Application.Quit();
		}

		public virtual void OnApplicationQuit ()
		{
			PauseGame (true);
			if (AccountManager.lastUsedAccountIndex == -1)
				return;
			// AccountManager.CurrentlyPlaying.PlayTime += Time.time;
			// SaveAndLoadManager.Instance.Save ();
		}

		public virtual void OnApplicationFocus (bool isFocused)
		{
			GameManager.isFocused = isFocused;
			if (isFocused)
			{
				foreach (IUpdatable pausedUpdatable in pausedUpdatables)
					updatables = updatables.Add(pausedUpdatable);
				pausedUpdatables = new IUpdatable[0];
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = false;
				foreach (TemporaryActiveGameObject tempActiveGo in TemporaryActiveGameObject.activeInstances)
					tempActiveGo.Do ();
			}
			else
			{
				IUpdatable updatable;
				for (int i = 0; i < updatables.Length; i ++)
				{
					updatable = updatables[i];
					if (updatable.PauseWhileUnfocused)
					{
						pausedUpdatables = pausedUpdatables.Add(updatable);
						updatables = updatables.RemoveAt(i);
						i --;
					}
				}
				foreach (Timer runningTimer in Timer.runningInstances)
					runningTimer.pauseIfCan = true;
				foreach (TemporaryActiveGameObject tempActiveGo in TemporaryActiveGameObject.activeInstances)
					tempActiveGo.Do ();
			}
		}

		public virtual void SetGosActive ()
		{
			if (GameManager.Instance != this)
			{
				GameManager.Instance.SetGosActive ();
				return;
			}
			string[] stringSeperators = { STRING_SEPERATOR };
			if (enabledGosString == null)
				enabledGosString = "";
			string[] enabledGos = enabledGosString.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in enabledGos)
			{
				for (int i = 0; i < registeredGos.Length; i ++)
				{
					if (goName == registeredGos[i].name)
					{
						registeredGos[i].SetActive(true);
						break;
					}
				}
			}
			if (disabledGosString == null)
				disabledGosString = "";
			string[] disabledGos = disabledGosString.Split(stringSeperators, StringSplitOptions.None);
			foreach (string goName in disabledGos)
			{
				GameObject go = GameObject.Find(goName);
				if (go != null)
					go.SetActive(false);
			}
		}
		
		public virtual void ActivateGoForever (GameObject go)
		{
			go.SetActive(true);
			ActivateGoForever (go.name);
		}
		
		public virtual void DeactivateGoForever (GameObject go)
		{
			go.SetActive(false);
			DeactivateGoForever (go.name);
		}
		
		public virtual void ActivateGoForever (string goName)
		{
			disabledGosString = disabledGosString.Replace(STRING_SEPERATOR + goName, "");
			if (!enabledGosString.Contains(goName))
				enabledGosString += STRING_SEPERATOR + goName;
		}
		
		public virtual void DeactivateGoForever (string goName)
		{
			enabledGosString = enabledGosString.Replace(STRING_SEPERATOR + goName, "");
			if (!disabledGosString.Contains(goName))
				disabledGosString += STRING_SEPERATOR + goName;
		}

		public virtual void SetGameObjectActive (string name)
		{
			GameObject.Find(name).SetActive(true);
		}

		public virtual void SetGameObjectInactive (string name)
		{
			GameObject.Find(name).SetActive(false);
		}

		public virtual void OnDestroy ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (GameManager.Instance != this)
				return;
			StopAllCoroutines();
			for (int i = 0; i < Timer.runningInstances.Length; i ++)
			{
				Timer timer = Timer.runningInstances[i];
				timer.Stop ();
				i --;
			}
			hideCursorTimer.onFinished -= HideCursor;
			// SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		public virtual void _Log (object o)
		{
			print(o);
		}

		public static void Log (object o)
		{
			print(o);
		}

		public static Object Clone (Object obj)
		{
			return Instantiate(obj);
		}

		public static Object Clone (Object obj, Transform parent)
		{
			return Instantiate(obj, parent);
		}

		public static Object Clone (Object obj, Vector3 position, Quaternion rotation)
		{
			return Instantiate(obj, position, rotation);
		}

		public static void _Destroy (Object obj)
		{
			Destroy(obj);
		}

		public static void _DestroyImmediate (Object obj)
		{
			DestroyImmediate(obj);
		}

		public virtual void ToggleGo (GameObject go)
		{
			go.SetActive(!go.activeSelf);
		}

		public virtual void PressButton (Button button)
		{
			button.onClick.Invoke();
		}

		// public static T GetSingleton<T> ()
		// {
		// 	if (!singletons.ContainsKey(typeof(T)))
		// 		return GetSingleton<T>(FindObjectsOfType<Object>());
		// 	else
		// 	{
		// 		object instance = singletons[typeof(T)];
		// 		if (instance == null || instance.Equals(default(T)))
		// 		{
		// 			T singleton = GetSingleton<T>(FindObjectsOfType<Object>());
		// 			singletons[typeof(T)] = singleton;
		// 			return singleton;
		// 		}
		// 		else
		// 			return (T) singletons[typeof(T)];
		// 	}
		// }

		// public static T GetSingleton<T> (Object[] objects)
		// {
		// 	if (typeof(T).IsSubclassOf(typeof(Object)))
		// 	{
		// 		foreach (Object obj in objects)
		// 		{
		// 			if (obj is T)
		// 			{
		// 				singletons.Remove(typeof(T));
		// 				singletons.Add(typeof(T), obj);
		// 				break;
		// 			}
		// 		}
		// 	}
		// 	object instance = default(T);
		// 	if (singletons.TryGetValue(typeof(T), out instance))
		// 	{
		// 	}
		// 	return (T) instance;
		// }

		// public static T GetSingletonIncludeAssets<T> ()
		// {
		// 	if (!singletons.ContainsKey(typeof(T)))
		// 		return GetSingletonIncludeAssets<T>(FindObjectsOfTypeIncludingAssets(typeof(T)));
		// 	else
		// 	{
		// 		if (singletons[typeof(T)] == null || singletons[typeof(T)].Equals(default(T)))
		// 		{
		// 			T singleton = GetSingletonIncludeAssets<T>(FindObjectsOfTypeIncludingAssets(typeof(T)));
		// 			singletons[typeof(T)] = singleton;
		// 			return singleton;
		// 		}
		// 		else
		// 			return (T) singletons[typeof(T)];
		// 	}
		// }

		// public static T GetSingletonIncludeAssets<T> (object[] objects)
		// {
		// 	if (typeof(T).IsSubclassOf(typeof(object)))
		// 	{
		// 		foreach (Object obj in objects)
		// 		{
		// 			if (obj is T)
		// 			{
		// 				singletons.Remove(typeof(T));
		// 				singletons.Add(typeof(T), obj);
		// 				break;
		// 			}
		// 		}
		// 	}
		// 	if (singletons.ContainsKey(typeof(T)))
		// 		return (T) singletons[typeof(T)];
		// 	else
		// 		return default(T);
		// }

		public static bool ModifierIsActiveAndExists (string name)
		{
			GameModifier gameModifier;
			if (gameModifierDict.TryGetValue(name, out gameModifier))
				return gameModifier.isActive;
			else
				return false;
		}

		public static bool ModifierIsActive (string name)
		{
			return gameModifierDict[name].isActive;
		}

		public static bool ModifierExists (string name)
		{
			return gameModifierDict.ContainsKey(name);
		}

		[Serializable]
		public class CursorEntry
		{
			public string name;
			public RectTransform rectTrs;

			public virtual void SetAsActive ()
			{
				if (activeCursorEntry != null)
					activeCursorEntry.rectTrs.gameObject.SetActive(false);
				rectTrs.gameObject.SetActive(true);
				activeCursorEntry = this;
			}
		}

		[Serializable]
		public class GameModifier
		{
			public string name;
			public bool isActive;
		}

		[Serializable]
		public class GameScene
		{
			public string name;
			public bool use = true;
		}
	}
}
