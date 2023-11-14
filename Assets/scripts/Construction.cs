using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;
  [SerializeField]
  private GameObject containerConstructions;
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
      GameObject building = Instantiate(prefabTest, buildingLocation, Quaternion.identity);
      building.transform.SetParent(containerConstructions.transform);

      Building buildingScript = building.GetComponent<Building>();
      buildingScript.SetInitialValues("Casita", 100, 250, 1, 5, true); 

      objetsAdministrator.isBuildingLocationSelected = false;
      objetsAdministrator.AddBuilding(building);
    }

  }
}
