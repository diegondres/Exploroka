using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;

  private Terreno tempPlanes;
  // Start is called before the first frame update
  void Start()
  {
    tempPlanes = FindAnyObjectByType<Terreno>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      //obtener posicion de adelante e instanciar
      Vector3 buildingLocation = tempPlanes.GetPositionToBuildIn();
      Instantiate(prefabTest, buildingLocation, Quaternion.identity);
    }

  }
}
