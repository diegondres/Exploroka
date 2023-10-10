using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class Heroe : MonoBehaviour
{
  //[SerializeField]
  //private float velocidad = 5f; // Velocidad de movimiento del personaje
  [SerializeField]
  private float minimumTime = 0.0f;
  [SerializeField]
 // private float longRayCast = 10f;

  private Camera camara;
  private Vector3 destino;
  private float distancia = 0.0f;
  private TerrainAdministrator terrainAdministrator;
  private Vector3 movement = Vector3.zero;
  private float rotation;
  private Vector3 distanciaEnVector = Vector3.zero;
  private float acumulatedTime = 0.0f;


  void Start()
  {
    destino = transform.position;
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    terrainAdministrator.InWhatTerrenoAmI(transform.position);
    camara = FindAnyObjectByType<Camera>();
    //MoveHero(movement, 0.0f);
  }

  void Update()
  {
    acumulatedTime += Time.deltaTime;

    ArrowMoving();

    if (Input.GetMouseButtonDown(1)) 
    {
      Vector3 posicionMouse = Input.mousePosition;
      Ray rayo = camara.ScreenPointToRay(posicionMouse);
      Plane plano = new(Vector3.up, transform.position);
      distancia = 0.0f;

      if (plano.Raycast(rayo, out distancia)){   
        destino = rayo.GetPoint(distancia);
      }
      distanciaEnVector = terrainAdministrator.CalculateDistance(transform.position, destino);
    }

    if (Vector3.Magnitude(distanciaEnVector) > 0.1f && acumulatedTime >= minimumTime)
    {
      MouseMoving(distanciaEnVector);
      distanciaEnVector = terrainAdministrator.CalculateDistance(transform.position, destino);
      acumulatedTime = 0.0f;
    }
   
  }
  
  private void MoveHero(Vector3 movement, float rotation)
  {    
    transform.position = terrainAdministrator.MoveHero(transform.position, movement);
    transform.eulerAngles = new Vector3 (0, rotation, 0);
  }

  private void ArrowMoving()
  {
    bool anyMove = false;
    rotation = 0.0f;
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    {
      movement = new Vector3(0, 0, terrainAdministrator.sizeEscaque);
      rotation = 0.0f;
      anyMove = true;
    }
    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    {
      movement = new Vector3(0, 0, -terrainAdministrator.sizeEscaque);
      rotation = 180f;
      anyMove = true;
    }
    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    {
      movement = new Vector3(terrainAdministrator.sizeEscaque, 0, 0);
      rotation = 90f;
      anyMove = true;
    }
    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    {
      movement = new Vector3(-terrainAdministrator.sizeEscaque, 0, 0);
      rotation = 270f;
      anyMove = true;
    }
    if (anyMove)
    {
      MoveHero(movement, rotation);
      distanciaEnVector = Vector3.zero;

    }
  }

  private void MouseMoving(Vector3 distance){
    if(Mathf.Abs(distance.x) > Mathf.Abs(distance.z)){
      if (distance.x < 0)
      {
        movement = new Vector3(terrainAdministrator.sizeEscaque, 0, 0);
        rotation = 90f;
      }
      else
      {
        movement = new Vector3(-terrainAdministrator.sizeEscaque, 0, 0);
        rotation = 270f;
      }
    }
    else{
      if (distance.z < 0)
      {
        movement = new Vector3(0, 0,terrainAdministrator.sizeEscaque);
        rotation = 0.0f;
      }
      else
      {
        movement = new Vector3(0, 0, -terrainAdministrator.sizeEscaque);
        rotation = 180f;
      }
    }
    MoveHero(movement, rotation);
  }

  /*
  private Terreno DetectTerrenoDown(){
    Vector3 origen = new(transform.position.x, 0.1f, transform.position.z);
    Ray rayito = new(origen, Vector3.down);

    if (Physics.Raycast(rayito, out RaycastHit hitInfo, longRayCast))
    {
      if (hitInfo.collider.gameObject.TryGetComponent<Terreno>(out var terreno))
      {
        return terreno;
      }
      else
      {
        return null;
      }
    }
    return null;
  }

  public void UpdateTerrainReference(){
    tempPlanes = DetectTerrenoDown();
  }
  */
}
