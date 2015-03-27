using UnityEngine;
using System.Collections;

public class Hill : HeightGenerator {

	public override float getHeight (float x, float z) {
		x = 2 * (x - 0.5f);
		z = 2 * (z - 0.5f);
		float t = Mathf.Min (1, Mathf.Sqrt (x*x + z*z));
		float h = (1 - t) * (1 - t) * (1 + t) * (1 + t);
		return h * MaxHeight;
	}
}
