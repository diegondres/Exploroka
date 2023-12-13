using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubResourcesObjAdmin : MonoBehaviour
{
    [SerializeField]
    private UIAdministrator uIAdministrator;
    public static readonly Dictionary<int, GameObject> resources = new(); //Diccionario indexNumerico - GameObject Recurso.
    public static readonly Dictionary<int, ResourcesClass> resourcesInfo = new();  //Diccionario indexNumerico - Data del recurso almacenado en la instancia de la clase recurso.

    public static ResourcesClass IsAResourceHere(int numericIndex)
    {
        //Es en el 1 porque esa es la posicion de los recursos en la lista allObjects
        if (resourcesInfo.ContainsKey(numericIndex))
        {
            return resourcesInfo[numericIndex];
        }

        return null;
    }

    public static void AddResource(GameObject resource, ResourcesClass resourcesClass, int numericIndex)
    {
 
        resources.Add(numericIndex, resource);
        resourcesInfo.Add(numericIndex, resourcesClass);
    }
    

    public static ResourcesClass GetResourceInfo(Tuple<int, Terreno> globalIndex)
    {
        int numericIndex = SubTerrainAdmReference.GetNumericIndex(globalIndex);

        if (resourcesInfo.ContainsKey(numericIndex))
        {
            return resourcesInfo[numericIndex];
        }

        return null;
    }

    public void DestroyResource(int numericIndex)
    {

        if (resourcesInfo.ContainsKey(numericIndex))
        {
            GameObject resourceObject = resources[numericIndex];
            resourcesInfo.Remove(numericIndex);
            resources.Remove(numericIndex);
            Destroy(resourceObject);
        }
    }
    public void PreConsume(int numericIndex)
    {
        ResourcesClass resource = resourcesInfo[numericIndex];

        if (resource.carvable) //Esto es por si nos encontramos con una piedra o similar que no hace nada
        {
            if (resource.population == 0 && resource.shields == 0)
            {
                Consume(resource, true);
            }
            else
            {
                if (SubTerrainAdmReference.influencedEscaques.ContainsKey(numericIndex))
                {
                    Town town = SubTerrainAdmReference.influencedEscaques[numericIndex];
                    town.RemoveResourceAvailable(numericIndex);
                    Consume(resource, false, town.city);
                }
                else
                {
                    List<City> citiesAround = SubTerrainAdmReference.DetectCity(resource.globalPosition, 20);
                    uIAdministrator.subUIAdminResources.ActivatePanelResourceDestination(citiesAround, resource);
                }
            }
        }
    }

    public void Consume(ResourcesClass resource, bool toInventory, City city = null)
    {
        if (toInventory)
        {
            Inventory.AddToInventory(resource);
            uIAdministrator.subUIAdminInventory.UpdateText();
        }
        if (city != null)
        {
            city.shields += resource.shields;
            city.population += resource.population;
            uIAdministrator.subUIAdminCity.ActivatePanelCityInformation(city);
            //Decirle al town que borre ese recurso disponible
        }
        DestroyResource(resource.numericIndex);
    }

}
