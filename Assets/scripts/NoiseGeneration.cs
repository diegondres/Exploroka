using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave {
  public float seed;
  public float frequency;
  public float amplitude;

}

public class NoiseGeneration : MonoBehaviour
{

  [SerializeField]
  private float scale = 10;

  public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float offsetX, float offsetZ, Wave[] waves)
  {
//    Debug.Log("offsetZ: " +  offsetZ + " offsetX: " + offsetX);
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
        float noise = 0f;
        float normalization = 0f;

        foreach (Wave wave in waves) {
          // generate noise value using PerlinNoise for a given Wave
          noise += wave.amplitude * Mathf.PerlinNoise (sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
          normalization += wave.amplitude;
        }
        // normalize the noise value so that it is within 0 and 1
        noise /= normalization;

        noiseMap[zIndex, xIndex] = noise;
      }
    }
    return noiseMap;
  }

  public float[,] GenerateUniformNoiseMap(int mapDepth, int mapWidth, float centerVertexZ, float maxDistanceZ, float offsetZ) {
    // create an empty noise map with the mapDepth and mapWidth coordinates
    float[,] noiseMap = new float[mapDepth, mapWidth];
    for (int zIndex = 0; zIndex < mapDepth; zIndex++) {
      // calculate the sampleZ by summing the index and the offset
      float sampleZ = zIndex + offsetZ;
      // calculate the noise proportional to the distance of the sample to the center of the level
      float noise = Mathf.Abs (sampleZ - centerVertexZ) / maxDistanceZ;
      // apply the noise for all points with this Z coordinate
      for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
        noiseMap [mapDepth - zIndex - 1, xIndex] = noise;
      }
    }
    return noiseMap;
  }
}
