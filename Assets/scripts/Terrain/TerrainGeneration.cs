using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public float heightMultiplier;
    private Terreno terreno;
    //GENERACION PROCEDURAL
    private NoiseGeneration noiseGeneration;
    private MeshRenderer tileRenderer;
    private int sizeTerrainInVertices;

    //COSITAS INTERNAS
    private TerrainAdministrator terrainAdministrator;
    private ObjetsAdministrator objetsAdministrator;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private float[,] heightMap;
    private Color32[,] colorMapa;
    private int tileDepth, tileWidth;
    TerrainType[,] chosenHeightTerrainTypes;


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
        objetsAdministrator = FindObjectOfType<ObjetsAdministrator>();
        noiseGeneration = FindAnyObjectByType<NoiseGeneration>();
        sizeTerrainInVertices = SubTerrainAdmReference.sizeTerrainInVertices;

        tileDepth = sizeTerrainInVertices; tileWidth = sizeTerrainInVertices;

        meshFilter.mesh = CrearPlanoConDivisiones(tileDepth, tileWidth, tileDepth, tileWidth);

        GetHeightMap();

        chosenHeightTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D heightTexture = BuildTexture(heightTerrainTypes, chosenHeightTerrainTypes);

        tileRenderer.material.mainTexture = heightTexture;

        UpdateMeshVertices();
    }

    private Texture2D BuildTexture(TerrainType[] terrainTypes, TerrainType[,] chosenTerrainTypes)
    {
        Color32[] colorMap = new Color32[tileDepth * tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = (heightMap[zIndex, xIndex] + heightMap[zIndex + 1, xIndex] + heightMap[zIndex, xIndex + 1] + heightMap[zIndex + 1, xIndex + 1]) / 4;
                // choose a terrain type according to the height value
                TerrainType terrainType = ChooseTerrainType(height, terrainTypes);
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = colorMapa[zIndex, xIndex]; //terrainType.color;

                // save the chosen terrain type
                chosenTerrainTypes[zIndex, xIndex] = terrainType;
            }
        }

        terreno.originalPixels = colorMap;
        Texture2D tileTexture = new(tileWidth, tileDepth)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
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

    private void GetHeightMap(){
        int heightMapDepth = tileDepth + 1;
        int heightMapWidth = tileWidth + 1;
        heightMap = new float[heightMapDepth, heightMapWidth];
        colorMapa = new Color32[heightMapDepth, heightMapWidth];

        Vector3[] meshVertices = meshFilter.mesh.vertices;

        int offsetX = (int)(transform.position.x / 20);
        int offsetZ = (int)(transform.position.z / 20);

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        

        for (int xIndex = 0; xIndex < heightMapDepth; xIndex++)
        {
            for (int zIndex = 0; zIndex < heightMapWidth; zIndex++)
            {
                Vector3 vertex = meshVertices[vertexIndex];
                
                int vertexX = (int)vertex.x + sizeTerrainInVertices /2 + offsetX; 
                int vertexZ = (int)vertex.z + sizeTerrainInVertices /2 + offsetZ;
                
                Tuple<float, Color32, string> datosEscaque = noiseGeneration.GetHeight(vertexX, vertexZ);
                heightMap[xIndex, zIndex] = datosEscaque.Item1;
                colorMapa[xIndex, zIndex] = datosEscaque.Item2;
                if(datosEscaque.Item3.Length>0)
                {
                    Instantiate(terrainAdministrator.Figuras3D[GetModelFromResource(datosEscaque.Item3)], new Vector3(vertexX*20-200 + Random.Range(-3f,3f), datosEscaque.Item1 * heightMultiplier * 20, vertexZ*20- 200 + Random.Range(-3f, 3f)),Quaternion.Euler(0,Random.Range(0,4)*90,0), objetsAdministrator.containerResources.transform);
                }
                vertexIndex++;
            }
        }
    }

    private int GetModelFromResource(string rec)
    {
        if(terrainAdministrator.modelosRecursos.ContainsKey(rec))
        {
            return terrainAdministrator.modelosRecursos[rec][Random.Range(0, terrainAdministrator.modelosRecursos[rec].Count)];
        } else
        {
            return 0;
        }
    }

    private void UpdateMeshVertices()
    {
        int heightMapDepth = heightMap.GetLength(0);
        int heightMapWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < heightMapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < heightMapWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * heightMultiplier, vertex.z);

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

    public float GetHeight(Vector3 relativePositionInVertices)
    {
        int xIndex = (int)(relativePositionInVertices.x + sizeTerrainInVertices / 2);
        int zIndex = (int)(relativePositionInVertices.z + sizeTerrainInVertices / 2);

        float meanHeight = (heightMap[zIndex, xIndex] + heightMap[zIndex + 1, xIndex] + heightMap[zIndex, xIndex + 1] + heightMap[zIndex + 1, xIndex + 1]) / 4;

        return meanHeight * heightMultiplier * gameObject.transform.localScale.y;
    }

    public string GetTerrainType(Vector3 relativePositionInVertices){
        int xIndex = (int)(relativePositionInVertices.x + sizeTerrainInVertices / 2);
        int zIndex = (int)(relativePositionInVertices.z + sizeTerrainInVertices / 2);

        return chosenHeightTerrainTypes[zIndex, xIndex].name;
    }
}



