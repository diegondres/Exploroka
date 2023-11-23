using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class UIAdministrator : MonoBehaviour
{
    [Header("Construction")]
    public GameObject panelBuildingTown;    
    public TextMeshProUGUI textMeshProUGUI;     
    private Construction construction;
    private Inventory inventory;
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
        textMeshProUGUI.text = texto;
    }

     // Método llamado cuando se hace clic en el botón "Sí"
    public void BuildTown()
    {
        construction.BuildTown();
        panelBuildingTown.SetActive(false);
    }

    // Método llamado cuando se hace clic en el botón "No"
    public void DontBuildTown()
    {
        // Ocultar el cuadro de diálogo sin realizar la acción
        panelBuildingTown.SetActive(false);
    }
}
