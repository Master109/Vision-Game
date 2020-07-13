using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeatKiller;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DialogAndStory
{
	//[ExecuteInEditMode]
	public class Conversation : MonoBehaviour
	{
		public Transform trs;
		public Dialog[] dialogs = new Dialog[0];
		public Coroutine updateRoutine;
		[HideInInspector]
		public Dialog currentDialog;
		
		public virtual void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				if (trs == null)
					trs = GetComponent<Transform>();
				EditorApplication.update += DoEditorUpdate;
				return;
			}
			else
				EditorApplication.update -= DoEditorUpdate;
#endif
		}
		
		public virtual IEnumerator UpdateRoutine ()
		{
			if (DialogManager.currentConversation != null)
				yield return DialogManager.currentConversation.updateRoutine;
			DialogManager.currentConversation = this;
			foreach (Dialog dialog in dialogs)
			{
				currentDialog = dialog;
				GameManager.GetSingleton<DialogManager>().StartDialog (dialog);
				yield return new WaitUntil(() => (!dialog.IsActive));
				GameManager.GetSingleton<DialogManager>().EndDialog (dialog);
			}
			yield break;
		}
		
		public virtual void OnDisable ()
		{
#if UNITY_EDITOR
			EditorApplication.update -= DoEditorUpdate;
			if (!Application.isPlaying)
				return;
#endif
			if (updateRoutine != null)
			{
				StopCoroutine (updateRoutine);
				updateRoutine = null;
				foreach (Dialog dialog in dialogs)
					dialog.gameObject.SetActive(false);
			}
		}
		
#if UNITY_EDITOR
		public virtual void DoEditorUpdate ()
		{
			if (dialogs.Length == 0)
				dialogs = GetComponentsInChildren<Dialog>();
			foreach (Dialog dialog in dialogs)
				dialog.conversation = this;
		}
#endif
	}
}