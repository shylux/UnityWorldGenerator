using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island : MonoBehaviour {


	public float WaterLevel = 0.3f; // percentage of terrain below water level (y coord below 0)
	public float PerlinNoiseScale = 6f;
	public int PerlinLevels = 1;

	public Transform palm;
	[MinMaxRange( 0f, 1f )]
	public MinMaxRange PalmPlacementRange;
	public int PalmCount = 100;

	public Transform bush;
	[MinMaxRange( 0f, 1f )]
	public MinMaxRange BushPlacementRange;
	public int BushCount = 100;

	private Terrain terr;
	private TerrainData terrdata;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> indices = new List<int>();
	private float[,] heightmap;
	private float waterHeight;
	private int res; // resolution
	private float Width;
	private float Length;
	private float MaxHeight;
	
	/**
	 * Gets height of point in terrain.
	 * x and z range between 0f and 1f
	 * Output can be any number. The numbers will be normalised in generateHeightMap.  
	 */
	public float getHeight(float x, float z) {
		float hx = 2 * (x - 0.5f);
		float hz = 2 * (z - 0.5f);
		float t = Mathf.Min (1, Mathf.Sqrt (hx*hx + hz*hz));
		float hillH = (1 - t) * (1 - t) * (1 + t) * (1 + t);

		// Perlin factor
		float perlinH = 0f;
		float detailLevel = 1f;
		for (int lv = 1; lv <= PerlinLevels; lv++) {
			float levelAddition = Mathf.PerlinNoise (x * PerlinNoiseScale / detailLevel, z * PerlinNoiseScale / detailLevel) * detailLevel;
			detailLevel /= 2;

			perlinH += levelAddition;
		}
		//float perlinH = Mathf.PerlinNoise (x * PerlinNoiseScale, z * PerlinNoiseScale);

		return hillH * perlinH;
	}

	public void populateTerrain() {
		GameObject trees = new GameObject ();
		trees.name = "Trees";
		trees.transform.parent = terr.transform;
		// Palms
		List<Vector3> possiblePalmPositions = getHeightsInRange (waterHeight + PalmPlacementRange.rangeStart, PalmPlacementRange.rangeEnd);
		for (int i = 0; i < PalmCount; i++) {
			Vector3 pos = possiblePalmPositions[Random.Range(0, possiblePalmPositions.Count)];
			pos.y = terrdata.GetHeight (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.z));
			pos.Scale(new Vector3(Width / res, 1, Length / res));

			Transform apalm = Instantiate(palm, pos + transform.position, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)) as Transform;
			apalm.transform.parent = trees.transform; // don't fill the Hierarchy
		}

		// Bushes
		List<Vector3> possibleBushPositions = getHeightsInRange (waterHeight + BushPlacementRange.rangeStart, BushPlacementRange.rangeEnd);
		for (int i = 0; i < BushCount; i++) {
			Vector3 pos = possibleBushPositions[Random.Range(0, possibleBushPositions.Count)];
			pos.y = terrdata.GetHeight (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.z));
			pos.Scale(new Vector3(Width / res, 1, Length / res));

			Transform abush = Instantiate(bush, pos + transform.position, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)) as Transform;
			abush.transform.parent = trees.transform; // don't fill the Hierarchy
		}
	}
	
	// Use this for initialization
	void Start () {
		terr = GetComponent<Terrain> ();
		terrdata = terr.terrainData;
		res = terrdata.heightmapResolution;
		Width = terrdata.size.x;
		Length = terrdata.size.z;
		MaxHeight = terrdata.size.y;

		heightmap = new float[res, res];
		generateHeightMap ();
		setWaterLevel ();
		terrdata.SetHeights (0, 0, heightmap);
		populateTerrain ();

		float max = 0f;
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				if (max < heightmap [x, z])
					max = heightmap [x, z];
			}
		}
	}

	// internal vector representation (x, h[0..1], z)
	private List<Vector3> getHeightsInRange(float above, float below) {
		List<Vector3> heights = new List<Vector3> ();
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				if (heightmap[x, z] > above && heightmap[x, z] < below)
					heights.Add(new Vector3(x, heightmap[x, z], z));
			}
		}
		return heights;
	}


	public Vector3 getRandomPointInHeightRange(float above, float below) {
		List<Vector3> points = getHeightsInRange (above, below);
		Vector3 point = points [Random.Range (0, points.Count-1)];
		point.y = terrdata.GetHeight (Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.z));
		point.Scale(new Vector3(Width / res, 1, Length / res));
		return point;
	}

	/**
	 * Calls getHeight to generate a hightmap.
	 * Since getHeight returns any number it will scale those that the highest number will be 1f.
	 */
	private void generateHeightMap() {
		float highestPoint = 0f;
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				// float y = Random.Range(0f, MaxHeight);
				heightmap[x, z] = getHeight((float)x/res, (float)z/res);
				if (highestPoint < heightmap [x, z]) highestPoint = heightmap [x, z];
			}
		}
		// Now scale heightmap that it containes values from 0f to 1f
		float scaleFactor = 1f / highestPoint;
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				heightmap [x, z] *= scaleFactor;
			}
		}
	}
	
	private void setWaterLevel() {
		// requires an existing heightmap
		List<float> heights = new List<float> ();
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				heights.Add(heightmap[x, z]);
			}
		}
		heights.Sort ();
		waterHeight = heights[(int)(WaterLevel * heights.Count)];
		Vector3 pos = transform.position;
		pos.y = -waterHeight * MaxHeight;
		transform.position = pos;
	}
	
	private void buildVertices () {
		for (int x = 0; x < res; x++) {
			for (int z = 0; z < res; z++) {
				Vector3 newPoint = new Vector3(Width / res * x,
				                               heightmap[x, z] * MaxHeight,
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
