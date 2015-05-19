using UnityEngine;
using System.Collections;

public class HoverMotor : MonoBehaviour {
	
	public float speed = 90f;
	public float turnSpeed = 5f;
	public float hoverForce = 65f;
	public float hoverHeight = 3.5f;
	private float powerInput;
	private float turnInput;
	private Rigidbody carRigidbody;

	private Vector3[] hoverEngines;
	
	
	void Awake () {
		carRigidbody = GetComponent <Rigidbody>();

//		Quaternion back = Quaternion.AngleAxis (150, Vector3.right);
//		Vector3 backleft = back * Quaternion.AngleAxis (45, Vector3.up) * Vector3.forward * hoverHeight;
//		Vector3 backright = back * Quaternion.AngleAxis (-45, Vector3.up) * Vector3.forward * hoverHeight;
//		Vector3 forward = Quaternion.AngleAxis (20, Vector3.right) * Vector3.forward * hoverHeight * 1.1f;
//		Vector3 turnme = Vector3.up * 5;

//		hoverEngines = new Vector3[] {backleft, backright, forward, turnme};
	}
	
	void Update () {
		powerInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");
	}
	
	void FixedUpdate() {
		Ray ray = new Ray (transform.position, -transform.up * hoverHeight);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit)) {
			float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
			carRigidbody.AddForce (appliedHoverForce, ForceMode.Acceleration);
		}

		carRigidbody.AddRelativeForce(Vector3.forward * powerInput * speed);
		carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);
	}
}