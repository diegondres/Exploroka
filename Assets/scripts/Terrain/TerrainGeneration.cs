using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
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
    string[,] chosenHeightTerrainTypes;


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

        Texture2D heightTexture = BuildTexture();

        tileRenderer.material.mainTexture = heightTexture;

        UpdateMeshVertices();
    }

    private Texture2D BuildTexture()
    {
        Color32[] colorMap = new Color32[tileDepth * tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = colorMapa[zIndex, xIndex]; //terrainType.color;
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


    private void GetHeightMap()
    {
        int heightMapDepth = tileDepth + 1;
        int heightMapWidth = tileWidth + 1;
        heightMap = new float[heightMapDepth, heightMapWidth];
        colorMapa = new Color32[heightMapDepth, heightMapWidth];
        chosenHeightTerrainTypes = new string[heightMapDepth, heightMapWidth];
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

                int vertexX = (int)vertex.x + sizeTerrainInVertices / 2 + offsetX;
                int vertexZ = (int)vertex.z + sizeTerrainInVertices / 2 + offsetZ;

                Tuple<float, Color32, string> datosEscaque = noiseGeneration.GetHeight(vertexX, vertexZ);
                heightMap[xIndex, zIndex] = datosEscaque.Item1;
                colorMapa[xIndex, zIndex] = datosEscaque.Item2;

                if (datosEscaque.Item3.Length > 0)
                {
                    Vector3 positionResource = new(vertexX * 20 - 190, datosEscaque.Item1 * heightMultiplier * 20, vertexZ * 20 - 190);
                    int indexPrefabResource = GetModelFromResource(datosEscaque.Item3);
                    int numericIndexResource = SubObjectsAdmReferences.GetNumericIndexFromGlobalPosition(positionResource, terreno);

                    if (!SubResourcesObjAdmin.resources.ContainsKey(numericIndexResource))
                    {
                        GameObject resource = Instantiate(terrainAdministrator.prefabsResources[indexPrefabResource].Item1, positionResource, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0), objetsAdministrator.containerResources.transform);
                        ResourcesClass resourceInfo = terrainAdministrator.prefabsResources[indexPrefabResource].Item2.Clone();
                        resourceInfo.numericIndex = numericIndexResource;
                        resourceInfo.globalPosition = positionResource;

                        SubResourcesObjAdmin.AddResource(resource, resourceInfo, terreno);
                    }
                }
                if (datosEscaque.Item1 == noiseGeneration.nAgua)
                {
                    chosenHeightTerrainTypes[xIndex, zIndex] = "water";
                }
                else
                {
                    chosenHeightTerrainTypes[xIndex, zIndex] = "notWater";
                }
                vertexIndex++;
            }
        }
    }

    private int GetModelFromResource(string rec)
    {
        if (terrainAdministrator.modelosRecursos.ContainsKey(rec))
        {
            return terrainAdministrator.modelosRecursos[rec][Random.Range(0, terrainAdministrator.modelosRecursos[rec].Count)];
        }
        else
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

    public string GetTerrainType(Vector3 relativePositionInVertices)
    {
        int xIndex = (int)(relativePositionInVertices.x + sizeTerrainInVertices / 2);
        int zIndex = (int)(relativePositionInVertices.z + sizeTerrainInVertices / 2);

        return chosenHeightTerrainTypes[zIndex, xIndex];
    }
}



