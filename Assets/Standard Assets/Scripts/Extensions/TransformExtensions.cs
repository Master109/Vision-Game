using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class TransformExtensions
	{
		public static Transform FindChild (this Transform trs, string childName)
		{
			List<Transform> remainingChildren = new List<Transform>();
			remainingChildren.Add(trs);
			while (remainingChildren.Count > 0)
			{
				foreach (Transform child in remainingChildren[0])
				{
					if (child.name.Equals(childName))
						return child;
					remainingChildren.Add(child);
				}
				remainingChildren.RemoveAt(0);
			}
			return null;
		}
		public static Transform[] FindChildren (this Transform trs, string childName)
		{
			List<Transform> output = new List<Transform>();
			List<Transform> remainingChildren = new List<Transform>();
			remainingChildren.Add(trs);
			while (remainingChildren.Count > 0)
			{
				foreach (Transform child in remainingChildren[0])
				{
					if (child.name.Equals(childName))
						output.Add(child);
					remainingChildren.Add(child);
				}
				remainingChildren.RemoveAt(0);
			}
			return output.ToArray();
		}

		public static Transform GetClosestTransform_2D (this Transform trs, Transform[] transforms)
		{
			// while (transforms.Contains(null))
			// 	transforms = transforms.Remove(null);
			// if (transforms.Length == 0)
			// 	return null;
			// else if (transforms.Length == 1)
			// 	return transforms[0];
			float distance;
			Transform closestTrs = transforms[0];
			float closestDistance = ((Vector2) (trs.position - closestTrs.position)).sqrMagnitude;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform transform = transforms[i];
				distance = ((Vector2) (trs.position - closestTrs.position)).sqrMagnitude;
				if (distance < closestDistance)
				{
					closestTrs = trs;
					closestDistance = distance;
				}
			}
			return closestTrs;
		}

		public static Transform GetClosestTransform_2D (Transform[] transforms, Vector2 position)
		{
			// while (transforms.Contains(null))
			// 	transforms = transforms.Remove(null);
			// if (transforms.Length == 0)
			// 	return null;
			// else if (transforms.Length == 1)
			// 	return transforms[0];
			float distance;
			Transform closestTrs = transforms[0];
			float closestDistance = (position - (Vector2) closestTrs.position).sqrMagnitude;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform trs = transforms[i];
				distance = (position - (Vector2) trs.position).sqrMagnitude;
				if (distance < closestDistance)
				{
					closestTrs = trs;
					closestDistance = distance;
				}
			}
			return closestTrs;
		}

		public static Transform GetClosestTransform_3D (this Transform trs, Transform[] transforms)
		{
			// while (transforms.Contains(null))
			// 	transforms = transforms.Remove(null);
			// if (transforms.Length == 0)
			// 	return null;
			// else if (transforms.Length == 1)
			// 	return transforms[0];
			float distance;
			Transform closestTrs = transforms[0];
			float closestDistance = (trs.position - closestTrs.position).sqrMagnitude;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform transform = transforms[i];
				distance = (trs.position - closestTrs.position).sqrMagnitude;
				if (distance < closestDistance)
				{
					closestTrs = trs;
					closestDistance = distance;
				}
			}
			return closestTrs;
		}

		public static Transform GetClosestTransform_3D (Transform[] transforms, Vector3 position)
		{
			// while (transforms.Contains(null))
			// 	transforms = transforms.Remove(null);
			// if (transforms.Length == 0)
			// 	return null;
			// else if (transforms.Length == 1)
			// 	return transforms[0];
			float distance;
			Transform closestTrs = transforms[0];
			float closestDistance = (position - closestTrs.position).sqrMagnitude;
			for (int i = 1; i < transforms.Length; i ++)
			{
				Transform trs = transforms[i];
				distance = (position - trs.position).sqrMagnitude;
				if (distance < closestDistance)
				{
					closestTrs = trs;
					closestDistance = distance;
				}
			}
			return closestTrs;
		}

		public static Rect GetRect (this Transform trs)
		{
			return Rect.MinMaxRect(trs.position.x - trs.lossyScale.x, trs.position.y - trs.lossyScale.y, trs.position.x + trs.lossyScale.x, trs.position.y + trs.lossyScale.y);
		}

		public static Bounds GetBounds (this Transform trs)
		{
			return new Bounds(trs.position, trs.lossyScale);
		}

		public static bool IsSameOrientationAndScale (this Transform trs, Transform other)
		{
			return trs.position == other.position && trs.rotation == other.rotation && trs.lossyScale == other.lossyScale;
		}

		public static Transform FindEquivalentChild (Transform root1, Transform child1, Transform root2)
		{
			TreeNode<Transform> childTree1 = root1.GetChildTree();
			int[] pathToChild1 = childTree1.GetPathToChild(child1);
			TreeNode<Transform> childTree2 = root2.GetChildTree().GetChildAtPath(pathToChild1);
			return childTree2.Value;
		}

		public static TreeNode<Transform> GetChildTree (this Transform root)
		{
			TreeNode<Transform> output = new TreeNode<Transform>(root);
			List<Transform> remainingChildren = new List<Transform>();
			remainingChildren.Add(root);
			Transform currentTrs;
			while (remainingChildren.Count > 0)
			{
				currentTrs = remainingChildren[0];
				foreach (Transform child in currentTrs)
				{
					output.GetRoot().GetChild(currentTrs).AddChild(child);
					remainingChildren.Add(child);
				}
				remainingChildren.RemoveAt(0);
			}
			return output;
		}

		public static void SetWorldScale (this Transform trs, Vector3 scale)
		{
			trs.localScale = Vector3.one;
			trs.localScale = scale.Divide(trs.lossyScale);
		}

		public static Matrix4x4 GetMatrix (this Transform trs)
		{
			return Matrix4x4.TRS(trs.position, trs.rotation, trs.lossyScale);
		}
	}
}