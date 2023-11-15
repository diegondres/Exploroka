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
  private int sizeEscaque;

  void Start()
  {
    destino = transform.position;
    terrainAdministrator = GameObject.FindGameObjectWithTag("Respawn").GetComponent<TerrainAdministrator>();
        terrainAdministrator.InWhatTerrenoAmI(transform.position);
    sizeEscaque = terrainAdministrator.GetSizeEscaque();
    camara = GameObject.FindGameObjectWithTag("Finish").GetComponent<Camera>();
    //TODO: se tiene que hacer un arreglo para que el personaje inicie en una referencia correcta del terreno
    //MoveHero(movement, 0.0f);
  }

  void Update()
  {
    acumulatedTime += Time.deltaTime;

    ArrowMoving();

    if (Input.GetMouseButtonDown(1))
    {
      Ray rayo = camara.ScreenPointToRay(Input.mousePosition);
      Plane plano = new(Vector3.up, transform.position);
      distancia = 0.0f;

      if (plano.Raycast(rayo, out distancia))
      {
        destino = rayo.GetPoint(distancia);
      }
      distanciaEnVector = terrainAdministrator.CalculateDistance(transform.position, destino);
    }

    if (Vector3.Magnitude(distanciaEnVector) > 2.8f && acumulatedTime >= minimumTime)
    {
      MouseMoving(distanciaEnVector);
      distanciaEnVector = terrainAdministrator.CalculateDistance(transform.position, destino);
      acumulatedTime = 0.0f;
    }

  }

  private void MoveHero(Vector3 movement, float rotation)
  {
    //TODO: cuando se solucione el movimiento con el mouse, esta funcion deberia desaparecer y solo dejar la corutina.
    transform.position = terrainAdministrator.MoveHero(transform.position, movement);
    transform.eulerAngles = new Vector3(0, rotation, 0);
  }

  private void ArrowMoving()
  {
    rotation = 0.0f;
    movement = Vector3.zero;
    if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isMoving)
    {
      movement = new Vector3(0, 0, sizeEscaque);
      rotation = 0.0f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isMoving)
    {
      movement = new Vector3(0, 0, -sizeEscaque);
      rotation = 180f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isMoving)
    {
      movement = new Vector3(sizeEscaque, 0, 0);
      rotation = 90f;
      StartCoroutine(ContinuosMove(movement, rotation));
    }
    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isMoving)
    {
      movement = new Vector3(-sizeEscaque, 0, 0);
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
        movement = new Vector3(sizeEscaque, 0, 0);
        rotation = 90f;
      }
      else
      {
        movement = new Vector3(-sizeEscaque, 0, 0);
        rotation = 270f;
      }
    }
    else
    {
      if (distance.z < 0)
      {
        movement = new Vector3(0, 0, sizeEscaque);
        rotation = 0.0f;
      }
      else
      {
        movement = new Vector3(0, 0, -sizeEscaque);
        rotation = 180f;
      }
    }
    MoveHero(movement, rotation);
  }

  private IEnumerator ContinuosMove(Vector3 movement, float rotation)
  {
    isMoving = true;

    Vector3 startPosition = transform.position;
    Vector3 endPosition =  terrainAdministrator.MoveHero(transform.position, movement);

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
 
}
