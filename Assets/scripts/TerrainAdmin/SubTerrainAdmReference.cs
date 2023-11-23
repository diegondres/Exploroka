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


  [NonSerialized]
  public static readonly Dictionary<int, Town> influencedEscaques = new();
  [NonSerialized]
  public static readonly List<Terreno> terrenosWithoutResources = new();
  private static readonly List<Tuple<int, Terreno>> sorroundingEscaques = new();


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

  public static Town DetectCity(Vector3 globalPositionTown, int radio)
  {
    Vector3 relativePosition = terrainOfHero.GetRelativePositionInVertices(globalPositionTown);
    for (int i = -radio; i < radio; i++)
    {
      for (int j = -radio; j < radio; j++)
      {
        Tuple<int, Terreno> indexGlobal = terrainOfHero.GetIndexGlobal(new Vector3(relativePosition.x + i, relativePosition.y, relativePosition.z + j));
        int numericIndex = SubObjectsAdmReferences.GetNumericIndex(indexGlobal);

        if (influencedEscaques.ContainsKey(numericIndex))
        {
          return influencedEscaques[numericIndex];
        };
      }
    }

    return null;
  }

}
