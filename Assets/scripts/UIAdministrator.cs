using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAdministrator : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;     
    private Inventory inventory;
    private int shields = 0;
    private int population = 0;
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        UpdateText(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(int foodForPeople, int woodToBuild){
        if(foodForPeople > 0) population += foodForPeople;
        if(woodToBuild > 0) shields += woodToBuild;
        string texto = "Poblacion: " + population + ", Escudos: " + shields + "\n";
        Dictionary<string, Tuple<int, Resource>> inv = inventory.inventory;
        foreach(var item in inv){
            if(item.Value.Item1 > 0){
                texto += "-" + item.Key + ": "+item.Value.Item1 + "\n";
            }
        }
        textMeshProUGUI.text = texto;
    }
}
