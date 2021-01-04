using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Extensions
{
	public static class GameObjectExtensions
	{
		public static GameObject FindGameObjectWithComponentInParents (Transform child, string componentName)
		{
			while (child != null)
			{
				child = child.parent;
				if (child != null)
				{
					if (child.GetComponent(componentName) != null)
						return child.gameObject;
				}
			}
			return null;
		}
		
		public static GameObject FindGameObjectWithComponentInParents (Transform child, Type componentType)
		{
			return FindGameObjectWithComponentInParents(child, componentType.Name);
		}

		public static T InsertParentWithComponent<T> (this Transform trs) where T : Component
		{
			return trs.InsertParent().gameObject.AddComponent<T>();
		}

#if UNITY_EDITOR
		public static T InsertParentWithComponentAndRegisterUndo<T> (this Transform trs) where T : Component
		{
			return trs.InsertParentAndRegisterUndo().gameObject.AddComponent<T>();
		}
#endif
	}
}