using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private int townCounter = 0;
    private int cityCounter = 0;
    private int cityPrice = 5;

    // Start is called before the first frame update
    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
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
        if (Input.GetKeyDown(KeyCode.Z) && CheckBuildingLocation())
        {
            int idConstru = Random.Range(0, terrainAdministrator.reglas.buildings.Count);

            GameObject building = InstanciateObject(terrainAdministrator.reglas.buildings[idConstru].modelsPrefab[0]);
            SubObjectsAdmReferences.AddBuilding(building);
        }


        if (Input.GetKeyDown(KeyCode.F) && CheckBuildingLocation() && !IsBuildingLocationInCity())
        {
            List<City> detectedCities = SubTerrainAdmReference.DetectCity(SubObjectsAdmReferences.GetBuildingLocation(), 15);
            uIAdministrator.subUIAdminCity.ActivatePanelBuildOrConectTown(detectedCities);
        }

    }

    private GameObject InstanciateObject(GameObject prefab)
    {
        Vector3 buildingLocation = SubObjectsAdmReferences.GetBuildingLocation();

        SubObjectsAdmReferences.isBuildingLocationSelected = false;

        return Instantiate(prefab, buildingLocation, Quaternion.identity, SubObjectsAdmReferences.containerConstructions.transform);
    }

    private bool IsBuildingLocationInCity()
    {
        int numericIndex = SubTerrainAdmReference.GetNumericIndexFromGlobalPosition(SubObjectsAdmReferences.GetBuildingLocation(), null);

        return SubTerrainAdmReference.influencedEscaques.ContainsKey(numericIndex);
    }

    private bool CheckBuildingLocation()
    {
        if (!SubObjectsAdmReferences.isBuildingLocationSelected) return false;

        Vector3 buildingLocation = SubObjectsAdmReferences.GetBuildingLocation();
        Terreno terreno = SubTerrainAdmReference.terrainOfHero;

        Vector3 relativePosition = terreno.GetRelativePositionInVertices(buildingLocation);
        Tuple<int, Terreno> index = terreno.GetIndexGlobal(relativePosition);
        relativePosition = index.Item2.GetGlobalPositionFromGlobalIndex(index,0f);
        relativePosition = index.Item2.GetRelativePositionInVertices(relativePosition);

        if (!index.Item2.IsWalkable(relativePosition)) return false;
        if (SubObjectsAdmReferences.IsSomethingBuiltInHere(index) != null) return false;

        return true;
    }

    public void BuildTown(string nameCity, City attachedCity)
    {
        GameObject town = InstanciateObject(prefabTown);
        SubObjectsAdmReferences.AddBuilding(town);

        Town townScript = town.GetComponent<Town>();
        townScript.id = townCounter;

        if (attachedCity != null)
        {
            townScript.city = attachedCity;
            town.transform.SetParent(attachedCity.transform);

            Inventory.governance -= 1;

        }
        else
        {
            GameObject city = Instantiate(prefabCity, transform.position, Quaternion.identity);
            town.transform.SetParent(city.transform);

            City cityScript = city.GetComponent<City>();
            cityScript.id = cityCounter;
            cityScript.nameCity = nameCity;
            cityScript.gameObject.name = nameCity;

            townScript.city = cityScript;

            SubObjectsAdmReferences.AddCity(cityScript);
            Inventory.governance -= cityPrice;
            cityCounter++;
        }

        StartCoroutine(GenerateNewInfluenceZone(townScript.city));
        uIAdministrator.subUIAdminInventory.UpdateText();
        townCounter++;
    }

    private IEnumerator GenerateNewInfluenceZone(City city)
    {
        yield return new WaitForSeconds(0.1f);
        city.GenerateNewInfluenceZone();
    }
}
