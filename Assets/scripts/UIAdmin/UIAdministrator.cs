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

    [Header("Inventario")]
    public TextMeshProUGUI textInventory;
    private Inventory inventory;

    [NonSerialized]
    public SubUIAdminInventory subUIAdminInventory;
    [NonSerialized]
    public SubUIAdminCity subUIAdminCity;
    [NonSerialized]
    public SubUIAdminResources subUIAdminResources;
    [NonSerialized]
    public List<GameObject> panels = new();
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();

        subUIAdminCity = GetComponent<SubUIAdminCity>();
        subUIAdminResources = GetComponent<SubUIAdminResources>();
        subUIAdminInventory = GetComponent<SubUIAdminInventory>();

    }
    void Update(){
        if (IsAnyPanelOpen()) SubTerrainAdmReference.terrainOfHero.IdleTime(0);
    }

    public bool IsAnyPanelOpen()
    {
        foreach (GameObject panel in panels)
        {
            if (panel.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
