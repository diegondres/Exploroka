using System;
using System.Collections.Generic;
using UnityEngine;

public class SubTerrainAdmGeneration : MonoBehaviour
{
  [SerializeField]
  private GameObject prefabTerreno;
  [SerializeField]
  private TerrainType[] terrainTypes;
  public float heightMultiplier;
  [SerializeField]
  private AnimationCurve heightCurve;
  [SerializeField]
  private Wave[] waves;
  public readonly Dictionary<int, Terreno> idTerrainDict = new();
  private int countTerrain = 0;
  [SerializeField]
  private GameObject containerTerrenos;
  private readonly Dictionary<int, Vector3> vecindario = new();

  public void SetNeighboorsReference()
  {
    vecindario.Add(0, new Vector3(-SubTerrainAdmReference.sizeOfTerrain, 0, -SubTerrainAdmReference.sizeOfTerrain));
    vecindario.Add(1, new Vector3(-SubTerrainAdmReference.sizeOfTerrain, 0, 0));
    vecindario.Add(2, new Vector3(-SubTerrainAdmReference.sizeOfTerrain, 0, SubTerrainAdmReference.sizeOfTerrain));
    vecindario.Add(3, new Vector3(0, 0, -SubTerrainAdmReference.sizeOfTerrain));
    vecindario.Add(4, new Vector3(0, 0, SubTerrainAdmReference.sizeOfTerrain));
    vecindario.Add(5, new Vector3(SubTerrainAdmReference.sizeOfTerrain, 0, -SubTerrainAdmReference.sizeOfTerrain));
    vecindario.Add(6, new Vector3(SubTerrainAdmReference.sizeOfTerrain, 0, 0));
    vecindario.Add(7, new Vector3(SubTerrainAdmReference.sizeOfTerrain, 0, SubTerrainAdmReference.sizeOfTerrain));
  }


  public void CreateFirstTerrain()
  {
    for (int i = -2; i < 3; i++)
    {
      for (int j = -2; j < 3; j++)
      {
        CreateTerrain(new(0 + i * SubTerrainAdmReference.sizeOfTerrain, 0, 0 + j * SubTerrainAdmReference.sizeOfTerrain), SubTerrainAdmReference.sizeTerrainInVertices);
      }
    }
  }

  public void CreateTerrain(Vector3 position, int sizeTerrainInVertices)
  {
    GameObject newTerreno = Instantiate(prefabTerreno, position, Quaternion.identity, containerTerrenos.transform);
    newTerreno.transform.localScale = new Vector3(sizeTerrainInVertices, sizeTerrainInVertices, sizeTerrainInVertices);

    Terreno scriptNewTerreno = newTerreno.GetComponent<Terreno>();
    ConnectWithNeighboors(scriptNewTerreno, position);
    scriptNewTerreno.id = countTerrain;
    SubTerrainAdmReference.terrainDict.Add(position, newTerreno);
    idTerrainDict.Add(countTerrain, scriptNewTerreno);

    TerrainGeneration scriptGeneration = newTerreno.GetComponent<TerrainGeneration>();
    scriptGeneration.heightTerrainTypes = terrainTypes;
    scriptGeneration.heightMultiplier = heightMultiplier;


    countTerrain++;
  }

  private void ConnectWithNeighboors(Terreno scripTerreno, Vector3 posNewTerreno)
  {
    Terreno[] terr = scripTerreno.neighboors;

    for (int i = 0; i < terr.Length; i++)
    {
      if (terr[i] == null)
      {
        if (SubTerrainAdmReference.terrainDict.ContainsKey(posNewTerreno + vecindario[i]))
        {
          Terreno temp = SubTerrainAdmReference.terrainDict[posNewTerreno + vecindario[i]].GetComponent<Terreno>();
          terr[i] = temp;
          temp.neighboors[7 - i] = scripTerreno;
        }
      }
    }
  }

  public void FillNeighborhood(Terreno terreno)
  {
    Terreno[] neighboor = terreno.neighboors;
    Vector3 position = terreno.GetPosition();

    for (int i = 0; i < neighboor.Length; i++)
    {
      if (neighboor[i] == null)
      {
        CreateTerrain(position + vecindario[i], SubTerrainAdmReference.sizeTerrainInVertices);
      }
    }
  }
  public Terreno GetTerrenoScriptFromId(int id)
  {
    return idTerrainDict[id];
  }
}
