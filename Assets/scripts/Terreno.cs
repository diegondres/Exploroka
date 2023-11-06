using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terreno : MonoBehaviour
{
  private int pixsLength;
  private Vector3 worldPositionTerrain;
  //TERRENOS VECINOS
  public Terreno[] neighboors = new Terreno[8];
  private TerrainAdministrator terrainAdministrator;
  private TerrainGeneration terrainGeneration;
  public int id;

  //GENERACION PROCEDURAL
  [NonSerialized]
  public Color32[] originalPixels;
  private MeshRenderer tileRenderer;


  //Tamaño de cada uno de los escaques
  private int sizeEscaque;
  //tamaño del terreno en cantidad de escaques
  private float sizeTerrainInVertices;


  void Start()
  {
    Inicialization();
  }
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  void Inicialization()
  {
    tileRenderer = GetComponent<MeshRenderer>();
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    sizeEscaque = terrainAdministrator.GetSizeEscaque();
    sizeTerrainInVertices = terrainAdministrator.GetSizeTerrainInVertices();
    terrainGeneration = GetComponent<TerrainGeneration>();

    worldPositionTerrain = transform.parent.TransformPoint(transform.localPosition);
  }

  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  // Update is called once per frame
  void Update()
  {
    /*if (Input.GetMouseButtonDown(0))
    {
      Vector3 posicionMouse = Input.mousePosition;
      Plane plano = new(Vector3.up, transform.position);
      Vector3 mousePosition;

      Ray rayo = camara.ScreenPointToRay(posicionMouse);

      if (plano.Raycast(rayo, out float distancia))
      {
        mousePosition = rayo.GetPoint(distancia);
        Vector3 relativePosition = mousePosition - transform.parent.TransformPoint(transform.localPosition);

        Vector3 centerPosition = CenterInEscaqueToGlobal(relativePosition / sizeEscaque);
        relativePosition = (centerPosition - transform.parent.TransformPoint(transform.localPosition)) / sizeEscaque;

        SelectEscaqueToBuildIn(relativePosition);
      }
    }*/
  }

  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------MOVIMIENTO-----------/////////////////////////////////////////////////////////

  public Vector3 Move(Vector3 position, Vector3 movement)
  {
    position += movement;
    Vector3 relativePositionInVertices = GetRelativePositionInVertices(position);
    
    if (GetTerrain(relativePositionInVertices) != null)
    {                                                                                                       
      Terreno neighboor = GetTerrain(relativePositionInVertices);
      terrainAdministrator.SetTerrenoOfHero(neighboor);

      return neighboor.Move(position, Vector3.zero);
    }
    else
    {
      PrintSorroundingEscaques(relativePositionInVertices);
      terrainAdministrator.isBuildingLocationSelected = false;

      return CenterInEscaqueToGlobal(relativePositionInVertices);
    }
  }
  private Vector3 CenterInEscaqueToGlobal(Vector3 relativePositionInVertices)
  {
    float sizeSquareX = relativePositionInVertices.x >= 0 ? sizeEscaque /2 : -sizeEscaque /2;
    float sizeSquareZ = relativePositionInVertices.z >= 0 ? sizeEscaque /2 : -sizeEscaque /2;
    float bodyHeight = 6f;

    return new Vector3((int)relativePositionInVertices.x * sizeEscaque + worldPositionTerrain.x + sizeSquareX, terrainGeneration.GetHeight(relativePositionInVertices) + bodyHeight, (int)relativePositionInVertices.z * sizeEscaque + worldPositionTerrain.z + sizeSquareZ);
  }

  private Vector3 GetGlobalPositionFromRelative(Vector3 relativePositionInVertices)
  {
    float sizeSquareX = relativePositionInVertices.x >= 0 ? sizeEscaque /2 : -sizeEscaque /2;
    float sizeSquareZ = relativePositionInVertices.z >= 0 ? sizeEscaque /2 : -sizeEscaque /2;
    
    return new Vector3((int)relativePositionInVertices.x * sizeEscaque + worldPositionTerrain.x + sizeSquareX, 0, (int)relativePositionInVertices.z * sizeEscaque + worldPositionTerrain.z + sizeSquareZ);
  }
  
  public Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
  {
    Vector3 relativeActualPosition = actualPosition - transform.parent.TransformPoint(transform.localPosition);
    Vector3 relativeActualPositionInVertices = relativeActualPosition / sizeEscaque;

    relativeActualPosition = destiny - transform.parent.TransformPoint(transform.localPosition);
    Vector3 relativeDestinyInVertices = relativeActualPosition / sizeEscaque;

    return CenterInEscaqueToGlobal(relativeActualPositionInVertices) - CenterInEscaqueToGlobal(relativeDestinyInVertices);
  }

  private void PrintSorroundingEscaques(Vector3 relativePositionInVertices)
  {

    CleanVisitedEscaques(terrainAdministrator.sorroundingEscaques);

    for (int i = -1; i < 2; i++)
    {
      for (int j = -1; j < 2; j++)
      {
        if (i != 0 || j != 0)
        {
          terrainAdministrator.sorroundingEscaques.Add(GetIndexGlobal(new(relativePositionInVertices.x + i, relativePositionInVertices.y, relativePositionInVertices.z + j)));
        }
      }
    }

    foreach (Tuple<int, Terreno> escaque in terrainAdministrator.sorroundingEscaques)
    {
      escaque.Item2.PaintPixelInfluence(escaque.Item1, Color.red);
    }
  }

  void CleanVisitedEscaques(List<Tuple<int, Terreno>> terreno)
  {
    foreach (Tuple<int, Terreno> escaque in terreno)
    {
      escaque.Item2.ReturnPixelToOriginal(escaque.Item1);
    }
    terreno.Clear();
  }
  //////////////////////////////----------MOVIMIENTO-----------/////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------CONSTRUCCION-----------///////////////////////////////////////////////////////

  private void SelectEscaqueToBuildIn(Vector3 relativePositionInVertices)
  {
    int index = GetIndex(relativePositionInVertices);
    Tuple<int, Terreno> globalIndex = GetIndexGlobal(relativePositionInVertices);

    if (IsThisEscaqueVisited(globalIndex) && !terrainAdministrator.isBuildingLocationSelected)
    {
      terrainAdministrator.isBuildingLocationSelected = true;
      terrainAdministrator.SetBuildingLocation(CenterInEscaqueToGlobal(relativePositionInVertices));
      globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.green);

    }
  }

  bool IsThisEscaqueVisited(Tuple<int, Terreno> visitedEscaque)
  {
    List<Tuple<int, Terreno>> visitedEscaques = terrainAdministrator.sorroundingEscaques;
    foreach (Tuple<int, Terreno> escaque in visitedEscaques)
    {
      if (terrainAdministrator.CompareToEscaques(visitedEscaque, escaque))
      {
        return true;
      }
    }
    return false;
  }


  //////////////////////////////----------CONSTRUCCION-----------///////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------UTILES-----------///////////////////////////////////////////////////////

  public int GetIndex(Vector3 relativePosition)
  {
    return  (int)((relativePosition.z + sizeTerrainInVertices / 2) * sizeTerrainInVertices + relativePosition.x);
  }

  Tuple<int, Terreno> GetIndexGlobal(Vector3 relativePositionInVertices)
  {
    Terreno terreno = GetTerrain(relativePositionInVertices) != null ? GetTerrain(relativePositionInVertices) : this;
    Vector3 globalPosition = GetGlobalPositionFromRelative(relativePositionInVertices);
    Vector3 relativePositionInVerticesInTerrain = terreno.GetRelativePositionInVertices(globalPosition);
    int index = terreno.GetIndex(relativePositionInVerticesInTerrain);

    return new Tuple<int, Terreno>(index, terreno);
  }
  public Terreno GetTerrain(Vector3 relativePositionInVertices)
  {
    if (relativePositionInVertices.x < -10)
    {
      if (relativePositionInVertices.z < -10) return neighboors[0];

      else if (relativePositionInVertices.z > 10) return neighboors[2];

      else return neighboors[1];
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

    pixs[index] = Color.Lerp(originalPixels[index], color, 0.45f);

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

  public void PaintAll(Color32[] pixs)
  {
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }

}
