using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{

    public string name;
    public float height;
    public Color color;

    public TerrainType(string name, float height, Color color)
    {
        this.name = name;
        this.height = height;
        this.color = color;
    }
}
public class TerrainGeneration : MonoBehaviour
{
    [NonSerialized]
    public TerrainType[] terrainTypes;
    [NonSerialized]
    public float heightMultiplier;
    [NonSerialized]
    public AnimationCurve heightCurve;
    //GENERACION PROCEDURAL
    private NoiseGeneration noiseGeneration;

    private MeshRenderer tileRenderer;
    private int sizeSquare;
    private TerrainAdministrator terrainAdministrator;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;


    // Start is called before the first frame update
    void Start()
    {
        EnCuadriculas();
    }
    void EnCuadriculas()
    {
        tileRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        noiseGeneration = FindAnyObjectByType<NoiseGeneration>();
        sizeSquare = terrainAdministrator.sizeEscaque;

        meshFilter.mesh = CrearPlanoConDivisiones((sizeSquare * 2) - 1, (sizeSquare * 2) - 1, sizeSquare, sizeSquare);
        tileRenderer = GetComponent<MeshRenderer>();

        // calculate the offsets based on the tile position
        float offsetX = transform.position.x;
        float offsetZ = transform.position.z;

        float[,] heightMap = noiseGeneration.GenerateNoiseMap(sizeSquare * 2, sizeSquare * 2, offsetX, offsetZ);

        Texture2D tileTexture = BuildTexture(heightMap);
        tileRenderer.material.mainTexture = tileTexture;
        UpdateMeshVertices(heightMap);
    }

    private Texture2D BuildTexture(float[,] heightMap)
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
                TerrainType terrainType = ChooseTerrainType(height);
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = terrainType.color;
            }
        }

        // create a new texture and set its pixel colors

        Texture2D tileTexture = new(tileWidth, tileDepth)
        {
            wrapMode = TextureWrapMode.Clamp
        };
        tileTexture.SetPixels32(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    TerrainType ChooseTerrainType(float height)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainType.height)
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

        // Definir los triángulos
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

        // Aplicar los triángulos a la malla
        mesh.triangles = triangles;

        // Aplicar las coordenadas de textura a la malla
        mesh.uv = uvs;

        // Aplicar las normales a la malla
        mesh.normals = normals;

        return mesh;
    }
}



