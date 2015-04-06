using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackGenerator : MonoBehaviour {

	[MinMaxRange( 0f, 1f )]
	public MinMaxRange StartPointHeight;
	public float ElementDistance = 20f;
	public int ElementCount = 150;
	public float AngleVariationDeg = 10f;

	public float TrackWidth = 20f;
	public float TrackLift = 1f;
	public int SlopeDeg = 20;
	public float SlopeSize = 2f;
	public Material TrackMaterial;
	public Material SlopeMaterial;

	public bool RoundCourse = true;
	public int RoundCourseSnapDistance = 2;
	public int RoundCourseApproachDistance = 15;

	Terrain terrain;
	TerrainData terrainData;
	Island island;

	GameObject track;
	List<Vector3> vertices;
	List<Vector2> uvs;

	private List<Vector3> trackPoints = new List<Vector3> ();

	// Use this for initialization
	void Start () {
		terrain = GetComponent<Terrain> ();
		terrainData = terrain.terrainData;
		island = GetComponent<Island> ();

		Vector3 startPoint = island.getRandomPointInHeightRange(StartPointHeight.rangeStart, StartPointHeight.rangeEnd);

		Debug.DrawLine (startPoint + transform.position , new Vector3(1000, 1000, 1000), Color.red, 30);
		List<Vector3> secondPoints = getAdjacentPoints (startPoint, ElementDistance, 12);
		//foreach (Vector3 v in secondPoints) Debug.DrawLine (startPoint + transform.position, v + transform.position, Color.magenta, 30);
		Vector3 secondPoint = getBestHeight (startPoint, secondPoints);
		trackPoints.Add (startPoint);
		trackPoints.Add (secondPoint);

		for (int i = 0; i < ElementCount; i++) {
			List<Vector3> possible = getAdjacentPoints (trackPoints [trackPoints.Count - 1], ElementDistance, 12, trackPoints [trackPoints.Count - 2], AngleVariationDeg);
			// foreach (Vector3 v in possible) Debug.DrawLine (trackPoints[trackPoints.Count - 1] + transform.position, v + transform.position, Color.yellow, 30);
			if (RoundCourse && i > 20 && Vector3.Distance (startPoint, trackPoints [trackPoints.Count - 1]) < RoundCourseSnapDistance * ElementDistance && i % 2 == 0) { // complete course
				trackPoints.Add (startPoint);
				break;
			} else if (RoundCourse && i > 20 && Vector3.Distance (startPoint, trackPoints [trackPoints.Count - 1]) < RoundCourseApproachDistance * ElementDistance) { // move toward start of course
				Vector3 nextPoint = getNearestPoint (startPoint, possible);
				trackPoints.Add (nextPoint);
			} else {
				Vector3 nextPoint = getBestHeight (trackPoints [trackPoints.Count - 1], possible);
				trackPoints.Add (nextPoint);
			}
		}

		// Draw Track
		for (int i = 0; i < trackPoints.Count-1; i++)
			Debug.DrawLine (trackPoints[i] + transform.position, trackPoints[i+1] + transform.position, Color.red, 60);

		renderTrack ();
	}

	public void renderTrack() {
		track = new GameObject ();
		track.name = "Track";
		track.transform.parent = terrain.transform;
		track.transform.localPosition = Vector3.zero;
		MeshFilter meshFilter = track.AddComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		MeshRenderer meshRenderer = track.AddComponent<MeshRenderer> ();
		meshRenderer.material = TrackMaterial;
		vertices = new List<Vector3> ();
		List<int> trackTr = new List<int> ();
		List<int> sideRTr = new List<int> ();
		List<int> sideLTr = new List<int> ();
		uvs = new List<Vector2> ();


		// Get the two points left and right
		for (int i = 0; i < trackPoints.Count-1; i++) {
			Vector3 direction = (trackPoints [i+1] - trackPoints [i]).normalized; // the forward direction of the track
			Vector3 rightDirection = Quaternion.AngleAxis (90, Vector3.up) * direction * TrackWidth / 2;
			Vector3 leftDirection = Quaternion.AngleAxis (270, Vector3.up) * direction * TrackWidth / 2;
			// left and right border of the road
			Vector3 right = trackPoints [i] + rightDirection;
			Vector3 left = trackPoints [i] + leftDirection;

			// Correct height so the track is not inside the terrain
			float rightHeight = terrain.SampleHeight (right + transform.position);
			float leftHeight = terrain.SampleHeight (left + transform.position);
			float correctHeight = Mathf.Max (rightHeight, leftHeight) + TrackLift;
			right.y = correctHeight;
			left.y = correctHeight;

			// Connect the road to the ground with a slope
			Vector3 sidewayR = right + Quaternion.AngleAxis (-SlopeDeg, direction) * rightDirection * SlopeSize;
			Vector3 sidewayL = left + Quaternion.AngleAxis (SlopeDeg, direction) * leftDirection * SlopeSize;
			Debug.DrawLine (sidewayR + transform.position, right + transform.position, Color.yellow, 60);
			Debug.DrawLine (sidewayL + transform.position, left + transform.position, Color.yellow, 60);

			vertices.Add (sidewayR);
			vertices.Add (sidewayL);
			vertices.Add (right);
			vertices.Add (left);

			// map edges of texture to the edges of each road segment
			if (i % 2 == 0) {
				uvs.Add (new Vector2 (0, 0));
				uvs.Add (new Vector2 (1, 0));
				uvs.Add (new Vector2 (1, 0));
				uvs.Add (new Vector2 (0, 0));
			} else {
				uvs.Add (new Vector2 (0, 1));
				uvs.Add (new Vector2 (1, 1));
				uvs.Add (new Vector2 (1, 1));
				uvs.Add (new Vector2 (0, 1));
			}

			if (i > 0) { // build triangles for new segment
				addTriangle (trackTr, vertices.Count - 5, vertices.Count - 1, vertices.Count - 6);
				addTriangle (trackTr, vertices.Count - 6, vertices.Count - 1, vertices.Count - 2);
				addTriangle (sideRTr, vertices.Count - 6, vertices.Count - 2, vertices.Count - 8);
				addTriangle (sideRTr, vertices.Count - 8, vertices.Count - 2, vertices.Count - 4);
				addTriangle (sideLTr, vertices.Count - 7, vertices.Count - 3, vertices.Count - 5);
				addTriangle (sideLTr, vertices.Count - 5, vertices.Count - 3, vertices.Count - 1);
			}
			if (i == trackPoints.Count-2) { // connect end to start
				addTriangle (trackTr, vertices.Count - 1, 3, vertices.Count - 2);
				addTriangle (trackTr, vertices.Count - 2, 3, 2);
				addTriangle (sideRTr, vertices.Count - 2, 2, vertices.Count - 4);
				addTriangle (sideRTr, vertices.Count - 4, 2, 0);
				addTriangle (sideLTr, vertices.Count - 3, 1, vertices.Count - 1);
				addTriangle (sideLTr, vertices.Count - 1, 1, 3);
			}

			Debug.DrawLine (left + transform.position, right + transform.position, Color.yellow, 60);
		}
		mesh.Clear ();
		mesh.subMeshCount = 3;
		mesh.vertices = vertices.ToArray ();
		mesh.SetTriangles (trackTr.ToArray (), 0);
		mesh.SetTriangles (sideRTr.ToArray (), 1);
		mesh.SetTriangles (sideLTr.ToArray (), 2);
		mesh.uv = uvs.ToArray ();

		meshRenderer.materials = new Material[] { TrackMaterial, SlopeMaterial, SlopeMaterial };
		MeshCollider meshCollider = track.AddComponent<MeshCollider> ();
	}

	public List<Vector3> getAdjacentPoints(Vector3 origin, float distance, int amount) {
		return getAdjacentPoints (origin, distance, amount, Vector3.zero, 360);
	}

	public List<Vector3> getAdjacentPoints(Vector3 origin, float distance, int amount, Vector3 previousOrigin, float angleVariation) {
		List<Vector3> points = new List<Vector3> ();
		for (int i = 0; i < amount; i++) {
			Vector3 adjacentPoint = origin + Quaternion.AngleAxis (angleVariation/amount * i - angleVariation/2, Vector3.up) * (origin - previousOrigin).normalized * distance;
			adjacentPoint.y = terrain.SampleHeight (new Vector3(adjacentPoint.x, 0, adjacentPoint.z)+transform.position);
			//Debug.Log ("x:"+adjacentPoint.x+" z:" +adjacentPoint.z+" y:" + adjacentPoint.y);
			points.Add (adjacentPoint);
		}
		return points;
	}

	public Vector3 getBestHeight(Vector3 origin, List<Vector3> points) {
		Vector3 bestPoint = Vector3.zero;
		float bestHeight = float.MaxValue;
		foreach (Vector3 point in points) {
			float hDifference = Mathf.Abs (origin.y - point.y);
			if (hDifference < bestHeight) {
				bestHeight = hDifference;
				bestPoint = point;
			}
		}
		return bestPoint;
	}

	public Vector3 getNearestPoint(Vector3 origin, List<Vector3> points) {
		Vector3 bestPoint = Vector3.zero;
		float bestDistance = float.MaxValue;
		foreach (Vector3 point in points) {
			float dist = Vector3.Distance (origin, point);
			if (dist < bestDistance) {
				bestDistance = dist;
				bestPoint = point;
			}
		}
		return bestPoint;
	}

	private void addTriangle(List<int> mesh, int i0, int i1, int i2) {
		mesh.Add (i0);
		mesh.Add (i1);
		mesh.Add (i2);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionStay(Collision collision) {
		Debug.Log ("Collision: " + collision);
	}
}
