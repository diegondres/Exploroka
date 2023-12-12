using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubUIAdminInventory : MonoBehaviour
{
    [Header("Inventario")]
    public TextMeshProUGUI textInventory;
    public TextMeshProUGUI textGovernance;

    public void UpdateText()
    {
        UpdateTextGovernance();
        string texto = "";

        foreach (var item in Inventory.inventory)
        {
            if (item.Value.Item1 > 0)
            {
                texto += "-" + item.Key + ": " + item.Value.Item1 + "\n";
            }
        }
        textInventory.text = texto;
    }

    public void UpdateTextGovernance(){
        textGovernance.text = "X " + Inventory.governance + "   Gobernabilidad";
    }
    
}
