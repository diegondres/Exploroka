using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGeneration : MonoBehaviour
{

  [SerializeField]
  private float scale = 10;

  public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float offsetX, float offsetZ)
  {
    Debug.Log("offsetZ: " +  offsetZ + " offsetX: " + offsetX);
    // create an empty noise map with the mapDepth and mapWidth coordinates
    float[,] noiseMap = new float[mapDepth, mapWidth];
    for (int zIndex = 0; zIndex < mapDepth; zIndex++)
    {
      for (int xIndex = 0; xIndex < mapWidth; xIndex++)
      {
        // calculate sample indices based on the coordinates, the scale and the offset
        float sampleX = (xIndex + offsetX) / scale;
        float sampleZ = (zIndex + offsetZ) / scale;
        // generate noise value using PerlinNoise
        float noise = Mathf.PerlinNoise(sampleZ, sampleX);

        noiseMap[zIndex, xIndex] = noise;
      }
    }
    return noiseMap;
  }
}
