using UnityEngine;
using System.Collections.Generic;

namespace Extensions
{
	public static class MeshExtensions
	{
		public static MeshTriangle[] GetSharedTriangles (this Mesh mesh, Mesh otherMesh)
		{
			List<MeshTriangle> output = new List<MeshTriangle>();
			for (int i = 0; i < mesh.vertexCount; i ++)
			{
				MeshVertex meshVertex = new MeshVertex(mesh, i);
				for (int i2 = 0; i2 < otherMesh.vertexCount; i2 ++)
				{
					MeshVertex otherMeshVertex = new MeshVertex(otherMesh, i2);
					foreach (MeshTriangle meshTriangle in meshVertex.trianglesIAmPartOf)
					{
						foreach (MeshTriangle otherMeshTriangle in otherMeshVertex.trianglesIAmPartOf)
						{
							if (meshTriangle.HasSamePoints(otherMeshTriangle) && !output.Contains(meshTriangle))
							{
								output.Add(meshTriangle);
								output.Add(otherMeshTriangle);
							}
						}
					}
				}
			}
			return output.ToArray();
		}

		public struct MeshVertex
		{
			public Mesh mesh;
			public int index;
			public Vector3 point;
			public MeshTriangle[] trianglesIAmPartOf;

			public MeshVertex (Mesh mesh, int index)
			{
				this.mesh = mesh;
				this.index = index;
				point = mesh.vertices[index];
				List<MeshTriangle> _trianglesIAmPartOf = new List<MeshTriangle>();
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					int vertexIndex1 = mesh.triangles[i];
					int vertexIndex2 = mesh.triangles[i + 1];
					int vertexIndex3 = mesh.triangles[i + 2];
					if (vertexIndex1 == index || vertexIndex2 == index || vertexIndex3 == index)
						_trianglesIAmPartOf.Add(new MeshTriangle(mesh, i / 3, vertexIndex1, vertexIndex2, vertexIndex3));
				}
				trianglesIAmPartOf = _trianglesIAmPartOf.ToArray();
			}

			public MeshVertex (Mesh mesh, int index, Vector3 point)
			{
				this.mesh = mesh;
				this.index = index;
				this.point = point;
				List<MeshTriangle> _trianglesIAmPartOf = new List<MeshTriangle>();
				for (int i = 0; i < mesh.triangles.Length; i += 3)
				{
					int vertexIndex1 = mesh.triangles[i];
					int vertexIndex2 = mesh.triangles[i + 1];
					int vertexIndex3 = mesh.triangles[i + 2];
					if (vertexIndex1 == index || vertexIndex2 == index || vertexIndex3 == index)
						_trianglesIAmPartOf.Add(new MeshTriangle(mesh, i / 3, vertexIndex1, vertexIndex2, vertexIndex3));
				}
				trianglesIAmPartOf = _trianglesIAmPartOf.ToArray();
			}
		}

		public struct MeshTriangle
		{
			public Mesh mesh;
			public int triangleIndex;
			public int vertexIndex1;
			public int vertexIndex2;
			public int vertexIndex3;
			public Vector3 point1;
			public Vector3 point2;
			public Vector3 point3;

			public MeshTriangle (Mesh mesh, int vertexIndex1, int vertexIndex2, int vertexIndex3)
			{
				this.mesh = mesh;
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
				point1 = mesh.vertices[vertexIndex1];
				point2 = mesh.vertices[vertexIndex2];
				point3 = mesh.vertices[vertexIndex3];
			}

			public MeshTriangle (Mesh mesh, int triangleIndex, int vertexIndex1, int vertexIndex2, int vertexIndex3)
			{
				this.mesh = mesh;
				this.triangleIndex = triangleIndex;
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				point1 = mesh.vertices[vertexIndex1];
				point2 = mesh.vertices[vertexIndex2];
				point3 = mesh.vertices[vertexIndex3];
			}

			public MeshTriangle (Mesh mesh, int vertexIndex1, int vertexIndex2, int vertexIndex3, Vector3 point1, Vector3 point2, Vector3 point3)
			{
				this.mesh = mesh;
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
				this.point1 = point1;
				this.point2 = point2;
				this.point3 = point3;
			}

			public MeshTriangle (Mesh mesh, int triangleIndex, int vertexIndex1, int vertexIndex2, int vertexIndex3, Vector3 point1, Vector3 point2, Vector3 point3)
			{
				this.mesh = mesh;
				this.triangleIndex = triangleIndex;
				this.vertexIndex1 = vertexIndex1;
				this.vertexIndex2 = vertexIndex2;
				this.vertexIndex3 = vertexIndex3;
				this.point1 = point1;
				this.point2 = point2;
				this.point3 = point3;
			}

			public bool HasSamePoints (MeshTriangle meshTriangle)
			{
				return Mathf.Approximately(point1.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3), 0) && Mathf.Approximately(point2.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3), 0) && Mathf.Approximately(point3.GetDistanceSqrToClosestPoint(meshTriangle.point1, meshTriangle.point2, meshTriangle.point3), 0);
			}

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