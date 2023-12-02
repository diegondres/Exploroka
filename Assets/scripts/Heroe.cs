using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class Heroe : MonoBehaviour
{
  [Header("Movilidad")]

  [SerializeField]
  private float moveDuration = 0.1f;
  public bool isMoving = false;
  private int sizeEscaque;
  private float rotation;

  //REFERENCIAS

  private Vector3 movement = Vector3.zero;
  private UIAdministrator uIAdministrator;
  public List<Tuple<Vector3, float>> route = new();
  public int indexRoute = 0;
  public bool IsRouteFinish = true;
  void Start()
  {
    uIAdministrator = FindAnyObjectByType<UIAdministrator>();
    SubTerrainAdmReference.InWhatTerrenoAmI(transform.position);
    sizeEscaque = SubTerrainAdmReference.sizeEscaque;

    //TODO: se tiene que hacer un arreglo para que el personaje inicie en una referencia correcta del terreno
    //MoveHero(movement, 0.0f);
  }

  void Update()
  {
  }
  public void GenerateRoute(Vector3 destino)
  {
    indexRoute = 0;
    route.Clear();
    IsRouteFinish = false;
    Vector3 distance = SubTerrainAdmReference.CalculateDistance(transform.position, destino);
    Vector3 distanceRelativePosition = distance / sizeEscaque;
    Vector3 heroCalculatedPosition = transform.position;
    
    while (Vector3.Magnitude(distanceRelativePosition) > 0.1f )
    {
      Tuple<Vector3, float> movementTuple = MouseMoving(distanceRelativePosition);
      heroCalculatedPosition += movementTuple.Item1;

      distanceRelativePosition = SubTerrainAdmReference.CalculateDistance(heroCalculatedPosition, destino) / sizeEscaque;
      route.Add(movementTuple);
    }
  }


  public void MoveThroughRoute()
  {
    StartCoroutine(ContinuosMove(route[indexRoute].Item1, route[indexRoute].Item2));
    indexRoute++;
    if(indexRoute == route.Count) IsRouteFinish = true;
  }

  private void MoveHero(Vector3 movement, float rotation)
  {
    //TODO: cuando se solucione el movimiento con el mouse, esta funcion deberia desaparecer y solo dejar la corutina.
    transform.position = SubTerrainAdmReference.MoveHero(transform.position, movement);
    transform.eulerAngles = new Vector3(0, rotation, 0);

    int numericIndex = SubTerrainAdmReference.GetNumericIndexFromGlobalPosition(transform.position);
    if(SubTerrainAdmReference.influencedEscaques.ContainsKey(numericIndex)){
      uIAdministrator.subUIAdminCity.ActivatePanelCityInformation(SubTerrainAdmReference.influencedEscaques[numericIndex].city);
    }
    else {
      uIAdministrator.subUIAdminCity.panelCityInformation.SetActive(false);
    }
  }

  public void ArrowMoving()
  {
    rotation = 0.0f;
    movement = Vector3.zero;
    if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isMoving)
    {
      movement += new Vector3(0, 0, sizeEscaque);
      rotation = 0.0f;
    }
    if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isMoving)
    {
      movement += new Vector3(0, 0, -sizeEscaque);
      rotation = 180f;
    }
    if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isMoving)
    {
      movement += new Vector3(sizeEscaque, 0, 0);
      rotation = 90f;
    }
    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isMoving)
    {
      movement += new Vector3(-sizeEscaque, 0, 0);
      rotation = 270f;
    }

    if (Vector3.Magnitude(movement) > 0)
    {
      StartCoroutine(ContinuosMove(movement, rotation));
      if (isMoving)
      {
        IsRouteFinish = true;
      }
    }
  }

  public Tuple<Vector3, float> MouseMoving(Vector3 distance)
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
    return new Tuple<Vector3, float>(movement, rotation);


  }

  private IEnumerator ContinuosMove(Vector3 movement, float rotation)
  {
    isMoving = true;

    Vector3 startPosition = transform.position;
    Vector3 endPosition = SubTerrainAdmReference.MoveHero(transform.position, movement);

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
