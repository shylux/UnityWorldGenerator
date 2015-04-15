using UnityEngine;
using System.Collections;

public class Fatality : MonoBehaviour {

	public void OnTriggerEnter(Collider other) {
		Debug.Log ("Triggered" + other);
		Destroy (gameObject);
	}
}
