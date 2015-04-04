using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackGenerator : MonoBehaviour {

	[MinMaxRange( 0f, 1f )]
	public MinMaxRange StartPointHeight;
	public float ElementDistance = 10f;
	public int ElementCount = 100;
	public float AngleVariationDeg = 10f;
	public bool RoundCourse = true;

	Terrain terrain;
	TerrainData terrainData;
	Island island;

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
			if (i > 20 && Vector3.Distance (startPoint, trackPoints [trackPoints.Count - 1]) < 2 * ElementDistance) {
				trackPoints.Add (startPoint);
				break;
			} else if (i > 20 && Vector3.Distance (startPoint, trackPoints [trackPoints.Count - 1]) < 10 * ElementDistance) {
				Vector3 nextPoint = getNearestPoint (startPoint, possible);
				trackPoints.Add (nextPoint);
			} else {
				Vector3 nextPoint = getBestHeight (trackPoints [trackPoints.Count - 1], possible);
				trackPoints.Add (nextPoint);
			}
		}

		for (int i = 0; i < trackPoints.Count-1; i++)
			Debug.DrawLine (trackPoints[i] + transform.position, trackPoints[i+1] + transform.position, Color.red, 60);
	}

	public List<Vector3> getAdjacentPoints(Vector3 origin, float distance, int amount) {
		return getAdjacentPoints (origin, distance, amount, Vector3.zero, 360);
	}

	public List<Vector3> getAdjacentPoints(Vector3 origin, float distance, int amount, Vector3 previousOrigin, float angleVariation) {
		List<Vector3> points = new List<Vector3> ();
		for (int i = 0; i < amount; i++) {
			//Vector3 displacement = Quaternion.AngleAxis (0, Vector3.forward) * (Vector3.forward) * distance;
			//Vector3 adjacentPoint = origin + displacement;
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

	
	// Update is called once per frame
	void Update () {
	
	}
}
