using UnityEngine;
using System.Collections;

public class MazeOverview : MonoBehaviour {

	public GameObject maze;

	private MazeGenerator mg;
	private SmoothFollow sf;

	// Use this for initialization
	void Start () {
		mg = maze.GetComponent<MazeGenerator> ();
		sf = GetComponent<SmoothFollow> ();

		float width = mg.getWidth ();
		float length = mg.getLength ();

		float height = Mathf.Max (width, length);
		transform.position = maze.transform.position + new Vector3 (width/2, height, length/2);
		transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
	}

	void Update() {
		if (Input.GetKeyDown ("space")) {
			sf.enabled = true;
			sf.target.GetComponent<HoverMotor>().enabled = true;
			enabled = false;
		}
	}
}
