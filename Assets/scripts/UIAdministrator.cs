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


    [Header("Inventario")]
    public TextMeshProUGUI textInventory;
    private Inventory inventory;

    [Header("Fundar y Anexar")]
    public GameObject panelBuildOrConectTown;
    public TextMeshProUGUI textBuildOrConectTownPanel;
    public TMP_Dropdown dropdownCities;

    //REFERENCES
    private Construction construction;
    private TerrainAdministrator terrainAdministrator;
    private List<City> cities;
    private City citySelected = null;

    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        construction = FindAnyObjectByType<Construction>();
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();

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
        texto += "Poblacion: " + inventory.population + ", Escudos: " + inventory.shields + "\n";

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
        construction.BuildTown(nameCity.text);
        panelBuildingTown.SetActive(false);
        citySelected = null;
    }

    public void ActivatePanelBuildTown()
    {
        panelBuildingTown.SetActive(true);
        Debug.Log(citySelected.nameCity);
        textBuildingTownPanel.text = "¿Esta seguro que quiere gastar 5 de gobernabilidad [Icon_gov]?";
        textBuildOrConectTownPanel.autoSizeTextContainer = true;

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
        textBuildOrConectTownPanel.text = "Elige entre fundar una nueva ciudad o elegir una ciudad a la que añexar nueva ciudad";
        textBuildOrConectTownPanel.autoSizeTextContainer = true;

    }

    public void HandleCityConnectionDropdown(int index)
    {
        Debug.Log(index);
        citySelected = cities[index];
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
