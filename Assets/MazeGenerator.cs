using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // used for Sum of array

public class MazeGenerator: MonoBehaviour {
	Maze2D maze;

	public int width = 10;
	public int length = 10;
	public Transform WallPrefab;
	float unitSize; // length of one sqare containing a cell and two walls

	List<List<Maze2D.Cell>> cellGroups = new List<List<Maze2D.Cell>>();

	void Start() {
		maze = new Maze2D (width, length);
		foreach (Maze2D.Wall wall in maze.Walls())
			wall.set (true);

		RandomizedKruskal ();
		BuildMazeInSzene ();
	}

	void BuildMazeInSzene() {
		Vector3 size = getBounds(WallPrefab);
		unitSize = size.x + size.z;
		foreach (Maze2D.Wall wall in maze.Walls()) {
			if (!wall.get()) continue;
			Vector3 offset, pos = new Vector3 (wall.x * unitSize, 0, Mathf.Min(wall.y/2) * unitSize);
			Quaternion rot;
			if (wall.y % 2 == 0) { // horizontal
				rot = Quaternion.identity;
				offset = new Vector3 (size.z, 0, size.z / 2);
			} else { // vertical
				rot = Quaternion.AngleAxis(-90, Vector3.up);
				offset = new Vector3 (size.z / 2, 0, size.z);
			}
			Transform wallObj = Instantiate (WallPrefab, pos + offset + transform.position, rot) as Transform;
			wallObj.transform.parent = transform.parent;
		}
		// Debug.Log (size.x + " " + size.y + " " + size.z);
	}

	Vector3 getBounds(Transform parent) {
		Bounds bounds = parent.GetComponent<Renderer>().bounds;
		foreach (Transform child in parent.transform) {
			bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
		}
		return bounds.size;
	}

	private void RandomizedKruskal() {
		foreach (Maze2D.Cell c in maze.Cells()) {
			cellGroups.Add(new List<Maze2D.Cell> {c});
		}
		foreach (Maze2D.Wall w in maze.Walls()) {

		}
		foreach (Maze2D.Cell c in maze.Cells()) {
			foreach (List<Maze2D.Cell> lst in cellGroups) {
				if (lst.Contains(c))
					Debug.Log(c);
			}
		}
	}
}
