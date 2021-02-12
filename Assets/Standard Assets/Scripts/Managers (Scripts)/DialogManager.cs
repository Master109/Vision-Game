using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogAndStory
{
	public class DialogManager : SingletonMonoBehaviour<DialogManager>
	{
		public static Conversation currentConversation;
		public static Dialog currentDialog;

		public virtual void StartDialog (Dialog dialog)
		{
			dialog.onStartedEvent.Do ();
			dialog.gameObject.SetActive(true);
			currentDialog = dialog;
		}
		
		public virtual void EndDialog (Dialog dialog)
		{
			if (dialog == null)
				return;
			if (!dialog.isFinished)
				dialog.onLeftWhileTalkingEvent.Do ();
			dialog.gameObject.SetActive(false);
			if (currentDialog == dialog)
				currentDialog = null;
		}
		
		public virtual void StartConversation (Conversation conversation)
		{
			conversation.updateRoutine = conversation.StartCoroutine(conversation.UpdateRoutine ());
			currentConversation = conversation;
		}

		public virtual void EndConversation (Conversation conversation)
		{
			EndDialog (conversation.currentDialog);
			if (conversation.updateRoutine != null)
			{
				conversation.StopCoroutine(conversation.updateRoutine);
				if (conversation == currentConversation)
					currentConversation = null;
			}
			conversation.updateRoutine = null;
		}
	}
}