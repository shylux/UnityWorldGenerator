using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MazeRunner : MonoBehaviour {
    public Transform Car;

	float startTime;

	// Use this for initialization
	void Start () {
	    Car.position = transform.Find("Start").position;

		startTime = Time.time;

		// Register event
	    GameObject finish = GameObject.FindGameObjectsWithTag ("Finish")[0];
		CollideNotifier notif = finish.GetComponent<CollideNotifier> ();
		notif.onCollide.AddListener (runnerFinished);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void runnerFinished(Collider c) {
		Debug.Log ("Time: "+(Time.time - startTime));
	}
}
