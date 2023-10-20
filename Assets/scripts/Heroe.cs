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
  private float moveDuration = 0.1f;
  private bool isMoving = false;

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

      if (plano.Raycast(rayo, out distancia))
      {
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
    transform.eulerAngles = new Vector3(0, rotation, 0);
  }

  private void ArrowMoving()
  {
    rotation = 0.0f;
    movement = Vector3.zero;
    if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isMoving)
    {
      movement = new Vector3(0, 0, terrainAdministrator.sizeEscaque);
      rotation = 0.0f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isMoving)
    {
      movement = new Vector3(0, 0, -terrainAdministrator.sizeEscaque);
      rotation = 180f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isMoving)
    {
      movement = new Vector3(terrainAdministrator.sizeEscaque, 0, 0);
      rotation = 90f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isMoving)
    {
      movement = new Vector3(-terrainAdministrator.sizeEscaque, 0, 0);
      rotation = 270f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if (isMoving)
    {
      distanciaEnVector = Vector3.zero;
    }
  }

  private void MouseMoving(Vector3 distance)
  {
    if (Mathf.Abs(distance.x) > Mathf.Abs(distance.z))
    {
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
    else
    {
      if (distance.z < 0)
      {
        movement = new Vector3(0, 0, terrainAdministrator.sizeEscaque);
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

  private IEnumerator ContinuosMove(Vector3 movement, float rotation)
  {
    isMoving = true;

    Vector3 startPosition = transform.position;
    Vector3 endPosition = transform.position + movement;

    float elapsedTime = 0f;
    while (elapsedTime < moveDuration)
    {
      elapsedTime += Time.deltaTime;
      float percent = elapsedTime / moveDuration;
      transform.position = Vector3.Lerp(startPosition, endPosition, percent);

      yield return null;
    }

    transform.position = endPosition;

    MoveHero(Vector3.zero, rotation);
    isMoving = false;
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