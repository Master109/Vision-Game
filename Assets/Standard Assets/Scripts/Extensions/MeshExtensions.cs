using UnityEngine;
using System.Collections.Generic;

namespace Extensions
{
	public static class MeshExtensions
	{
		public static MeshTriangle[] GetTriangles (this KeyValuePair<Mesh, Transform>[] meshesAndTransforms)
		{
			List<MeshTriangle> output = new List<MeshTriangle>();
            for (int i = 0; i < meshesAndTransforms.Length; i ++)
			{
                KeyValuePair<Mesh, Transform> meshAndTransform = meshesAndTransforms[i];
                for (int i2 = 0; i2 < meshAndTransform.Key.vertexCount; i2 ++)
				{
					MeshVertex meshVertex = new MeshVertex(meshAndTransform.Key, meshAndTransform.Value, i2);
					foreach (MeshTriangle meshTriangle in meshVertex.trianglesIAmPartOf)
					{
						bool alreadyContained = false;
						foreach (MeshTriangle outputMeshTriangle in output)
						{
							if (meshTriangle.mesh == outputMeshTriangle.mesh && meshTriangle.point1 == outputMeshTriangle.point1 && meshTriangle.point2 == outputMeshTriangle.point2 && meshTriangle.point3 == outputMeshTriangle.point3)
							{
								alreadyContained = true;
								break;
							}
						}
						if (!alreadyContained)
							output.Add(meshTriangle);
					}
				}
			}
			return output.ToArray();
		}

		// public static MeshTriangle[] GetTrianglesOnTheSameQuad (this Mesh mesh, Transform trs, Mesh otherMesh, Transform otherTrs)
		// {
		// 	List<MeshTriangle> output = new List<MeshTriangle>();
		// 	for (int i = 0; i < mesh.vertexCount; i ++)
		// 	{
		// 		MeshVertex meshVertex = new MeshVertex(mesh, trs, i);
		// 		foreach (MeshTriangle meshTriangle in meshVertex.trianglesIAmPartOf)
		// 		{
		// 			bool alreadyContained = false;
		// 			foreach (MeshTriangle outputMeshTriangle in output)
		// 			{
		// 				if (meshTriangle.mesh == outputMeshTriangle.mesh && meshTriangle.point1 == outputMeshTriangle.point1 && meshTriangle.point2 == outputMeshTriangle.point2 && meshTriangle.point3 == outputMeshTriangle.point3)
		// 				{
		// 					alreadyContained = true;
		// 					break;
		// 				}
		// 			}
		// 			if (!alreadyContained)
		// 				output.Add(meshTriangle);
		// 		}
		// 	}
		// 	for (int i = 0; i < otherMesh.vertexCount; i ++)
		// 	{
		// 		MeshVertex otherMeshVertex = new MeshVertex(otherMesh, otherTrs, i);
		// 		foreach (MeshTriangle otherMeshTriangle in otherMeshVertex.trianglesIAmPartOf)
		// 		{
		// 			bool alreadyContained = false;
		// 			foreach (MeshTriangle outputMeshTriangle in output)
		// 			{
		// 				if (otherMeshTriangle.mesh == outputMeshTriangle.mesh && otherMeshTriangle.point1 == outputMeshTriangle.point1 && otherMeshTriangle.point2 == outputMeshTriangle.point2 && otherMeshTriangle.point3 == outputMeshTriangle.point3)
		// 				{
		// 					alreadyContained = true;
		// 					break;
		// 				}
		// 			}
		// 			if (!alreadyContained)
		// 				output.Add(otherMeshTriangle);
		// 		}
		// 	}
		// 	for (int i = 0; i < output.Count; i ++)
		// 	{
		// 		MeshTriangle meshTriangle = output[i];
		// 		bool isShared = false;
		// 		for (int i2 = 0; i2 < output.Count; i2 ++)
		// 		{
		// 			MeshTriangle otherMeshTriangle = output[i2];
		// 			// if (i != i2 && meshTriangle.mesh != otherMeshTriangle.mesh && meshTriangle.IsOnTheSameQuad(otherMeshTriangle))
		// 			// {
		// 			// 	isShared = true;
		// 			// 	break;
		// 			// }
		// 		}
		// 		if (!isShared)
		// 		{
		// 			output.RemoveAt(i);
		// 			i --;
		// 		}
		// 	}
		// 	return output.ToArray();
		// }

		public struct MeshVertex
		{
			public Mesh mesh;
			public Transform trs;
			public int index;
			public Vector3 point;
			public MeshTriangle[] trianglesIAmPartOf;

			public MeshVertex (Mesh mesh, Transform trs, int index)
			{
				this.mesh = mesh;
				this.trs = trs;
				this.index = index;
				point = trs.TransformPoint(mesh.vertices[index]);
				// point = mesh.vertices[index];
				List<MeshTriangle> _trianglesIAmPartOf = new List<MeshTriangle>();
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					int vertexIndex1 = mesh.triangles[i];
					int vertexIndex2 = mesh.triangles[i + 1];
					int vertexIndex3 = mesh.triangles[i + 2];
					if (vertexIndex1 == index || vertexIndex2 == index || vertexIndex3 == index)
						_trianglesIAmPartOf.Add(new MeshTriangle(mesh, trs, i / 3, vertexIndex1, vertexIndex2, vertexIndex3));
				}
				trianglesIAmPartOf = _trianglesIAmPartOf.ToArray();
			}

