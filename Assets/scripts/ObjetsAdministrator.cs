using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Clase pensada en manjer toda la generalidad de los objetos que esten sobre el mapa, como construcciones, recursos, etc. 
/// </summary>
public class ObjetsAdministrator : MonoBehaviour
{
    //LAS LISTAS DE OBJETOS Y LA LISTA DE LISTAS
    private readonly List<Dictionary<int, GameObject>> allObjects = new();
    private readonly Dictionary<int, GameObject> constructions = new();
    private readonly Dictionary<int, GameObject> resources = new();

    //CONSTRUCCION
    private Tuple<int, Terreno> buildingGlobalIndex;
    [NonSerialized]
    public bool isBuildingLocationSelected = false;

    [Header("Recursos")]
    [SerializeField]
    private GameObject containerResources;
    [SerializeField]
    private GameObject resourcePrefab;
    public int probabilidadRecursos = 50;

    //COSITAS
    private TerrainAdministrator terrainAdministrator;

    private const int multiplier = 10000;

    void Start()
    {
        terrainAdministrator = GetComponent<TerrainAdministrator>();
        allObjects.Add(constructions);
        allObjects.Add(resources);
    }

    public void SelectEscaqueToBuildIn(Tuple<int, Terreno> globalIndex)
    {
        if (terrainAdministrator.IsThisEscaqueVisited(globalIndex))
        {
            if (isBuildingLocationSelected)
            {
                if (globalIndex.Item1 == buildingGlobalIndex.Item1)
                {
                    buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);
                    isBuildingLocationSelected = false;
                    return;
                }
                buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);
            }
            isBuildingLocationSelected = true;
            SetBuildingLocation(globalIndex);
            globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.gray);
        }
    }

    public void SetBuildingLocation(Tuple<int, Terreno> pos)
    {
        buildingGlobalIndex = pos;
    }
    public Vector3 GetBuildingLocation()
    {
        isBuildingLocationSelected = false;
        buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);

        return buildingGlobalIndex.Item2.GetGlobalPositionFromGlobalIndex(buildingGlobalIndex);
    }

    public void AddBuilding(GameObject building)
    {
        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(building.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);

        int indexBuildingDict = indexBuilding.Item1 + indexBuilding.Item2.id * multiplier;
        constructions.Add(indexBuildingDict, building);
    }

    public void AddResource(GameObject resource, Terreno terreno)
    {
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(resource.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);

        int indexBuildingDict = indexBuilding.Item1 + indexBuilding.Item2.id * multiplier;
        resource.GetComponent<Resource>().indexDict = indexBuildingDict;
        resources.Add(indexBuildingDict, resource);
    }

    public GameObject IsSomethingBuiltInHere(Tuple<int, Terreno> globalIndex)
    {
        int indexDict = globalIndex.Item2.id * multiplier + globalIndex.Item1;

        foreach (Dictionary<int, GameObject> dict in allObjects)
        {
            if (dict.ContainsKey(indexDict)) return dict[indexDict];
        }
        return null;
    }


    public void GenerateRandomResource(Terreno terreno)
    {

        List<float> probs = new();
        do
        {
            int location = UnityEngine.Random.Range(0, 400);
            Vector3 position = terreno.GetGlobalPositionFromGlobalIndex(new Tuple<int, Terreno>(location, terreno));

            GameObject resource = Instantiate(resourcePrefab, position, Quaternion.identity);
            resource.transform.SetParent(containerResources.transform);
            AddResource(resource, terreno);

            Resource resourceScript = resource.GetComponent<Resource>();
            resourceScript.SetInitialValues("Cosita", location, false, false);

            probs.Add(UnityEngine.Random.Range(0, 100));
        } while (probs.Average() > probabilidadRecursos);
    }
}
