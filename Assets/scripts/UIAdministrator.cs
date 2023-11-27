using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

    [Header("Referencias ciudades")]
    public TMP_Dropdown dropdownCities;

    //REFERENCES
    private Construction construction;
    private TerrainAdministrator terrainAdministrator;
    
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
        if(panelBuildingTown.activeSelf) SubTerrainAdmReference.terrainOfHero.IdleTime(0);
    }

    public void UpdateText(){
        string texto = "Gobernabilidad: " + inventory.governance + "\n";
        texto += "Poblacion: " + inventory.population + ", Escudos: " + inventory.shields + "\n";
        
        Dictionary<string, Tuple<int, Resource>> inv = inventory.inventory;
        foreach(var item in inv){
            if(item.Value.Item1 > 0){
                texto += "-" + item.Key + ": "+item.Value.Item1 + "\n";
            }
        }
        textInventory.text = texto;
    }

     // Método llamado cuando se hace clic en el botón "Sí"
    public void BuildTown()
    {
        construction.BuildTown(nameCity.text);
        
        panelBuildingTown.SetActive(false);
    }

    // Método llamado cuando se hace clic en el botón "No"
    public void DontBuildTown()
    {   
        panelBuildingTown.SetActive(false);
    }

    public void ActivatePanelBuildTown(string texto){
        
        panelBuildingTown.SetActive(true);
        textBuildingTownPanel.text = texto;
        
    }

    public bool IsAnyPanelOpen(){
        if(panelBuildingTown.activeSelf){
            return true;
        }
        else {
            return false;
        }
    }
}
