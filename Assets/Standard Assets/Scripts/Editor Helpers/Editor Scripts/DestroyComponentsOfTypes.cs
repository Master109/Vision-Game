#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using Extensions;
using System;

public class DestroyComponentsOfTypes : EditorScript
{
	public Component[] components;
	public Type[] types;

	public override void OnEnable ()
	{
		base.OnEnable ();
		Do ();
	}

	public virtual void Do ()
	{
		int indexOfNull;
		do
		{
			indexOfNull = components.IndexOf(null);
			if (indexOfNull != -1)
				components = components.RemoveAt(indexOfNull);
			else
				break;
		} while (true);
		List<Type> _types = new List<Type>();
		for (int i = 0; i < components.Length; i ++)
		{
			Component component = components[i];
			_types.Add(component.GetType());
		}
		types = _types.ToArray();
		List<Component> _components = new List<Component>(GetComponents<Component>());
		for (int i = 0; i < types.Length; i ++)
		{
			Type type = types[i];
			for (int i2 = 0; i2 < _components.Count; i2 ++)
			{
				Component _component = _components[i2];
				if (_component.GetType() == type)
					DestroyImmediate(_component);
			}
		}
		components = new Component[0];
	}
}
#else
public class DestroyComponentsOfTypes : EditorScript
{
}
#endif