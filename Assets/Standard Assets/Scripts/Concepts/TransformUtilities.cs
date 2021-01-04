using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Extensions;
using VisionGame;
#endif

public static class TransformUtilities
{
#if UNITY_EDITOR
		[MenuItem("Tools/Insert Parent(s) With Freeze Transform %#p")]
		public static void InsertParentsWithFreezeTransform ()
		{
			for (int i = 0; i < Selection.transforms.Length; i ++)
			{
				Transform trs = Selection.transforms[i];
				FreezeTransform freezeTrs = trs.InsertParentWithComponentAndRegisterUndo<FreezeTransform>();
			}
		}
		
		[MenuItem("Tools/Insert Parent With Freeze Transform %#&p")]
		public static void InsertParentWithFreezeTransform ()
		{
			FreezeTransform freezeTrs = Selection.transforms[Selection.transforms.Length - 1].InsertParentWithComponentAndRegisterUndo<FreezeTransform>();
			for (int i = 0; i < Selection.transforms.Length - 1; i ++)
			{
				Transform trs = Selection.transforms[i];
				Undo.SetTransformParent(trs, freezeTrs.trs, "Insert parent with freeze transform");
			}
		}
#endif
}