using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    public Color color;
}
[System.Serializable]
public class BiomeRow
{
    public Biome[] biomes;
}
[System.Serializable]
public class TerrainType
{
    public string name;
    public float threshold;
    public Color color;
    public int index;
}
public class TerrainGeneration : MonoBehaviour
{
    [NonSerialized]
    public TerrainType[] heightTerrainTypes;
    [NonSerialized]
    public TerrainType[] heatTerrainTypes;
    [NonSerialized]
    public float heightMultiplier;
    [NonSerialized]
    public AnimationCurve heightCurve;
    [NonSerialized]
    public AnimationCurve heatCurve;
    private Terreno terreno;
    //GENERACION PROCEDURAL
    [NonSerialized]
    public Wave[] waves;
    [NonSerialized]
    public Wave[] heatWaves;
    [NonSerialized]
    public TerrainType[] moistureTerrainTypes;
    [NonSerialized]
    public AnimationCurve moistureCurve;
    [NonSerialized]
    public Wave[] moistureWaves;
    [NonSerialized]
    public BiomeRow[] biomes;
    private NoiseGeneration noiseGeneration;
    private MeshRenderer tileRenderer;
    private int sizeTerrainInVertices;
    
    private TerrainAdministrator terrainAdministrator;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private Color waterColor = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        Inicialization();
    }
    void Inicialization()
    {
        tileRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        terreno = GetComponent<Terreno>();
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        noiseGeneration = FindAnyObjectByType<NoiseGeneration>();
        
        sizeTerrainInVertices = terrainAdministrator.GetSizeTerrainInVertices();

        int tileDepth = sizeTerrainInVertices;
        int tileWidth = sizeTerrainInVertices;

        meshFilter.mesh = CrearPlanoConDivisiones(tileDepth, tileWidth, tileDepth, tileWidth);
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        meshCollider.sharedMesh = meshFilter.mesh;

        // calculate the offsets based on the tile position
        float offsetX = transform.position.x;
        float offsetZ = transform.position.z;

        float[,] heightMap = noiseGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX / sizeTerrainInVertices, offsetZ / sizeTerrainInVertices, waves);

        Vector3 tileDimensions = meshFilter.mesh.bounds.size;
        float distanceBetweenVertices = tileDimensions.z / (float)tileDepth;
        float vertexOffsetZ = gameObject.transform.position.z / distanceBetweenVertices;
        
        float[,] uniformHeatMap = noiseGeneration.GenerateUniformNoiseMap(tileWidth, tileDepth, 200, 200, vertexOffsetZ);
        float[,] randomHeatMap = noiseGeneration.GenerateNoiseMap(tileWidth, tileWidth, offsetX / sizeTerrainInVertices, offsetZ / sizeTerrainInVertices, heatWaves);
        float[,] heatMap = new float[tileWidth, tileDepth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // mix both heat maps together by multiplying their values
                heatMap[zIndex, xIndex] = uniformHeatMap[zIndex, xIndex] * randomHeatMap[zIndex, xIndex];
                // makes higher regions colder, by adding the height value to the heat map
                heatMap[zIndex, xIndex] += heatCurve.Evaluate(heightMap[zIndex, xIndex]) * heightMap[zIndex, xIndex];
            }
        }
        // generate a moistureMap using Perlin Noise
        float[,] moistureMap = noiseGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ, moistureWaves);
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // makes higher regions dryer, by reducing the height value from the heat map
                moistureMap[zIndex, xIndex] -= moistureCurve.Evaluate(heightMap[zIndex, xIndex]) * heightMap[zIndex, xIndex];
            }
        }
        TerrainType[,] chosenHeightTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D heightTexture = BuildTexture(heightMap, this.heightTerrainTypes, chosenHeightTerrainTypes);
        TerrainType[,] chosenHeatTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D heatTexture = BuildTexture(heatMap, this.heatTerrainTypes, chosenHeatTerrainTypes);
        // build a Texture2D from the moisture map
        TerrainType[,] chosenMoistureTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D moistureTexture = BuildTexture(moistureMap, this.moistureTerrainTypes, chosenMoistureTerrainTypes);
        // build a biomes Texture2D from the three other noise variables
        Texture2D biomeTexture = BuildBiomeTexture(chosenHeightTerrainTypes, chosenHeatTerrainTypes, chosenMoistureTerrainTypes);

        

        //TerrainType[,] chosenHeightTerrainTypes = new TerrainType[tileDepth, tileWidth];
        //Texture2D heightTexture = BuildTexture(heightMap, heightTerrainTypes, chosenHeightTerrainTypes);
        
        tileRenderer.material.mainTexture = biomeTexture;

       // UpdateMeshVertices(heightMap);
    }

    private Texture2D BuildTexture(float[,] heightMap, TerrainType[] terrainTypes, TerrainType[,] chosenTerrainTypes)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color32[] colorMap = new Color32[tileDepth * tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // choose a terrain type according to the height value
                TerrainType terrainType = ChooseTerrainType(height, terrainTypes);
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = terrainType.color;

                // save the chosen terrain type
                chosenTerrainTypes[zIndex, xIndex] = terrainType;
            }
        }

        // create a new texture and set its pixel colors
        terreno.originalPixels = colorMap;
        terreno.heights = heightMap;
        terreno.heightMultiplier = heightMultiplier;
        terreno.heightCurve = heightCurve;
        Texture2D tileTexture = new(tileWidth, tileDepth)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        tileTexture.SetPixels32(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    private Texture2D BuildBiomeTexture(TerrainType[,] heightTerrainTypes, TerrainType[,] heatTerrainTypes, TerrainType[,] moistureTerrainTypes)
    {
        int tileDepth = heatTerrainTypes.GetLength(0);
        int tileWidth = heatTerrainTypes.GetLength(1);
        Color32[] colorMap = new Color32[tileDepth * tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                int colorIndex = zIndex * tileWidth + xIndex;
                TerrainType heightTerrainType = heightTerrainTypes[zIndex, xIndex];
                // check if the current coordinate is a water region
                if (heightTerrainType.name != "water")
                {
                    // if a coordinate is not water, its biome will be defined by the heat and moisture values
                    TerrainType heatTerrainType = heatTerrainTypes[zIndex, xIndex];
                    TerrainType moistureTerrainType = moistureTerrainTypes[zIndex, xIndex];
                    // terrain type index is used to access the biomes table
                    Biome biome = biomes[moistureTerrainType.index].biomes[heatTerrainType.index];
                    // assign the color according to the selected biome
                    colorMap[colorIndex] = biome.color;
                }
                else
                {
                    // water regions don't have biomes, they always have the same color
                    colorMap[colorIndex] = waterColor;
                }
            }
        }
        // create a new texture and set its pixel colors
        Texture2D tileTexture = new(tileWidth, tileDepth)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        tileTexture.SetPixels32(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    private TerrainType ChooseTerrainType(float noise, TerrainType[] terrainTypes)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if (noise < terrainType.threshold)
            {
                return terrainType;
            }
        }
        return terrainTypes[^1];
    }

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }
        // update the vertices in the mesh and update its properties
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    Mesh CrearPlanoConDivisiones(int divisionesX, int divisionesY, float tamañoX, float tamañoY)
    {
        Mesh mesh = new();

        // Crear vértices
        int verticeCountX = divisionesX + 1;
        int verticeCountY = divisionesY + 1;
        Vector3[] vertices = new Vector3[verticeCountX * verticeCountY];
        Vector2[] uvs = new Vector2[vertices.Length]; // Arreglo de coordenadas de textura
        Vector3[] normals = new Vector3[vertices.Length]; // Arreglo de normales

        for (int y = 0; y < verticeCountY; y++)
        {
            for (int x = 0; x < verticeCountX; x++)
            {
                int index = x + y * verticeCountX;
                float posX = (x / (float)divisionesX) * tamañoX - tamañoX / 2.0f;
                float posY = (y / (float)divisionesY) * tamañoY - tamañoY / 2.0f;
                vertices[index] = new Vector3(posX, 0f, posY);
                uvs[index] = new Vector2(x / (float)divisionesX, y / (float)divisionesY);
                normals[index] = Vector3.up; // Asigna una normal vertical

            }
        }

        // Aplicar los vértices a la malla
        mesh.vertices = vertices;

        int[] triangles = new int[divisionesX * divisionesY * 6];
        int trianguloIndex = 0;
        for (int y = 0; y < divisionesY; y++)
        {
            for (int x = 0; x < divisionesX; x++)
            {
                int vertexIndex0 = x + y * verticeCountX;
                int vertexIndex1 = (x + 1) + y * verticeCountX;
                int vertexIndex2 = x + (y + 1) * verticeCountX;
                int vertexIndex3 = (x + 1) + (y + 1) * verticeCountX;

                // Triángulo 1 del cuadrado
                triangles[trianguloIndex++] = vertexIndex0;
                triangles[trianguloIndex++] = vertexIndex2;
                triangles[trianguloIndex++] = vertexIndex1;

                // Triángulo 2 del cuadrado
                triangles[trianguloIndex++] = vertexIndex1;
                triangles[trianguloIndex++] = vertexIndex2;
                triangles[trianguloIndex++] = vertexIndex3;
            }
        }

        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;

        return mesh;
    }
}