			public MeshVertex (Mesh mesh, Transform trs, int index, Vector3 point)
			{
				this.mesh = mesh;
				this.trs = trs;
				this.index = index;
				this.point = trs.TransformPoint(point);
				// this.point = point;
				List<MeshTriangle> _trianglesIAmPartOf = new List<MeshTriangle>();
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					int vertexIndex1 = mesh.triangles[i];
					int vertexIndex2 = mesh.triangles[i + 1];
					int vertexIndex3 = mesh.triangles[i + 2];
					if (vertexIndex1 == index || vertexIndex2 == index || vertexIndex3 == index)
						_trianglesIAmPartOf.Add(new MeshTriangle(mesh, trs, i / 3, vertexIndex1, vertexIndex2, vertexIndex3));
				}
				trianglesIAmPartOf = _trianglesIAmPartOf.ToArray();
			}
		}

		public struct MeshTriangle
		{
			public Mesh mesh;
			public Transform trs;
			public int triangleIndex;
			public int vertexIndex1;
			public int vertexIndex2;
			public int vertexIndex3;
			public Vector3 point1;
			public Vector3 point2;
			public Vector3 point3;

			public MeshTriangle (Mesh mesh, Transform trs, int vertexIndex1, int vertexIndex2, int vertexIndex3)
			{
				this.mesh = mesh;
				this.trs = trs;
				triangleIndex = MathfExtensions.NULL_INT;
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					if (mesh.triangles[i] == vertexIndex1 && mesh.triangles[i + 1] == vertexIndex2 && mesh.triangles[i + 2] == vertexIndex3)
					{
						triangleIndex = i / 3;
						break;
					}
				}
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				point1 = trs.TransformPoint(mesh.vertices[vertexIndex1]);
				point2 = trs.TransformPoint(mesh.vertices[vertexIndex2]);
				point3 = trs.TransformPoint(mesh.vertices[vertexIndex3]);
				// point1 = mesh.vertices[vertexIndex1];
				// point2 = mesh.vertices[vertexIndex2];
				// point3 = mesh.vertices[vertexIndex3];
			}

			public MeshTriangle (Mesh mesh, Transform trs, int triangleIndex, int vertexIndex1, int vertexIndex2, int vertexIndex3)
			{
				this.mesh = mesh;
				this.trs = trs;
				this.triangleIndex = triangleIndex;
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				point1 = trs.TransformPoint(mesh.vertices[vertexIndex1]);
				point2 = trs.TransformPoint(mesh.vertices[vertexIndex2]);
				point3 = trs.TransformPoint(mesh.vertices[vertexIndex3]);
				// point1 = mesh.vertices[vertexIndex1];
				// point2 = mesh.vertices[vertexIndex2];
				// point3 = mesh.vertices[vertexIndex3];
			}

			public MeshTriangle (Mesh mesh, Transform trs, int vertexIndex1, int vertexIndex2, int vertexIndex3, Vector3 point1, Vector3 point2, Vector3 point3)
			{
				this.mesh = mesh;
				this.trs = trs;
				triangleIndex = MathfExtensions.NULL_INT;
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					if (mesh.triangles[i] == vertexIndex1 && mesh.triangles[i + 1] == vertexIndex2 && mesh.triangles[i + 2] == vertexIndex3)
					{
						triangleIndex = i / 3;
						break;
					}
				}
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				this.point1 = trs.TransformPoint(point1);
				this.point2 = trs.TransformPoint(point2);
				this.point3 = trs.TransformPoint(point3);
				// this.point1 = point1;
				// this.point2 = point2;
				// this.point3 = point3;
			}

			public MeshTriangle (Mesh mesh, Transform trs, int triangleIndex, int vertexIndex1, int vertexIndex2, int vertexIndex3, Vector3 point1, Vector3 point2, Vector3 point3)
			{
				this.mesh = mesh;
				this.trs = trs;
				this.triangleIndex = triangleIndex;
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				this.point1 = trs.TransformPoint(point1);
				this.point2 = trs.TransformPoint(point2);
				this.point3 = trs.TransformPoint(point3);
				// this.point1 = point1;
				// this.point2 = point2;
				// this.point3 = point3;
			}

			public bool HasSimilarPoints (MeshTriangle meshTriangle, float maxDistanceSqr)
			{
				return point1.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr && point2.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr && point3.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr;
			}

			public bool HasSimilarPoint (MeshTriangle meshTriangle, float maxDistanceSqr)
			{
				return point1.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr || point2.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr || point3.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3) <= maxDistanceSqr;
			}

			public bool IsOnSamePlane (MeshTriangle meshTriangle)
			{
				Plane plane = new Plane(point1, point2, point3);
				Plane otherPlane = new Plane(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3);
				return plane.Equals(otherPlane) || plane.Equals(otherPlane.flipped);
			}

			// public bool DoesOverlap (MeshTriangle meshTriangle)
			// {
				
			// }

			public void RemoveFromMesh ()
			{
				List<int> triangles = new List<int>();
				triangles.AddRange(mesh.triangles);
				triangles.RemoveRange(triangleIndex * 3, 3);
				mesh.triangles = triangles.ToArray();
			}

			// public bool IsValid ()
			// {

			// }
		}
	}
}