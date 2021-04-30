#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor;

namespace AmbitiousSnake
{
	public class DeleteProBuilderMeshSharedFaces : EditorScript
	{
		public ProBuilderMesh proBuilderMesh;
	
		void Start ()
		{
			if (!Application.isPlaying)
			{
				if (proBuilderMesh == null)
					proBuilderMesh = GetComponent<ProBuilderMesh>();
				return;
			}
		}

		public override void Do ()
		{
			if (proBuilderMesh != null)
				_Do (proBuilderMesh);
		}

		public static void _Do (ProBuilderMesh proBuilderMesh)
		{
			while (true)
			{
				bool removedAFace = false;
				for (int i = 0; i < proBuilderMesh.faceCount; i ++)
				{
					Face face = proBuilderMesh.faces[i];
					for (int i2 = i + 1; i2 < proBuilderMesh.faceCount; i2 ++)
					{
						Face face2 = proBuilderMesh.faces[i2];
						bool isSharedFace = true;
						for (int i3 = 0; i3 < face.indexes.Count; i3 ++)
						{
							int index = face.indexes[i3];
							bool isSharedIndex = false;
							for (int i4 = 0; i4 < face2.indexes.Count; i4 ++)
							{
								int index2 = face2.indexes[i4];
								if (proBuilderMesh.positions[index] == proBuilderMesh.positions[index2])
								{
									isSharedIndex = true;
									break;
								}
							}
							if (!isSharedIndex)
							{
								isSharedFace = false;
								break;
							}
						}
						if (isSharedFace)
						{
							proBuilderMesh.DeleteFaces(new int[2] { i, i2 });
							removedAFace = true;
						}
					}
				}
				if (!removedAFace)
					break;
			}
			proBuilderMesh.ToMesh();
			proBuilderMesh.Refresh();
		}

		[MenuItem("Tools/Delete shared faces of selected ProBuilderMeshes")]
		static void _Do ()
		{
			Transform[] selectedTransforms = Selection.transforms;
			for (int i = 0; i < selectedTransforms.Length; i ++)
			{
				Transform selectedTrs = selectedTransforms[i];
				ProBuilderMesh proBuilderMesh = selectedTrs.GetComponent<ProBuilderMesh>();
				if (proBuilderMesh != null)
					_Do (proBuilderMesh);
			}
		}
	}
}
#else
namespace AmbitiousSnake
{
	public class DeleteProBuilderMeshSharedFaces : EditorScript
	{
	}
}
#endif