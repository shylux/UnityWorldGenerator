using UnityEngine;
using System.Collections;

public class MazeRunner : MonoBehaviour {
    public Transform Car;

	// Use this for initialization
	void Start () {
	    Car.position = transform.Find("Start").position;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
