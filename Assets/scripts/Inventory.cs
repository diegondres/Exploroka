using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, Tuple<int, Resource>> inventory = new();
    public int governance = 5;
    public int shields = 0;
    public int population = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)){
            foreach(var resource in inventory){
                Debug.Log(resource.Key + resource.Value.Item1);
            }
        }
    }

    public void AddToInventory(Resource resource){
        if(inventory.ContainsKey(resource.ResourceName)){
            inventory[resource.ResourceName] = new Tuple<int, Resource>(inventory[resource.ResourceName].Item1 + resource.quantity, resource);
        }
        else{
            inventory.Add(resource.ResourceName, new Tuple<int, Resource>(resource.quantity, resource));
        }
    }
}
