using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory 
{
    public static Dictionary<string, Tuple<int, Resource>> inventory = new();
    public static int governance = 5;

    public static void AddToInventory(Resource resource){
        if(inventory.ContainsKey(resource.ResourceName)){
            inventory[resource.ResourceName] = new Tuple<int, Resource>(inventory[resource.ResourceName].Item1 + resource.quantity, resource);
        }
        else{
            inventory.Add(resource.ResourceName, new Tuple<int, Resource>(resource.quantity, resource));
        }
    } 
}
 