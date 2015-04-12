using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class StartCountdown : MonoBehaviour {
	public int CountMax = 3;
	private int _countDown;

	private CarController CarScript;

	// Use this for initialization
	void Start () {
		CarScript = GetComponent<CarController> ();
		print ("Begin Start" + Time.time);
		StartCoroutine (GameStart ());
		print ("End Start" + Time.time);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator GameStart() {
		CarScript.enabled = false;
		for (_countDown = CountMax; _countDown > 0; _countDown--) {
			yield return new WaitForSeconds(1);
			print("WaitForSeconds"+Time.time);
		}
		CarScript.enabled = true;
	}
}
