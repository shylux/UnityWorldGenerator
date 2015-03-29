using UnityEngine;
using System.Collections;

public class WaterCollision : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Debug.Log("Test");
		Destroy(other.gameObject);
	}
}
