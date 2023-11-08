using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class TerrainAdministrator : MonoBehaviour
{
  //TERRENOS
  private readonly Dictionary<Vector3, GameObject> terrainDict = new();
  static int countTerrain = 0;
  [SerializeField]
  private BiomeRow[] biomes;
  [SerializeField]
  private GameObject Terreno;
  [SerializeField]
  private GameObject container;
  [SerializeField]
  private TerrainType[] terrainTypes;
  [SerializeField]
  private TerrainType[] heatTerrainTypes;
  [SerializeField]
  private TerrainType[] moistureTerrainTypes;
  public float heightMultiplier;
  [SerializeField]
  private AnimationCurve heightCurve;
  [SerializeField]
  private AnimationCurve heatCurve;
  [SerializeField]
  private AnimationCurve moistureCurve;
  [SerializeField]
  private Wave[] waves;
  [SerializeField]
  private Wave[] heatWaves;

  [SerializeField]
  private Wave[] moistureWaves;

  //////////////  HEROE   /////////////////////////////
  [SerializeField]
  private Heroe heroe;
  private Terreno terrenoOfHero;
  public int sizeOfTerrain = 200;
  private int sizeEscaque;
  private int sizeTerrainInVertices;
  private Dictionary<int, Vector3> vecindario = new();
  public List<Tuple<int, Terreno>> sorroundingEscaques = new();

  //CONSTRUCCION
  private Tuple<int, Terreno> buildingGlobalIndex;
  public bool isBuildingLocationSelected = false;


  void Awake()
  {
    sizeEscaque = sizeOfTerrain / 20;
    sizeTerrainInVertices = sizeOfTerrain / 20;
    SetNeighboorsReference();

    CreateFirstTerrain();

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

  void Update()
  {

  }

  /// <summary>
  /// Creacion del primer grupo de planos que entregan la formacion inicial de terreno
  /// </summary>
  private void CreateFirstTerrain()
  {
    for (int i = -1; i < 2; i++)
    {
      for (int j = -1; j < 2; j++)
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
    return terrenoOfHero.Move(position, movement);
  }

  public void SetTerrenoOfHero(Terreno terreno)
  {
    terrenoOfHero = terreno;
    FillNeighborhood(terreno);
  }

  private void FillNeighborhood(Terreno terreno)
  {
    Terreno[] terr = terreno.neighboors;
    Vector3 position = terreno.GetPosition();

    for (int i = 0; i < terr.Length; i++)
    {
      if (terr[i] == null)
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
      if (CompareToEscaques(visitedEscaque, escaque))
      {
        return true;
      }
    }
    return false;
  }

   public void SelectEscaqueToBuildIn(Tuple<int, Terreno> globalIndex)
  {
    
    if (IsThisEscaqueVisited(globalIndex))
    {
      if(isBuildingLocationSelected){
        if (globalIndex.Item1 == buildingGlobalIndex.Item1 ){
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

  private void CreateTerrain(Vector3 position)
  {
    GameObject newTerreno = Instantiate(Terreno, position, Quaternion.identity);
    newTerreno.transform.SetParent(container.transform);
    newTerreno.transform.localScale = new Vector3(sizeTerrainInVertices, sizeTerrainInVertices, sizeTerrainInVertices);

    Terreno scriptNewTerreno = newTerreno.GetComponent<Terreno>();
    ConnectWithNeighboors(scriptNewTerreno, position);
    scriptNewTerreno.id = countTerrain;
    terrainDict.Add(position, newTerreno);

    TerrainGeneration scriptGeneration = newTerreno.GetComponent<TerrainGeneration>();
    scriptGeneration.heightTerrainTypes = terrainTypes;
    scriptGeneration.heightMultiplier = heightMultiplier;
    scriptGeneration.heightCurve = heightCurve;
    scriptGeneration.waves = waves;

    countTerrain++;
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

  public bool CompareToEscaques(Tuple<int, Terreno> tuple1, Tuple<int, Terreno> tuple2)
  {
    bool item1 = false;
    bool item2 = false;

    if (tuple1.Item1 == tuple2.Item1) item1 = true;
    if (tuple1.Item2.id == tuple1.Item2.id) item2 = true;

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
