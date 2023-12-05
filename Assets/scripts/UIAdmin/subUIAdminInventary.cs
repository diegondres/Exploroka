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
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateText()
    {
        UpdateTextGovernance();
        string texto = "";

        foreach (var item in inventory.inventory)
        {
            if (item.Value.Item1 > 0)
            {
                texto += "-" + item.Key + ": " + item.Value.Item1 + "\n";
            }
        }
        textInventory.text = texto;
    }

    public void UpdateTextGovernance(){
        textGovernance.text = "X " + inventory.governance + "   Gobernabilidad";
    }
}
