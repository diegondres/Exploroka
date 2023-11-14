using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;
  [SerializeField]
  private GameObject prefabTown;
  [SerializeField]
  private GameObject containerConstructions;
  private TerrainAdministrator terrainAdministrator;
  private ObjetsAdministrator objetsAdministrator;
  private int cityCounter = 0;

  // Start is called before the first frame update
  void Start()
  {
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
  }

  // Update is called once per frame
  void Update()
  {
    if (objetsAdministrator.isBuildingLocationSelected)
    {
      if (Input.GetKeyDown(KeyCode.Space))
      {
        GameObject building = InstanciateObject(prefabTest);

        Building buildingScript = building.GetComponent<Building>();
        buildingScript.SetInitialValues("Casita", 100, 250, 1, 5, true);
      }

      if (Input.GetKeyDown(KeyCode.F))
      {
        GameObject city = InstanciateObject(prefabTown);
        City cityScript = city.GetComponent<City>();
        cityScript.id = cityCounter;

        cityCounter++;
      }
    }
  }

  private GameObject InstanciateObject(GameObject prefab)
  {
    Vector3 buildingLocation = objetsAdministrator.GetBuildingLocation();
    GameObject building = Instantiate(prefab, buildingLocation, Quaternion.identity);
    building.transform.SetParent(containerConstructions.transform);

    objetsAdministrator.isBuildingLocationSelected = false;
    objetsAdministrator.AddBuilding(building);

    return building;
  }
}
