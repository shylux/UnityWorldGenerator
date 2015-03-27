using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class HeightGenerator : MonoBehaviour {
	public float Width = 10f;
	public float Length = 10f;
	public float MaxHeight = 5f;
	public int res = 20; // resolution
	public float WaterLevel = 0.2f; // percentage of terrain below water level (y coord below 0)

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> indices = new List<int>();
	private float[][] heightmap;

	/**
	 * Gets height of point in terrain.
	 * x and z are fractions
	 */
	public abstract float getHeight(float x, float z);

	// Use this for initialization
	void Start () {
		heightmap = new float[res][];
		for (int i = 0; i < res; i++)
			heightmap [i] = new float[res];
		generateHeightMap ();
		setWaterLevel ();

		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.sharedMesh = build ();
		filter.GetComponent<Renderer>().sharedMaterial.color = Color.gray;
	}

	private void generateHeightMap() {
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				// float y = Random.Range(0f, MaxHeight);
				heightmap[x][z] = getHeight((float)x/res, (float)z/res);
			}
		}
	}

	private void setWaterLevel() {
		// requires an existing heightmap
		List<float> heights = new List<float> ();
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				heights.Add(heightmap[x][z]);
			}
		}
		heights.Sort ();
		float waterHeight = heights[(int)(WaterLevel * heights.Count)];
		Vector3 pos = transform.position;
		pos.y = -waterHeight;
		transform.position = pos;
	}

	private void buildVertices () {
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				Vector3 newPoint = new Vector3(Width / res * x,
				                               heightmap[x][z],
				                               Length / res * z);
				int index = addVertex(newPoint);
				int column = index % res;
				int row = (index - column) / res;
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
	}

	private Mesh build() {
		buildVertices ();

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = indices.ToArray ();
		
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		
		return mesh;
	}

	// Update is called once per frame
	void Update () {
	
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
	private int addVertex(Vector3 v) {
		vertices.Add (v);
		return vertices.Count - 1;
	}
	private int addVertex(float a, float b, float c) {
		return addVertex (new Vector3 (a, b, c));
	}
	private int getIndex(int row, int column) {
		return row * res + column;
	}
}
