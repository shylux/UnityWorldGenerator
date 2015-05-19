using UnityEngine;
using System.Collections;

public class HoverEngine : MonoBehaviour {
	public float hoverHeight = 3.5f;
	public float hoverForce = 60f;

	public Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		if (rigidbody == null)
			rigidbody = transform.parent.GetComponent <Rigidbody> ();
	}
	
	void FixedUpdate() {
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit;
		Debug.DrawLine(transform.position, transform.position+(transform.forward*2));
		if (Physics.Raycast(ray, out hit, hoverHeight)) {
			float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = transform.up * proportionalHeight * hoverForce;
			rigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
		}
	}
}
