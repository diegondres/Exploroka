using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool isBuildingLocationSelected = false;

    //COSAS
    private TerrainAdministrator terrainAdministrator;

    // Start is called before the first frame update
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

    public void AddBuilding (GameObject building){
        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(building.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);
        
        int indexBuildingDict = indexBuilding.Item1 + indexBuilding.Item2.id * 1000;
        constructions.Add(indexBuildingDict, building);
    }

    public void AddResource(GameObject resource){
        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(resource.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);
        
        int indexBuildingDict = indexBuilding.Item1 + indexBuilding.Item2.id * 1000;
        resources.Add(indexBuildingDict, resource);
    }

    public GameObject IsSomethingBuiltInHere(Tuple<int, Terreno> globalIndex){
        int indexDict = globalIndex.Item2.id*1000 + globalIndex.Item1;
       
        foreach(Dictionary<int, GameObject> dict in allObjects){
            if(dict.ContainsKey(indexDict)) return dict[indexDict];
        }
        return null;
    }
}
