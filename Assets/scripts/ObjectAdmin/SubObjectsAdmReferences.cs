using System;
using System.Collections.Generic;
using UnityEngine;

public static class SubObjectsAdmReferences
{
    public static readonly List<Dictionary<int, GameObject>> allObjects = new();
    private static readonly Dictionary<int, GameObject> constructions = new();
    public static readonly Dictionary<int, List<GameObject>> frontiers = new();
    private static readonly List<City> cities = new();
    private static Tuple<int, Terreno> buildingGlobalIndex;
    public static bool isBuildingLocationSelected = false;
    private static readonly int multiplier = 1000;

    public static GameObject containerConstructions;
    public static GameObject containerFrontiers;

    public static void Inicializate()
    {
        allObjects.Add(constructions);
        allObjects.Add(SubResourcesObjAdmin.resources);
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
    public static void AddCity(City city)
    {
        cities.Add(city);
    }
    public static bool IsNameCityTaken(string nameCity)
    {
        foreach (City city in cities)
        {
            if (city.nameCity == nameCity)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetNumericIndex(Tuple<int, Terreno> index)
    {
        return index.Item2.id * multiplier + index.Item1;
    }

    public static int GetNumericIndexFromGlobalPosition(Vector3 globalPosition, Terreno terreno)
    {
        Vector3 relativePosition = terreno.GetRelativePositionInVertices(globalPosition);
        Tuple<int, Terreno> globalIndex = terreno.GetIndexGlobal(relativePosition);
        
        int wea = GetNumericIndex(globalIndex);
  
        return wea;
    }

    public static Tuple<GameObject, int> IsSomethingBuiltInHere(Tuple<int, Terreno> globalIndex)
    {
        int indexDict = GetNumericIndex(globalIndex);
        int count = 0;
        foreach (Dictionary<int, GameObject> dict in allObjects)
        {
            if (dict.ContainsKey(indexDict)) return new Tuple<GameObject, int>(dict[indexDict], count);
            count++;
        }
        return null;
    }


}