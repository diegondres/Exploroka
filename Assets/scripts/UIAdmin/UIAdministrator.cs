using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAdministrator : MonoBehaviour
{
    [Header("Fundar")]
    public GameObject panelBuildingTown;
    public TextMeshProUGUI textBuildingTownPanel;
    public TextMeshProUGUI nameCity;
    public GameObject inputCity;


    [Header("Inventario")]
    public TextMeshProUGUI textInventory;
    private Inventory inventory;

    [Header("Fundar y Anexar")]
    public GameObject panelBuildOrConectTown;
    public TextMeshProUGUI textBuildOrConectTownPanel;
    public TMP_Dropdown dropdownCities;

    //REFERENCES
    private Construction construction;
    private List<City> cities;
    private City citySelected = null;
    private bool isCitySelected = false;

    [Header("Recursos")]
    public GameObject panelSelectCityResource;
    public TMP_Dropdown dropdownCitiesResource;
    private List<City> citiesNearResource;
    private City citySelectedResource = null;
    private bool isCityResourceSelected = false;
    private Resource resource;
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        construction = FindAnyObjectByType<Construction>();

        dropdownCities.onValueChanged.AddListener(HandleCityConnectionDropdown);
        dropdownCitiesResource.onValueChanged.AddListener(HandleCityResourceDropdown);

        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if (panelBuildingTown.activeSelf) SubTerrainAdmReference.terrainOfHero.IdleTime(0);
    }

    public void UpdateText()
    {
        string texto = "Gobernabilidad: " + inventory.governance + "\n";

        Dictionary<string, Tuple<int, Resource>> inv = inventory.inventory;
        foreach (var item in inv)
        {
            if (item.Value.Item1 > 0)
            {
                texto += "-" + item.Key + ": " + item.Value.Item1 + "\n";
            }
        }
        textInventory.text = texto;
    }

    public void BuildTown()
    {
        panelBuildingTown.SetActive(false);
        construction.BuildTown(nameCity.text, citySelected);
        citySelected = null;
        nameCity.text = "";
    }

    public void ActivatePanelBuildTown()
    {
        if (isCitySelected)
        {
            textBuildingTownPanel.text = "¿Esta seguro que quiere crear un PUEBLO por 1 de gobernabilidad [Icon_gov]?";
            inputCity.SetActive(false);
        }
        else
        {
            textBuildingTownPanel.text = "¿Esta seguro que quiere fundar una CIUDAD por 5 de gobernabilidad [Icon_gov]?";
            inputCity.SetActive(true);

        }

        isCitySelected = false;
        panelBuildingTown.SetActive(true);
    }

    public void ActivatePanelBuildOrConectTown(List<City> cities)
    {
        this.cities = cities;
        List<string> cityNames = cities.Select(city => city.nameCity).ToList();

        dropdownCities.ClearOptions();
        List<TMP_Dropdown.OptionData> options = dropdownCities.options;
        options.Insert(0, new TMP_Dropdown.OptionData(""));
        dropdownCities.AddOptions(cityNames);
        dropdownCities.value = 0;

        panelBuildOrConectTown.SetActive(true);
        textBuildOrConectTownPanel.text = "Elige entre fundar una nueva ciudad o elegir una ciudad a la que anexar nueva ciudad";
    }

    public void HandleCityConnectionDropdown(int index)
    {
        isCitySelected = true;
        index -= 1;
        citySelected = cities[index];
        panelBuildOrConectTown.SetActive(false);
        ActivatePanelBuildTown();
    }

    public void ActivatePanelResourceDestination(List<City> cities, Resource resource)
    {
        citiesNearResource = cities;
        this.resource = resource;
        List<string> cityNames = cities.Select(city => city.nameCity).ToList();

        dropdownCitiesResource.ClearOptions();
        List<TMP_Dropdown.OptionData> options = dropdownCitiesResource.options;
        options.Insert(0, new TMP_Dropdown.OptionData(""));
        dropdownCitiesResource.AddOptions(cityNames);
        dropdownCitiesResource.value = 0;

        panelSelectCityResource.SetActive(true);
    }

    public void HandleCityResourceDropdown(int index)
    {
        isCityResourceSelected = true;
        index -= 1;
        citySelectedResource = citiesNearResource[index];
    }
    
    public void ResourceDestinationSelected(){
        if(isCityResourceSelected){
            //llamar al consume del recurso
            resource.Consume(false, citySelectedResource);
            panelSelectCityResource.SetActive(false);
            isCitySelected = false;
        }
    }


    public bool IsAnyPanelOpen()
    {
        if (panelBuildingTown.activeSelf)
        {
            return true;
        }
        if (panelBuildOrConectTown.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
