using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBuilder : MonoBehaviour {
	public float Width = 10f;
	public float Length = 10f;
	public float MaxHeight = 5f;
	public int resolution = 10;
	
	private List<Vector3> vertices = new List<Vector3> ();
	private List<int> indices = new List<int> ();

	void Start () {
		for (int x = 0; x < resolution; x++) {
			for (int z = 0; z < resolution; z++) {
				float y = Random.Range(0f, MaxHeight);
				Vector3 newPoint = new Vector3(Width / resolution * x,
				                               y,
				                               Length / resolution * z);
				int index = addVertex(newPoint);
				int column = index % resolution;
				int row = (index - column) / resolution;
				if (row > 0 && column > 0) {
					addTriangle(getIndex(row-1, column-1),
					            getIndex(row-1, column),
					            getIndex(row, column));
					addTriangle(getIndex(row-1, column-1),
					            getIndex(row, column),
					            getIndex(row, column-1));
				}
			}
		}




		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.sharedMesh = build ();
	}

	void Update() {
	}

	private int getIndex(int row, int column) {
		return row * resolution + column;
	}
	private int addVertex(Vector3 v) {
		vertices.Add (v);
		return vertices.Count - 1;
	}
	private int addVertex(float a, float b, float c) {
		return addVertex (new Vector3 (a, b, c));
	}


	public void addTriangle(Vector3 a, Vector3 b, Vector3 c) {
		int i = addVertex (a);
		int j = addVertex (b);
		int k = addVertex (c);
		addTriangle (i, j, k);
	}

	private void addTriangle(int i0, int i1, int i2) {
		indices.Add (i0);
		indices.Add (i1);
		indices.Add (i2);
	}
	private void addQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		int i = addVertex (a);
		int j = addVertex (b);
		int k = addVertex (c);
		int l = addVertex (d);
		addTriangle (i, j, k);
		addTriangle (i, k, l);
	}

	public Mesh build() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = indices.ToArray ();
		
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		
		return mesh;
	}
}
