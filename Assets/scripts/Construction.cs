using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;

  private TerrainAdministrator tempPlanes;
  // Start is called before the first frame update
  void Start()
  {
    tempPlanes = FindAnyObjectByType<TerrainAdministrator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && tempPlanes.isBuildingLocationSelected)
    {
      Vector3 buildingLocation = tempPlanes.GetBuildingLocation();
      Instantiate(prefabTest, buildingLocation, Quaternion.identity);
      tempPlanes.isBuildingLocationSelected = false;
    }

  }
}
