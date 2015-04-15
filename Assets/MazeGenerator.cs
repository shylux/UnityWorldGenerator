using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // used for Sum of array

public class MazeGenerator : Island {
	Maze2D maze;

	public override float getHeight(float x, float z) {
		//return (maze.cell(x, z).get()) ? 3f: 0f;
		return (((x + z) % 2 == 0) ? 1f: 0f);
	}

//	void Start() {
//		Terrain terrain = GetComponent<Terrain>();
//		TerrainData terrainData = terrain.terrainData;
//		float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
//
//		maze = new Maze2D(terrainData.alphamapWidth, terrainData.alphamapHeight);
//		foreach (Maze2D.Cell c in maze.Cells())
//			c.set (((c.x + c.y) % 2 == 0));
//	}

//	void Start() {
//		Terrain terrain = GetComponent<Terrain>();
//		TerrainData terrainData = terrain.terrainData;
//		float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
//
//		Maze2D maze = new Maze2D(terrainData.alphamapWidth, terrainData.alphamapHeight);
//		foreach (Maze2D.Cell c in maze.Cells())
//			c.set (((c.x + c.y) % 2 == 0));
//
//		for (int iz = 0; iz < terrainData.alphamapHeight; iz++) {
//			for (int ix = 0; ix < terrainData.alphamapWidth; ix++) {
//				float[] splatWeights = new float[terrainData.alphamapLayers];
//
//				splatWeights[0] = maze.cell(ix, iz).get() ? 1f : 0f;
//				splatWeights[1] = maze.cell(ix, iz).get() ? 0f : 1f;
//
//				float texture_sum = splatWeights.Sum();
//				for(int i = 0; i<terrainData.alphamapLayers; i++){
//					splatWeights[i] /= texture_sum;
//					splatmapData[ix, iz, i] = splatWeights[i];
//				}
//			}
//		}
//
//		terrainData.SetAlphamaps(0, 0, splatmapData);
//	}
}
