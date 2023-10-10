using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terreno : MonoBehaviour
{
  private MeshRenderer tileRenderer;
  private Camera camara;
  public int sizeSquare = 10;
  private Renderer rend;
  private List<int> visitedEscaques = new();
  private Color32[] originalPixels;

  private Color32[] actualPixels;

  //Variables de construccion
  private bool isBuildingLocationSelected = false;
  private Vector3 positionToBuildIn;

  //Variables de referencias cercanas
  [SerializeField]
  public Terreno[] neighboors = new Terreno[8];
  
  public TerrainAdministrator terrainAdministrator;
  public int id;

  void Start()
  {
    EnCuadriculas();
    camara = FindAnyObjectByType<Camera>();
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 posicionMouse = Input.mousePosition;
      Plane plano = new(Vector3.up, transform.position);
      Vector3 mousePosition;

      Ray rayo = camara.ScreenPointToRay(posicionMouse);

      if (plano.Raycast(rayo, out float distancia))
      {
        mousePosition = rayo.GetPoint(distancia);
        Vector3 relativePosition = mousePosition - transform.parent.TransformPoint(transform.localPosition);
        //Consideracion para cuando se salga del terreno

        Vector3 centerPosition = CenterInEscaqueToGlobal(relativePosition / sizeSquare);
        relativePosition = (centerPosition - transform.parent.TransformPoint(transform.localPosition)) / sizeSquare;

        SelectEscaqueToBuildIn(relativePosition);
      }
    }


  }

  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  void EnCuadriculas()
  {
    rend = GetComponent<Renderer>();
    tileRenderer = GetComponent<MeshRenderer>();

    Texture2D tileTexture = BuildTexture(sizeSquare * 2, sizeSquare * 2);
    tileRenderer.material.mainTexture = tileTexture;
  }

  private Texture2D BuildTexture(int tileDepth, int tileWidth)
  {
    actualPixels = new Color32[tileDepth * tileWidth];
    Color32 color1 = id % 2 == 0 ? Color.black : Color.magenta;
    Color32 color2 = id % 2 == 0 ? Color.cyan : Color.white;
    int color = 0;
    for (int zIndex = 0; zIndex < tileDepth; zIndex++)
    {
      color = color == 0 ? 1 : 0;
      for (int xIndex = 0; xIndex < tileWidth; xIndex++)
      {
        // transform the 2D map index is an Array index
        int colorIndex = zIndex * tileWidth + xIndex;
        if (color == 0)
        {
          actualPixels[colorIndex] = color1;
          color = 1;
        }
        else
        {
          actualPixels[colorIndex] = color2;
          color = 0;
        }
      }

    }
    // create a new texture and set its pixel colors
    Texture2D tileTexture = new(tileWidth, tileDepth)
    {
      wrapMode = TextureWrapMode.Repeat,
      filterMode = FilterMode.Point
    };
    originalPixels = actualPixels;
    tileTexture.SetPixels32(actualPixels);

    tileTexture.Apply();
    return tileTexture;
  }

  //////////////////////////////----------START-----------//////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
      isBuildingLocationSelected = false;

      return CenterInEscaqueToGlobal(relativePositionInVertices);
    }
    
  }
  private Vector3 CenterInEscaqueToGlobal(Vector3 positionHero)
  {
    Vector3 worldPosition = transform.parent.TransformPoint(transform.localPosition);
    float sizeSquareX = positionHero.x >= 0 ? sizeSquare / 2 : -sizeSquare / 2;
    float sizeSquareZ = positionHero.z >= 0 ? sizeSquare / 2 : -sizeSquare / 2;

    return new Vector3((int)positionHero.x * sizeSquare + worldPosition.x + sizeSquareX, 1.6f, (int)positionHero.z * sizeSquare + worldPosition.z + sizeSquareZ);
  }

  public Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
  {
    Vector3 relativeActualPosition = actualPosition - transform.parent.TransformPoint(transform.localPosition);
    Vector3 relativeActualPositionInVertices = relativeActualPosition / sizeSquare;

    relativeActualPosition = destiny - transform.parent.TransformPoint(transform.localPosition);
    Vector3 relativeDestinyInVertices = relativeActualPosition / sizeSquare;

    return CenterInEscaqueToGlobal(relativeActualPositionInVertices) - CenterInEscaqueToGlobal(relativeDestinyInVertices);
  }

  private void PrintSorroundingEscaques(Vector3 relativePositionInVertices)
  {
    int pixsLength = 400;

    CleanVisitedEscaques(terrainAdministrator.sorroundingEscaques); 

    for (int i = -1; i < 2; i++)
    {
      for (int j = -1; j < 2; j++)
      {
        if (i != 0 || j != 0)
        {
          terrainAdministrator.sorroundingEscaques.Add(GetIndexGlobal(pixsLength, new(relativePositionInVertices.x + i, relativePositionInVertices.y, relativePositionInVertices.z + j)));
        }
      }
    }
    foreach (Tuple<int,Terreno> escaque in terrainAdministrator.sorroundingEscaques)
    {
      escaque.Item2.PaintPixelInfluence(escaque.Item1, Color.red);
    }
  }

  void CleanVisitedEscaques(List<Tuple<int,Terreno>> terreno)
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
  
  private void SelectEscaqueToBuildIn(Vector3 position)
  {
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;
    Color32[] pixs = texture2D.GetPixels32();

    int pixsLength = pixs.Length;
    int index = GetIndex(pixsLength, position);

    if (IsThisEscaqueSelected(index) && !isBuildingLocationSelected)
    {
      isBuildingLocationSelected = true;
      positionToBuildIn = CenterInEscaqueToGlobal(position);
      pixs[index] = Color.green;

      texture2D.SetPixels32(pixs);
      texture2D.Apply();
    }

  }

  bool IsThisEscaqueSelected(int indexPosition)
  {
    foreach (int escaque in visitedEscaques)
    {
      if (indexPosition == escaque)
      {
        return true;
      }
    }
    return false;
  }

  //Esto deberia irse al administrador probablemente
  public Vector3 GetPositionToBuildIn()
  {
    isBuildingLocationSelected = false;
    return positionToBuildIn;
  }

  //////////////////////////////----------CONSTRUCCION-----------///////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  //////////////////////////////----------UTILES-----------///////////////////////////////////////////////////////

  public int GetIndex(int pixsLength, Vector3 relativePosition)
  {
    return pixsLength - (int)((relativePosition.z + sizeSquare) * sizeSquare * 2 + relativePosition.x) - 1;
  }

  Tuple<int, Terreno> GetIndexGlobal(int pixsLength, Vector3 relativePositionInVertices){
    Terreno terreno = GetTerrain(relativePositionInVertices) != null ? GetTerrain(relativePositionInVertices) : this;
    Vector3 globalPosition = CenterInEscaqueToGlobal(relativePositionInVertices);
    Vector3 relativePositionIV = terreno.GetRelativePositionInVertices(globalPosition);
    int index = terreno.GetIndex(pixsLength, relativePositionIV);
    
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

  public Vector3 GetRelativePositionInVertices(Vector3 globalPosition){
    Vector3 relativePosition = globalPosition - transform.parent.TransformPoint(transform.localPosition);
    return relativePosition / sizeSquare;
  } 

  public Vector3 GetPosition(){
    return transform.position;
  }
  //////////////////////////////----------UTILES-----------///////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  public void PaintPixelInfluence(int index, Color32 color){
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;
    Color32[] pixs = texture2D.GetPixels32();

    pixs[index] = Color.Lerp(originalPixels[index], color, 0.45f);

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }

  public void ReturnPixelToOriginal(int index){
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;
    Color32[] pixs = texture2D.GetPixels32();
       
    pixs[index] = originalPixels[index];

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }
  
  public void PaintAll(Color32[] pixs){
    Texture2D texture2D = (Texture2D)tileRenderer.material.mainTexture;

    texture2D.SetPixels32(pixs);
    texture2D.Apply();
  }

}
