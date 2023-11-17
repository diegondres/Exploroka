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
  private GameObject prefabCity;
  [SerializeField]
  private GameObject containerConstructions;
  private TerrainAdministrator terrainAdministrator;
  private ObjetsAdministrator objetsAdministrator;
  private int townCounter = 0;

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
        GameObject town = InstanciateObject(prefabTown);
        Town townScript = town.GetComponent<Town>();
        townScript.id = townCounter;

        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Town detectedCity = terreno.DetectCity(terreno.GetRelativePositionInVertices(town.transform.position), terreno, 60);

        if (detectedCity == null)
        {
          GameObject city = Instantiate(prefabCity, transform.position, Quaternion.identity);
          town.transform.SetParent(city.transform);
          townScript.city = city.GetComponent<City>();
        }else{
          townScript.city = detectedCity.city;
          town.transform.SetParent(detectedCity.city.transform);
        }

        townCounter++;
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
