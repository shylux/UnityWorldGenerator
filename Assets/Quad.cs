using UnityEngine;
using System.Collections;

public class Quad : MonoBehaviour {
	public float Width = 20f;
	public float Length = 10f;

	Vector3[] vertices = new Vector3[4];
	Vector3[] normals = new Vector3[4];
	Vector2[] uv = new Vector2[4];
	int[] indices = new int[6]; // 2 triangles * 3 indices

	// Use this for initialization
	void Start () {

		MeshBuilder builder = new MeshBuilder ();
		builder.vertices.Add (new Vector3 (0f, 0f, 0f));
		builder.vertices.Add (new Vector3 (0f, 0f, Length));
		builder.vertices.Add (new Vector3 (Width, 0f, Length));
		builder.vertices.Add (new Vector3 (Width, 0f, 0f));
		builder.addTriangle (0, 1, 2);
		builder.addTriangle (0, 2, 3);


		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.sharedMesh = builder.CreateMesh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
