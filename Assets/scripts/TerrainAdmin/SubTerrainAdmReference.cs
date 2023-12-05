using System;
using System.Collections.Generic;
using UnityEngine;

static class SubTerrainAdmReference
{
  public static readonly int sizeOfTerrain = 400;
  [NonSerialized]
  public static int sizeEscaque = sizeOfTerrain / 20;
  [NonSerialized]
  public static int sizeTerrainInVertices = sizeOfTerrain / 20;
  [NonSerialized]
  public static readonly Dictionary<Vector3, GameObject> terrainDict = new();

  [NonSerialized]
  public static Terreno terrainOfHero;
  private static Vector3 positionHero = Vector3.zero;

  private static readonly int multiplier = 10000;



  [NonSerialized]
  public static readonly Dictionary<int, Town> influencedEscaques = new();
  [NonSerialized]
  public static readonly List<Terreno> terrenosWithoutResources = new();
  private static readonly List<Tuple<int, Terreno>> sorroundingEscaques = new();
  [NonSerialized]
  public static readonly List<Tuple<int, Terreno>> newInfluenceEscaques = new();


  public static bool IsThisEscaqueVisited(Tuple<int, Terreno> visitedEscaque)
  {
    foreach (Tuple<int, Terreno> escaque in sorroundingEscaques)
    {
      if (CompareTwoEscaques(visitedEscaque, escaque))
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
  public static int GetNumericIndexFromGlobalPosition(Vector3 globalPosition){
    Vector3 relativePosition = terrainOfHero.GetRelativePositionInVertices(globalPosition);
    Tuple<int, Terreno> index = terrainOfHero.GetIndexGlobal(relativePosition);
    
    return GetNumericIndex(index);
  }
   public static Tuple<int, Terreno> GetIndexFromNumeric(int num, TerrainAdministrator terrainAdministrator)
    {
        int id = num / multiplier;
        int index = num - id * multiplier;
        Terreno terreno = terrainAdministrator.GetTerrenoScriptFromId(id);

        return new Tuple<int, Terreno>(index, terreno);
    }

  public static bool CompareTwoEscaques(Tuple<int, Terreno> escaque1, Tuple<int, Terreno> escaque2)
  {
    bool item1 = false;
    bool item2 = false;

    if (escaque1.Item1 == escaque2.Item1) item1 = true;
    if (escaque1.Item2.id == escaque1.Item2.id) item2 = true;

    return item1 && item2;
  }

  public static void CleanVisitedEscaques()
  {
    foreach (Tuple<int, Terreno> escaque in sorroundingEscaques)
    {
      escaque.Item2.ReturnPixelToOriginal(escaque.Item1);
    }
    sorroundingEscaques.Clear();
  }
  public static void AddSorroundinEscaque(Tuple<int, Terreno> IndexEscaque)
  {
    sorroundingEscaques.Add(IndexEscaque);
  }
  public static List<Tuple<int, Terreno>> GetSorroundingEscaques()
  {
    return sorroundingEscaques;
  }

  public static Terreno InWhatTerrenoAmI(Vector3 position)
  {
    Vector3 relativePosition = position / sizeOfTerrain;
    Vector3 terrainPosition = new((int)relativePosition.x * sizeOfTerrain, 0, (int)relativePosition.z * sizeOfTerrain);

    if (terrainDict.ContainsKey(terrainPosition))
    {
      terrainOfHero = terrainDict[terrainPosition].GetComponent<Terreno>();
      return terrainOfHero;
    }
    return null;
  }

  public static bool IsTerrainActive(Terreno terreno)
  {
    return terrainOfHero.id == terreno.id;
  }

  public static Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
  {
    return terrainOfHero.CalculateDistance(actualPosition, destiny);
  }
  public static Vector3 MoveHero(Vector3 position, Vector3 movement)
  {
    positionHero = terrainOfHero.Move(position, movement);
    
    return positionHero;
  }
  public static void SetTerrenoOfHero(Terreno terreno, SubTerrainAdmGeneration subTerrainAdmGeneration)
  {
    terrainOfHero = terreno;
    subTerrainAdmGeneration.FillNeighborhood(terreno);

    foreach (Terreno item in terreno.neighboors)
    {
      subTerrainAdmGeneration.FillNeighborhood(item);
    }
  }

  public static List<City> DetectCity(Vector3 globalPositionTown, int radio)
  {
    Vector3 relativePosition = terrainOfHero.GetRelativePositionInVertices(globalPositionTown);
    List<City> citiesDetected = new();

    for (int i = -radio; i < radio; i++)
    {
      for (int j = -radio; j < radio; j++)
      {
        Tuple<int, Terreno> indexGlobal = terrainOfHero.GetIndexGlobal(new Vector3(relativePosition.x + i, relativePosition.y, relativePosition.z + j));
        int numericIndex = GetNumericIndex(indexGlobal);

        if (influencedEscaques.ContainsKey(numericIndex) && !CheckIfCityIsInList(citiesDetected, influencedEscaques[numericIndex].city.id))
        {
          citiesDetected.Add(influencedEscaques[numericIndex].city);
        };
      }
    }

    return citiesDetected;
  }

  static bool CheckIfCityIsInList(List<City> cities, int id)
  {
    foreach (City city in cities)
    {
      if (city.id == id)
      {
        return true;
      }
    }
    return false;
  }

  public static bool IsThisEscaqueInfluenced(Vector3 relativePosition, Terreno terreno, int city)
  {
    //Calidad garantizada por el practicante tassadar
    Tuple<int, Terreno> indexSides = terreno.GetIndexGlobal(relativePosition);
    Vector3 realRelativePosition = indexSides.Item2.GetRelativePositionFromGlobalIndex(indexSides);

    int numericIndex = GetNumericIndex(indexSides);

    if (!terreno.IsWalkable(realRelativePosition))
    {
      return true;
    }
    if (city == -1)
    {
      return influencedEscaques.ContainsKey(numericIndex);
    }
    else
    {
      return influencedEscaques.ContainsKey(numericIndex) && influencedEscaques[numericIndex].city.id == city;
    }
  }

  public static void CheckNewInfluencedEscaques(List<Tuple<int, Terreno>> influencedEscaques, int city, TerrainAdministrator terrainAdministrator)
  {
    foreach (Tuple<int, Terreno> newInfluencedEscaque in influencedEscaques)
    {
      Vector3 relativePosition = newInfluencedEscaque.Item2.GetRelativePositionFromGlobalIndex(newInfluencedEscaque);

      if (!IsThisEscaqueInfluenced(relativePosition + new Vector3(-1, 0, 0), newInfluencedEscaque.Item2, city))
      {
        terrainAdministrator.PutFrontierInEscaque(newInfluencedEscaque, new Vector3(-9, 0, 0), Quaternion.identity, city);
      }

      if (!IsThisEscaqueInfluenced(relativePosition + new Vector3(1, 0, 0), newInfluencedEscaque.Item2, city))
      {
        terrainAdministrator.PutFrontierInEscaque(newInfluencedEscaque, new Vector3(9, 0, 0), Quaternion.identity, city);
      }

      if (!IsThisEscaqueInfluenced(relativePosition + new Vector3(0, 0, -1), newInfluencedEscaque.Item2, city))
      {
        terrainAdministrator.PutFrontierInEscaque(newInfluencedEscaque, new Vector3(0, 0, -9), Quaternion.Euler(new Vector3(0, -90, 0)), city);
      }

      if (!IsThisEscaqueInfluenced(relativePosition + new Vector3(0, 0, 1), newInfluencedEscaque.Item2, city))
      {
        terrainAdministrator.PutFrontierInEscaque(newInfluencedEscaque, new Vector3(0, 0, 9), Quaternion.Euler(new Vector3(0, 90, 0)), city);
      }

    }
    newInfluenceEscaques.Clear();
  }

}
