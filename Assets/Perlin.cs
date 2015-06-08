using UnityEngine;
using System.Collections;

public class Perlin : HeightGenerator {
	public float NoiseScale = 1f;
	public int Levels = 1;
	public float sigma = 1f;

	public override float getHeight(float x, float z) {
		float detailLevel = 1f;
		float height = 0f;
		// distance to center of extrema
//		float d = Mathf.Abs (x - 0.5f) + Mathf.Abs (z - 0.5f);
//		float height = Mathf.Exp (-d * d / (sigma * sigma));
//		return height * MaxHeight;

		for (int lv = 1; lv <= Levels; lv++) {
			float levelAddition = Mathf.PerlinNoise (x * NoiseScale * detailLevel,
			                                         z * NoiseScale * detailLevel);
			detailLevel /= 2;

			//levelAddition /= Mathf.Pow(10, lv-1);
			height += levelAddition;
		}

		return height * MaxHeight;
	}
}