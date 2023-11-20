using System;
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
    
      if (Input.GetKeyDown(KeyCode.Space) && CheckBuildingLocation())
      {
        GameObject building = InstanciateObject(prefabTest);

        Building buildingScript = building.GetComponent<Building>();
        buildingScript.SetInitialValues("Casita", 100, 250, 1, 5, true);
      }

      if (Input.GetKeyDown(KeyCode.F) && CheckBuildingLocation())
      {
        GameObject town = InstanciateObject(prefabTown);
        Town townScript = town.GetComponent<Town>();
        townScript.id = townCounter;

        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Town detectedCity = terreno.DetectCity(terreno.GetRelativePositionInVertices(town.transform.position), terreno, 20);

        if (detectedCity == null)
        {
          GameObject city = Instantiate(prefabCity, transform.position, Quaternion.identity);
          town.transform.SetParent(city.transform);
          townScript.city = city.GetComponent<City>();
          townScript.city.id = cityCounter;
          
          cityCounter++;
        }else{
          townScript.city = detectedCity.city;
          town.transform.SetParent(detectedCity.city.transform);
        }

        townCounter++;
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

  private bool CheckBuildingLocation(){
    if(!objetsAdministrator.isBuildingLocationSelected) return false;

    Vector3 buildingLocation = objetsAdministrator.GetBuildingLocation();
    Terreno terreno = terrainAdministrator.terrenoOfHero;

    Vector3 relativePosition = terreno.GetRelativePositionInVertices(buildingLocation);
    Tuple<int, Terreno> index = terreno.GetIndexGlobal(relativePosition);
    relativePosition = index.Item2.GetGlobalPositionFromGlobalIndex(index);
    relativePosition = index.Item2.GetRelativePositionInVertices(relativePosition);

    if(!index.Item2.IsWalkable(relativePosition)) return false;
    if(objetsAdministrator.IsSomethingBuiltInHere(index) != null ) return false;

    return true;
  }
}
