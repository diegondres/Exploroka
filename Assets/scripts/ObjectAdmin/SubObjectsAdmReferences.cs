using System;
using System.Collections.Generic;
using UnityEngine;

public static class SubObjectsAdmReferences
{
    private static readonly List<Dictionary<int, GameObject>> allObjects = new();
    private static readonly Dictionary<int, GameObject> constructions = new();
    private static readonly Dictionary<int, GameObject> resources = new();
    public static readonly Dictionary<int, List<GameObject>> frontiers = new();
    public static readonly List<City> cities = new();
    private static Tuple<int, Terreno> buildingGlobalIndex;
    public static bool isBuildingLocationSelected = false;
    private static readonly int multiplier = 10000;

    public static GameObject containerConstructions;
    public static GameObject containerFrontiers;

    public static void Inicializate()
    {
        allObjects.Add(constructions);
        allObjects.Add(resources);
    }

    public static void InicializateContainerReferences(GameObject containerConst, GameObject containerFront)
    {
        containerConstructions = containerConst;
        containerFrontiers = containerFront;
    }

    public static void SelectEscaqueToBuildIn(Tuple<int, Terreno> globalIndex)
    {
        if (SubTerrainAdmReference.IsThisEscaqueVisited(globalIndex))
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
    public static void SetBuildingLocation(Tuple<int, Terreno> pos)
    {
        buildingGlobalIndex = pos;
    }
    public static Vector3 GetBuildingLocation()
    {
        isBuildingLocationSelected = false;
            buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);

        return buildingGlobalIndex.Item2.GetGlobalPositionFromGlobalIndex(buildingGlobalIndex);
    }

    public static void AddBuilding(GameObject building)
    {
        Terreno terreno = SubTerrainAdmReference.terrainOfHero;
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(building.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);

        int indexBuildingDict = GetNumericIndex(indexBuilding);
        constructions.Add(indexBuildingDict, building);
    }

    public static void AddResource(GameObject resource, Terreno terreno)
    {
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(resource.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);

        int indexBuildingDict = GetNumericIndex(indexBuilding);
        resource.GetComponent<Resource>().indexInDict = indexBuildingDict;
        resources.Add(indexBuildingDict, resource);
    }

    public static int GetNumericIndex(Tuple<int, Terreno> index)
    {
        return index.Item2.id * multiplier + index.Item1;
    }


    public static GameObject IsSomethingBuiltInHere(Tuple<int, Terreno> globalIndex)
    {
        int indexDict = GetNumericIndex(globalIndex);
        foreach (Dictionary<int, GameObject> dict in allObjects)
        {
            if (dict.ContainsKey(indexDict)) return dict[indexDict];
        }
        return null;
    }
    
    public static Resource IsAResourceHere(int numericIndex){
        //Es en el 1 porque esa es la posicion de los recursos en la lista allObjects
        if(allObjects[1].ContainsKey(numericIndex)){
            return allObjects[1][numericIndex].GetComponent<Resource>();
        }
        
        return null;
    }

}