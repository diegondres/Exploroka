using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Construction : MonoBehaviour
{

  [SerializeField]
  private GameObject prefabTest;
  [SerializeField]
  private GameObject prefabTown;
  [SerializeField]
  private GameObject prefabCity;

  private TerrainAdministrator terrainAdministrator;
  private ObjetsAdministrator objetsAdministrator;
  private UIAdministrator uIAdministrator;
  private Inventory inventory;
  private int townCounter = 0;
  private int cityCounter = 0;
  private int cityPrice = 5;

  // Start is called before the first frame update
  void Start()
  {
    terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
    uIAdministrator = FindAnyObjectByType<UIAdministrator>();
    inventory = FindAnyObjectByType<Inventory>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) && CheckBuildingLocation())
    {
      GameObject building = InstanciateObject(prefabTest);
      SubObjectsAdmReferences.AddBuilding(building);

      Building buildingScript = building.GetComponent<Building>();
      buildingScript.SetInitialValues("Casita", 100, 250, 1, 5, true);
    }

    if (Input.GetKeyDown(KeyCode.F) && CheckBuildingLocation())
    {
      //Detectar si esta proximo a ciudades, de ser asi, enviar lista de ciudades cercanas.
      
      uIAdministrator.ActivatePanelBuildTown();
      

    }

  }

  private GameObject InstanciateObject(GameObject prefab)
  {
    Vector3 buildingLocation = SubObjectsAdmReferences.GetBuildingLocation();

    SubObjectsAdmReferences.isBuildingLocationSelected = false;

    return Instantiate(prefab, buildingLocation, Quaternion.identity, SubObjectsAdmReferences.containerConstructions.transform);
  }

  private bool CheckBuildingLocation()
  {
    if (!SubObjectsAdmReferences.isBuildingLocationSelected) return false;

    Vector3 buildingLocation = SubObjectsAdmReferences.GetBuildingLocation();
    Terreno terreno = SubTerrainAdmReference.terrainOfHero;

    Vector3 relativePosition = terreno.GetRelativePositionInVertices(buildingLocation);
    Tuple<int, Terreno> index = terreno.GetIndexGlobal(relativePosition);
    relativePosition = index.Item2.GetGlobalPositionFromGlobalIndex(index);
    relativePosition = index.Item2.GetRelativePositionInVertices(relativePosition);

    if (!index.Item2.IsWalkable(relativePosition)) return false;
    if (SubObjectsAdmReferences.IsSomethingBuiltInHere(index) != null) return false;

    return true;
  }

  public void BuildTown(string nameCity)
  {
    GameObject town = InstanciateObject(prefabTown);
    SubObjectsAdmReferences.AddBuilding(town);

    Town townScript = town.GetComponent<Town>();
    townScript.id = townCounter;

    Town detectedCity = SubTerrainAdmReference.DetectCity(town.transform.position, 15);

    if (detectedCity == null)
    {
      GameObject city = Instantiate(prefabCity, transform.position, Quaternion.identity);
      town.transform.SetParent(city.transform);
      City cityScript = city.GetComponent<City>();
      cityScript.id = cityCounter;
      cityScript.nameCity = nameCity;
      cityScript.gameObject.name = nameCity;

      townScript.city = cityScript;

      inventory.governance -= cityPrice;
      cityCounter++;
    }
    else
    {
      townScript.city = detectedCity.city;
      town.transform.SetParent(detectedCity.city.transform);
    }
    StartCoroutine(GenerateNewInfluenceZone(townScript.city));
    uIAdministrator.UpdateText();
    townCounter++;
  }

  private IEnumerator GenerateNewInfluenceZone(City city)
  {
    yield return new WaitForSeconds(0.1f);
    city.GenerateNewInfluenceZone();
  }
}
