using UnityEngine;
using System.Collections;

public class Gauss : HeightGenerator {
	public float sigma = 1f;

	public override float getHeight (float x, float z) {
		// distance to center of extrema
		float d = Mathf.Abs (x - 0.5f) + Mathf.Abs (z - 0.5f);
		return Mathf.Exp(-d * d / (sigma * sigma)) * MaxHeight;
	}
}
