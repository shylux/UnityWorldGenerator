using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CollideNotifier : MonoBehaviour {
	public class CollideEvent : UnityEvent<Collider> {};

	public CollideEvent onCollide = new CollideEvent();

	void OnTriggerEnter(Collider c) {
		onCollide.Invoke (c);
	}
}
