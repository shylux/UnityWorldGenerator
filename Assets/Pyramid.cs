using UnityEngine;
using System.Collections;

public class Pyramid : HeightGenerator {

	public override float getHeight(float x, float z) {
		return Mathf.Min (
			MaxHeight - (Mathf.Abs(x - 0.5f) * MaxHeight),
			MaxHeight - (Mathf.Abs(z - 0.5f) * MaxHeight));
	}
}
