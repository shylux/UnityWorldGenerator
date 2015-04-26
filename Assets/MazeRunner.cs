using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class MazeRunner : MonoBehaviour {
    public Transform Car;
    public Text elapsedTimeLabel;

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
        elapsedTimeLabel.text = formatDeltaTime(Time.time - startTime);
	}

	void runnerFinished(Collider c) {
		Debug.Log ("Time: "+(Time.time - startTime));
        enabled = false;
	}

    string formatDeltaTime(float time) {
        float fraction = time % 1;
        int seconds = (int)(time - fraction) % 60;
        int minutes = (int)(time - fraction - seconds) / 60;
        return String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, Mathf.Round(fraction*10));
    }
}
