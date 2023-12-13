using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terreno : MonoBehaviour
{

  private const float limitTime = 0.2f;
  private float acumulatedTime = 0f;
  bool isSorroundingEscaquesPainted = false;
  private Vector3 worldPositionTerrain;

  //TERRENOS VECINOS
  [NonSerialized]
  public Terreno[] neighboors = new Terreno[8];

  public int id;
  private TerrainAdministrator terrainAdministrator;
  private TerrainGeneration terrainGeneration;

  //GENERACION PROCEDURAL
  [NonSerialized]
  public Color32[] originalPixels;
  private MeshRenderer tileRenderer;

  //Tamaño de cada uno de los escaques
  private int sizeEscaque;
  //tamaño del terreno en cantidad de escaques
  private float sizeTerrainInVertices = 20;

  void Awake()
  {
    Inicialization();
  }
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  void Inicialization()
  {
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    sizeEscaque = SubTerrainAdmReference.sizeEscaque;
    sizeTerrainInVertices = SubTerrainAdmReference.sizeTerrainInVertices;
    tileRenderer = GetComponent<MeshRenderer>();
    terrainGeneration = GetComponent<TerrainGeneration>();

    worldPositionTerrain = transform.parent.TransformPoint(transform.localPosition);
  }

  public void IdleTime(float num)
  {
    acumulatedTime = num;
  }
  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  void Update()
  {
    if (SubTerrainAdmReference.IsTerrainActive(this))
    {
      if (acumulatedTime > limitTime && !isSorroundingEscaquesPainted)
      {
        foreach (Tuple<int, Terreno> escaque in SubTerrainAdmReference.GetSorroundingEscaques())
        {
          escaque.Item2.PaintPixelInfluence(escaque.Item1, Color.red);
        }
        isSorroundingEscaquesPainted = true;
      }
      else if (acumulatedTime <= limitTime)
      {
        IdleTime(acumulatedTime + Time.deltaTime);
        isSorroundingEscaquesPainted = false;
      }

    }
  }

  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------MOVIMIENTO-----------/////////////////////////////////////////////////////////

  public Vector3 Move(Vector3 position, Vector3 movement)
  {
    IdleTime(0);
    position += movement;
    Vector3 relativePositionInVertices = GetRelativePositionInVertices(position);


    if (GetTerrain(relativePositionInVertices) != null)
    {
      Terreno neighboor = GetTerrain(relativePositionInVertices);
      terrainAdministrator.SetTerrenoOfHero(neighboor);

      return neighboor.Move(position - movement, movement);
    }
    else
    {
      if (IsWalkable(relativePositionInVertices))
      {
        AddSorroundingEscaques(relativePositionInVertices);
        SubObjectsAdmReferences.isBuildingLocationSelected = false;

        return CenterInEscaqueToGlobal(relativePositionInVertices, 5f);
      }
      else
      {
        return Move(position - movement, Vector3.zero);
      }
    }
  }

  public bool IsWalkable(Vector3 relativePositionInVertices)
  {
    return terrainGeneration.GetTerrainType(relativePositionInVertices) != "water";
  }

  public Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
  {
    Vector3 relativeActualPositionInVertices = GetRelativePositionInVertices(actualPosition);
    Vector3 relativeDestinyInVertices = GetRelativePositionInVertices(destiny);

    Tuple<int, Terreno> actualPositionGlobalIndex = GetIndexGlobal(relativeActualPositionInVertices);
    Tuple<int, Terreno> destinyGlobalIndex = GetIndexGlobal(relativeDestinyInVertices);

    return GetGlobalPositionFromGlobalIndexWithoutHeight(actualPositionGlobalIndex) - GetGlobalPositionFromGlobalIndexWithoutHeight(destinyGlobalIndex);
  }

  private void AddSorroundingEscaques(Vector3 relativePositionInVertices)
  {
    SubTerrainAdmReference.CleanVisitedEscaques();

    for (int i = -1; i < 2; i++)
    {
      for (int j = -1; j < 2; j++)
      {
        if (i != 0 || j != 0)
        {
          SubTerrainAdmReference.AddSorroundinEscaque(GetIndexGlobal(new(relativePositionInVertices.x + i, relativePositionInVertices.y, relativePositionInVertices.z + j)));
        }
      }
    }
  }


  //////////////////////////////----------MOVIMIENTO-----------/////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------UTILES-----------///////////////////////////////////////////////////////

  public int GetIndex(Vector3 relativePosition)
  {
    return (int)((relativePosition.z + sizeTerrainInVertices / 2) * sizeTerrainInVertices + relativePosition.x);
  }
  public Tuple<int, Terreno> GetIndexGlobal(Vector3 relativePositionInVertices)
  {
    Terreno terreno = GetTerrain(relativePositionInVertices) != null ? GetTerrain(relativePositionInVertices) : this;
    Vector3 globalPosition = GetGlobalPositionFromRelative(relativePositionInVertices);

    Vector3 relativePositionInVerticesInTerrain = terreno.GetRelativePositionInVertices(globalPosition);

    int index = terreno.GetIndex(relativePositionInVerticesInTerrain);

    return new Tuple<int, Terreno>(index, terreno);
  }

  public Vector3 CenterInEscaqueToGlobal(Vector3 relativePositionInVertices, float offsetY)
  {
    float sizeSquareX = relativePositionInVertices.x >= 0 ? sizeEscaque / 2 : -sizeEscaque / 2;
    float sizeSquareZ = relativePositionInVertices.z >= 0 ? sizeEscaque / 2 : -sizeEscaque / 2;

    return new Vector3((int)relativePositionInVertices.x * sizeEscaque + worldPositionTerrain.x + sizeSquareX, terrainGeneration.GetHeight(relativePositionInVertices) + offsetY, (int)relativePositionInVertices.z * sizeEscaque + worldPositionTerrain.z + sizeSquareZ);
  }

  private Vector3 GetGlobalPositionFromRelative(Vector3 relativePositionInVertices)
  {
    float sizeSquareX = relativePositionInVertices.x >= 0 ? sizeEscaque / 2 : -sizeEscaque / 2;
    float sizeSquareZ = relativePositionInVertices.z >= 0 ? sizeEscaque / 2 : -sizeEscaque / 2;

    return new Vector3((int)relativePositionInVertices.x * sizeEscaque + worldPositionTerrain.x + sizeSquareX, 0, (int)relativePositionInVertices.z * sizeEscaque + worldPositionTerrain.z + sizeSquareZ);
  }

  public Vector3 GetGlobalPositionFromGlobalIndex(Tuple<int, Terreno> globalIndex)
  {
    int ejeX = (int)(globalIndex.Item1 / sizeTerrainInVertices);
    float wea2 = globalIndex.Item1 / (sizeTerrainInVertices / 2);
    float relativePositionX = globalIndex.Item1 % 200 > 0 ? ejeX - sizeTerrainInVertices / 2 + 0.5f : ejeX - sizeTerrainInVertices / 2 + 1 - 0.5f;
    float relativePositionZ = wea2 % 2 == 0 ? globalIndex.Item1 - ejeX * sizeTerrainInVertices - (sizeTerrainInVertices / 2 - 1) - 0.5f : globalIndex.Item1 - ejeX * sizeTerrainInVertices - sizeTerrainInVertices / 2 + 0.5f;

    return globalIndex.Item2.CenterInEscaqueToGlobal(new Vector3(relativePositionZ, 0f, relativePositionX), 5f);
  }
  public Vector3 GetGlobalPositionFromGlobalIndexWithoutHeight(Tuple<int, Terreno> globalIndex)
  {
    int ejeX = (int)(globalIndex.Item1 / sizeTerrainInVertices);
    float wea2 = globalIndex.Item1 / (sizeTerrainInVertices / 2);
    float relativePositionX = globalIndex.Item1 % 200 > 0 ? ejeX - sizeTerrainInVertices / 2 + 0.5f : ejeX - sizeTerrainInVertices / 2 + 1 - 0.5f;
    float relativePositionZ = wea2 % 2 == 0 ? globalIndex.Item1 - ejeX * sizeTerrainInVertices - (sizeTerrainInVertices / 2 - 1) - 0.5f : globalIndex.Item1 - ejeX * sizeTerrainInVertices - sizeTerrainInVertices / 2 + 0.5f;

    return globalIndex.Item2.GetGlobalPositionFromRelative(new Vector3(relativePositionZ, 0f, relativePositionX));
  }
  public Vector3 GetRelativePositionFromGlobalIndex(Tuple<int, Terreno> globalIndex)
  {
    int ejeX = (int)(globalIndex.Item1 / sizeTerrainInVertices);
    float wea2 = globalIndex.Item1 / (sizeTerrainInVertices / 2);
    float relativePositionX = globalIndex.Item1 % 200 > 0 ? ejeX - sizeTerrainInVertices / 2 + 0.5f : ejeX - sizeTerrainInVertices / 2 + 1 - 0.5f;
    float relativePositionZ = wea2 % 2 == 0 ? globalIndex.Item1 - ejeX * sizeTerrainInVertices - (sizeTerrainInVertices / 2 - 1) - 0.5f : globalIndex.Item1 - ejeX * sizeTerrainInVertices - sizeTerrainInVertices / 2 + 0.5f;

    return new Vector3(relativePositionZ, 0f, relativePositionX);
  }

  public Terreno GetTerrain(Vector3 relativePositionInVertices)
  {
    if (relativePositionInVertices.x < -10)
    {
      if (relativePositionInVertices.z < -10)
      {
        return neighboors[0];
      }

      else if (relativePositionInVertices.z > 10)
      {
        return neighboors[2];
      }

      else
      {
        return neighboors[1];
      }
    }
    else if (relativePositionInVertices.x > 10)
    {
      if (relativePositionInVertices.z < -10) return neighboors[5];

      else if (relativePositionInVertices.z > 10) return neighboors[7];

      else return neighboors[6];
    }
    else
    {
      if (relativePositionInVertices.z < -10) return neighboors[3];

      else if (relativePositionInVertices.z > 10) return neighboors[4];

      else return null;
    }

  }

  public Vector3 GetRelativePositionInVertices(Vector3 globalPosition)
  {
    Vector3 relativePosition = globalPosition - worldPositionTerrain;

    return relativePosition / sizeTerrainInVertices;
  }

  public Vector3 GetPosition()
  {
    return transform.position;
  }


  //////////////////////////////----------UTILES-----------///////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void PaintPixelInfluence(int index, Color32 color)
  {
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;
    Color32[] pixs = texture2D.GetPixels32();

    pixs[index] = Color.Lerp(originalPixels[index], color, 0.35f);

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }
  public void ReturnPixelToOriginal(int index)
  {
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;
    Color32[] pixs = texture2D.GetPixels32();

    pixs[index] = originalPixels[index];

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }
  public void UpdateOriginalPixel(int index, Color32 color)
  {
    originalPixels[index] = Color.Lerp(originalPixels[index], color, 0.35f); ;
  }

  public void PaintAll(Color32[] pixs)
  {
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }
}
