using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Extensions;

namespace BeatKiller
{
	//[ExecuteInEditMode]
	public class AccountManager : MonoBehaviour
	{
		public static Account[] Accounts
		{
			get
			{
				return new Account[0];
			}
		}
		public static Account CurrentlyPlaying
		{
			get
			{
				// if (lastUsedAccountIndex == -1)
					return new Account();
				// return Accounts[lastUsedAccountIndex];
			}
		}
		public static int lastUsedAccountIndex = -1;
	}
}
