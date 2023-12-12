using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory 
{
    public static Dictionary<string, Tuple<int, ResourcesClass>> inventory = new();
    public static int governance = 5;

    public static void AddToInventory(ResourcesClass resource){
        if(inventory.ContainsKey(resource.name)){
            inventory[resource.name] = new Tuple<int, ResourcesClass>(inventory[resource.name].Item1 + 1, resource);
        }
        else{
            inventory.Add(resource.name, new Tuple<int, ResourcesClass>(1, resource));
        }
    } 
}
 