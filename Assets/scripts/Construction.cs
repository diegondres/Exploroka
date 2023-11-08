using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;

  private TerrainAdministrator terrainAdministrator;
  private ObjetsAdministrator objetsAdministrator;
  // Start is called before the first frame update
  void Start()
  {
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && objetsAdministrator.isBuildingLocationSelected)
    {
      Vector3 buildingLocation = objetsAdministrator.GetBuildingLocation();
      GameObject construction = Instantiate(prefabTest, buildingLocation, Quaternion.identity);

      objetsAdministrator.isBuildingLocationSelected = false;
      objetsAdministrator.AddBuilding(construction);
    }

  }
}
