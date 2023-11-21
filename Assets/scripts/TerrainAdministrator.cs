using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TerrainAdministrator : MonoBehaviour
{
  [SerializeField]
  private GameObject containerTerrenos;
  private ObjetsAdministrator objetsAdministrator;

  [Header("Generacion Procedural")]
  [SerializeField]
  private GameObject Terreno;
  [SerializeField]
  private TerrainType[] terrainTypes;
  public float heightMultiplier;
  [SerializeField]
  private AnimationCurve heightCurve;
  [SerializeField]
  private Wave[] waves;
  private readonly Dictionary<Vector3, GameObject> terrainDict = new();
  public readonly Dictionary<int, Terreno> idTerrainDict = new();
  private int countTerrain = 0;

  [Header("Heroe")]
  public int sizeOfTerrain = 200;
  [NonSerialized]
  public Terreno terrenoOfHero;
  public Vector3 positionHero = Vector3.zero;
  [SerializeField]
  private GameObject prefabFrontier;
  private int sizeEscaque;
  private int sizeTerrainInVertices;
  private readonly Dictionary<int, Vector3> vecindario = new();
  public List<Tuple<int, Terreno>> sorroundingEscaques = new();
  public List<Tuple<Tuple<int, Terreno>, int>> frontierEscaques = new();


  [NonSerialized]
  public Dictionary<int, Town> influencedEscaques = new();
  private readonly List<Terreno> terrenosWithoutResources = new();

  void Awake()
  {
    objetsAdministrator = GetComponent<ObjetsAdministrator>();

    sizeEscaque = sizeOfTerrain / 20;
    sizeTerrainInVertices = sizeOfTerrain / 20;
    SetNeighboorsReference();

    CreateFirstTerrain();
  }

  void Update()
  {

    if (terrenosWithoutResources.Count > 0)
    {
      foreach (Terreno terreno in terrenosWithoutResources)
      {
        StartCoroutine(InvokeBueno(terreno));
      }
      terrenosWithoutResources.Clear();
    }

    if (frontierEscaques.Count > 0)
    {
      foreach (Tuple<Tuple<int, Terreno>, int> tuple in frontierEscaques)
      {
        Vector3 relativePosition = tuple.Item1.Item2.GetRelativePositionFromGlobalIndex(tuple.Item1);
        Tuple<int, Terreno> indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(-1, 0, 0));

        if (!influencedEscaques.ContainsKey(objetsAdministrator.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(-9, 0, 0), Quaternion.identity);
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(1, 0, 0));
        if (!influencedEscaques.ContainsKey(objetsAdministrator.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(9, 0, 0), Quaternion.identity);
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(0, 0, -1));
        if (!influencedEscaques.ContainsKey(objetsAdministrator.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(0, 0, -9), Quaternion.Euler(new Vector3(0, -90, 0)));
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(0, 0, 1));
        if (!influencedEscaques.ContainsKey(objetsAdministrator.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(0, 0, 9), Quaternion.Euler(new Vector3(0, 90, 0)));
        }
      }
      frontierEscaques.Clear();
    }
  }

  private void PutFrontierInEscaque(Tuple<Tuple<int, Terreno>, int> tuple, Vector3 offset, Quaternion rotation)
  {
    Vector3 position = tuple.Item1.Item2.GetGlobalPositionFromGlobalIndex(tuple.Item1) + offset;
    if (objetsAdministrator.frontiers.ContainsKey(tuple.Item2))
    {
      objetsAdministrator.frontiers[tuple.Item2].Add(Instantiate(prefabFrontier, position, rotation));
    }
    else
    {
      List<GameObject> fronteritas = new()
            {
                Instantiate(prefabFrontier, position,rotation)
            };
      objetsAdministrator.frontiers[tuple.Item2] = fronteritas;
    }
  }
  private IEnumerator InvokeBueno(Terreno terreno)
  {
    yield return new WaitForSeconds(1f);
    objetsAdministrator.GenerateRandomResource(terreno);
  }
  private IEnumerator ReturnToOriginal(Tuple<int, Terreno> tuple)
  {
    yield return new WaitForSeconds(1f);
    tuple.Item2.ReturnPixelToOriginal(tuple.Item1);
  }

  public void PaintInfluence()
  {
    foreach (var pair in influencedEscaques)
    {
      Tuple<int, Terreno> globalIndex = objetsAdministrator.GetIndexFromNumeric(pair.Key);
      globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.magenta);
      StartCoroutine(ReturnToOriginal(globalIndex));
    }
  }
  void SetNeighboorsReference()
  {
    vecindario.Add(0, new Vector3(-sizeOfTerrain, 0, -sizeOfTerrain));
    vecindario.Add(1, new Vector3(-sizeOfTerrain, 0, 0));
    vecindario.Add(2, new Vector3(-sizeOfTerrain, 0, sizeOfTerrain));
    vecindario.Add(3, new Vector3(0, 0, -sizeOfTerrain));
    vecindario.Add(4, new Vector3(0, 0, sizeOfTerrain));
    vecindario.Add(5, new Vector3(sizeOfTerrain, 0, -sizeOfTerrain));
    vecindario.Add(6, new Vector3(sizeOfTerrain, 0, 0));
    vecindario.Add(7, new Vector3(sizeOfTerrain, 0, sizeOfTerrain));
  }

  /// <summary>
  /// Creacion del primer grupo de planos que entregan la formacion inicial de terreno
  /// </summary>
  private void CreateFirstTerrain()
  {
    for (int i = -2; i < 3; i++)
    {
      for (int j = -2; j < 3; j++)
      {
        CreateTerrain(new(0 + i * sizeOfTerrain, 0, 0 + j * sizeOfTerrain));
      }
    }
  }

  /// <summary>
  /// Funcion que retorna el terreno en el que esta parado el objeto que llamo a la funcion 
  /// </summary>
  /// <param name="position"></param>
  /// <returns></returns>
  public Terreno InWhatTerrenoAmI(Vector3 position)
  {
    Vector3 relativePosition = position / sizeOfTerrain;
    Vector3 terrainPosition = new((int)relativePosition.x * sizeOfTerrain, 0, (int)relativePosition.z * sizeOfTerrain);

    if (terrainDict.ContainsKey(terrainPosition))
    {
      terrenoOfHero = terrainDict[terrainPosition].GetComponent<Terreno>();
      return terrenoOfHero;
    }

    return null;
  }

  /// <summary>
  /// En la creacion de un terreno, agregar todos los vecinos que este tenga. Reconociendo por su posicion
  /// </summary>
  /// <param name="scripTerreno">Instancia del script del terreno recien creado</param>
  /// <param name="posNewTerreno">posicion del nuevo terreno</param>
  /// <returns></returns>
  private void ConnectWithNeighboors(Terreno scripTerreno, Vector3 posNewTerreno)
  {
    Terreno[] terr = scripTerreno.neighboors;

    for (int i = 0; i < terr.Length; i++)
    {
      if (terr[i] == null)
      {
        if (terrainDict.ContainsKey(posNewTerreno + vecindario[i]))
        {
          Terreno temp = terrainDict[posNewTerreno + vecindario[i]].GetComponent<Terreno>();
          terr[i] = temp;
          temp.neighboors[7 - i] = scripTerreno;
        }
      }
    }
  }

  public Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
  {
    return terrenoOfHero.CalculateDistance(actualPosition, destiny);
  }

  public Vector3 MoveHero(Vector3 position, Vector3 movement)
  {

    positionHero = terrenoOfHero.Move(position, movement);
    return positionHero;
  }

  public void SetTerrenoOfHero(Terreno terreno)
  {
    terrenoOfHero = terreno;
    FillNeighborhood(terreno);

    foreach (Terreno item in terreno.neighboors)
    {
      FillNeighborhood(item);
    }


  }

  private void FillNeighborhood(Terreno terreno)
  {
    Terreno[] neighboor = terreno.neighboors;
    Vector3 position = terreno.GetPosition();

    for (int i = 0; i < neighboor.Length; i++)
    {
      if (neighboor[i] == null)
      {
        CreateTerrain(position + vecindario[i]);
      }
    }

  }
  public bool IsTerrainActive(Terreno terreno)
  {
    return terrenoOfHero.id == terreno.id;
  }

  public bool IsThisEscaqueVisited(Tuple<int, Terreno> visitedEscaque)
  {
    List<Tuple<int, Terreno>> visitedEscaques = sorroundingEscaques;
    foreach (Tuple<int, Terreno> escaque in visitedEscaques)
    {
      if (CompareTwoEscaques(visitedEscaque, escaque))
      {
        return true;
      }
    }
    return false;
  }

  private void CreateTerrain(Vector3 position)
  {
    GameObject newTerreno = Instantiate(Terreno, position, Quaternion.identity);
    newTerreno.transform.SetParent(containerTerrenos.transform);
    newTerreno.transform.localScale = new Vector3(sizeTerrainInVertices, sizeTerrainInVertices, sizeTerrainInVertices);

    Terreno scriptNewTerreno = newTerreno.GetComponent<Terreno>();
    ConnectWithNeighboors(scriptNewTerreno, position);
    scriptNewTerreno.id = countTerrain;
    terrainDict.Add(position, newTerreno);
    idTerrainDict.Add(countTerrain, scriptNewTerreno);

    TerrainGeneration scriptGeneration = newTerreno.GetComponent<TerrainGeneration>();
    scriptGeneration.heightTerrainTypes = terrainTypes;
    scriptGeneration.heightMultiplier = heightMultiplier;
    scriptGeneration.heightCurve = heightCurve;
    scriptGeneration.waves = waves;

    countTerrain++;

    terrenosWithoutResources.Add(scriptNewTerreno);
  }


  public bool CompareTwoEscaques(Tuple<int, Terreno> escaque1, Tuple<int, Terreno> escaque2)
  {
    bool item1 = false;
    bool item2 = false;

    if (escaque1.Item1 == escaque2.Item1) item1 = true;
    if (escaque1.Item2.id == escaque1.Item2.id) item2 = true;

    return item1 && item2;
  }
  public int GetSizeEscaque()
  {
    return sizeEscaque;
  }
  public int GetSizeTerrainInVertices()
  {
    return sizeTerrainInVertices;
  }

}
