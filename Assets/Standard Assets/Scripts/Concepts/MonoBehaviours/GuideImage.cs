using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GuideImage
{
	public Texture2D texture;
	public ColorType defaultColorType;
	public ColorType[] colorTypes = new ColorType[0];

	// Returns whether the guide image has been applied successfully
	public virtual bool Apply (object obj)
	{
		return false;
	}

	// Returns whether the guide image has been applied successfully
	public virtual IEnumerator ApplyRoutine (object obj)
	{
		yield return false;
	}

	// Returns a copy of the object after the guide image has been applied
	public virtual object GetResultOfApply (object obj)
	{
		return obj.DeepCopyByExpressionTree();
	}

	// Returns a copy of the object after the guide image has been applied
	public virtual IEnumerator GetResultOfApplyRoutine (object obj)
	{
		yield return obj.DeepCopyByExpressionTree();
	}

	[Serializable]
	public struct ColorType
	{
		public Color color;
		public string name;
		public string description;
	}
}