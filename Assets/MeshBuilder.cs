using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBuilder {

	public List<Vector3> vertices = new List<Vector3>();
	private List<int> indices = new List<int>();

	public void addTriangle(int i0, int i1, int i2) {
		indices.Add (i0);
		indices.Add (i1);
		indices.Add (i2);
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = indices.ToArray ();

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		return mesh;
	}
}
