using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // used for Sum of array

public class HeightSplatMap : MonoBehaviour {
	[Range(0f, 0.5f)]
	public float BlendArea = 0.1f;
	[Range(0f, 1f)]
	public float BlendPoint1 = 0.25f;
	[Range(0f, 1f)]
	public float BlendPoint2 = 0.5f;
	[Range(0f, 1f)]
	public float BlendPoint3 = 0.75f;

	void Start () {
		// Get the attached terrain component
		Terrain terrain = GetComponent<Terrain>();

		// Get a reference to the terrain data
		TerrainData terrainData = terrain.terrainData;

		List<float> blendPoints = new List<float> () { BlendPoint1, BlendPoint2, BlendPoint3 };


		// Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
		float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

		for (int iz = 0; iz < terrainData.alphamapHeight; iz++) {
			for (int ix = 0; ix < terrainData.alphamapWidth; ix++) {
				// Normalise x/y coordinates to range 0-1 
				float z = (float)iz/(float)terrainData.alphamapHeight;
				float x = (float)ix/(float)terrainData.alphamapWidth;

				// Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
				float height = terrainData.GetHeight(Mathf.RoundToInt(z * terrainData.heightmapHeight),Mathf.RoundToInt(x * terrainData.heightmapWidth) ) / terrainData.size.y;

				// Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
				Vector3 normal = terrainData.GetInterpolatedNormal(z,x);

				// Calculate the steepness of the terrain
				float steepness = terrainData.GetSteepness(z,x);

				// Setup an array to record the mix of texture weights at this point
				float[] splatWeights = new float[terrainData.alphamapLayers];

				// CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

				for (int i = 0; i < terrainData.alphamapLayers; i++) {

					float startBlend, endBlend;

					if (i == 0)
						startBlend = float.MaxValue;
					else
						startBlend = (1f / BlendArea) * (height - (blendPoints [i-1] - BlendArea));

					if (i + 1 == terrainData.alphamapLayers)
						endBlend = float.MaxValue;
					else
						endBlend = (-1f / BlendArea) * (height - (blendPoints [i] + BlendArea));

					float splatWeight = Mathf.Clamp01 (Mathf.Min (startBlend, endBlend));

					splatWeights [i] = splatWeight;
				}
					
				// Texture[0] has constant influence
				//splatWeights[0] = 0.5f;

				// Texture[1] is stronger at lower altitudes
				//splatWeights[1] = Mathf.Clamp01((height / terrainData.heightmapHeight));

				// Texture[2] stronger on flatter terrain
				// Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
				// Subtract result from 1.0 to give greater weighting to flat surfaces
				//splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight/5.0f));

				// Texture[3] increases with height but only on surfaces facing positive Z axis 
				//splatWeights[3] = height * Mathf.Clamp01(normal.z);

				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
				float texture_sum = splatWeights.Sum();

				// Loop through each terrain texture
				for(int i = 0; i<terrainData.alphamapLayers; i++){

					// Normalize so that sum of all texture weights = 1
					splatWeights[i] /= texture_sum;

					// Assign this point to the splatmap array
					splatmapData[ix, iz, i] = splatWeights[i];
				}
			}
		}

		// Finally assign the new splatmap to the terrainData:
		terrainData.SetAlphamaps(0, 0, splatmapData);
	}
}