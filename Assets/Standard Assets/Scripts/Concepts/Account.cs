using System;
using Extensions;
using UnityEngine;

namespace BeatKiller
{
	[Serializable]
	public class Account : IDefaultable<Account>, ISaveableAndLoadable
	{
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
		// public string Name
		// {
		// 	get
		// 	{
		// 		return name;
		// 	}
		// 	set
		// 	{
		// 		name = value;
		// 	}
		// }
		public int index;
		// [SaveAndLoadValue(true)]
		// public string name;
		public string Name
		{
			get
			{
				return PlayerPrefs.GetString("Account " + index + " name", "");
			}
			set
			{
				PlayerPrefs.SetString("Account " + index + " name", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public float playTime;
		public float PlayTime
		{
			get
			{
				return PlayerPrefs.GetFloat("Account " + index + " play time", 0);
			}
			set
			{
				PlayerPrefs.SetFloat("Account " + index + " play time", value);
			}
		}
		public int MostRecentlyUsedSaveEntryIndex
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " most recently used save entry index", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " most recently used save entry index", value);
			}
		}
		public int LastSaveEntryIndex
		{
			get
			{
				return PlayerPrefs.GetInt("Account " + index + " last save entry index", 0);
			}
			set
			{
				PlayerPrefs.SetInt("Account " + index + " last save entry index", value);
			}
		}
		// [SaveAndLoadValue(true)]
		// public bool isPlaying;
		
		public Account GetDefault ()
		{
			Account account = this;
			account.Name = "";
			account.PlayTime = 0;
			// account.isPlaying = false;
			return account;
		}
	}
}