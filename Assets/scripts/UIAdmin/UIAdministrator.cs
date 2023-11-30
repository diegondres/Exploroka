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

        subUIAdminCity = FindAnyObjectByType<SubUIAdminCity>();
        subUIAdminResources = FindAnyObjectByType<SubUIAdminResources>();
        subUIAdminInventory = FindAnyObjectByType<SubUIAdminInventory>();

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
