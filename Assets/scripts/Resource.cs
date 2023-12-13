using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum Tags
    {
        arbol, piedra, tallable, no_tallable, animal, planta, medicinal, hongo
    }
    //REFERENCIAS

    private UIAdministrator uIAdministrator;


    //COSAS DE RECURSOS
    public string ResourceName;
    [SerializeField]
    private Tags tags;
    [SerializeField]
    private Tags tags2;
    public int population = 0;
    public int shields = 0;
    public int quantity;
    public bool canRunOut = false;
    public Tuple<int, Terreno> myGlobalIndex;


    [NonSerialized]
    public int indexInDict;

    //EXTRACCION
    public bool extractionInInfluenceZone = false;

    void Start()
    {
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
    }


    public void PreConsume()
    {
        if (tags2 != Tags.no_tallable) //Esto es por si nos encontramos con una pieda o similar que no hace nada
        {
            if (ThisResourceGoToInvetory())
            {
        //        Consume(true);
            }
            else
            {
                int numericIndex = SubTerrainAdmReference.GetNumericIndexFromGlobalPosition(transform.position);
                if (SubTerrainAdmReference.influencedEscaques.ContainsKey(numericIndex))
                {
                    Town town = SubTerrainAdmReference.influencedEscaques[numericIndex];
                    town.RemoveResourceAvailable(numericIndex);
        //            Consume(false, town.city);
                }
                else
                {
                    List<City> citiesAround = SubTerrainAdmReference.DetectCity(transform.position, 20);
                  //  uIAdministrator.subUIAdminResources.ActivatePanelResourceDestination(citiesAround, this);
                }
            }
        }
    }

    public void Consume(bool toInventory, City city = null)
    {
        if (toInventory)
        {
         //   Inventory.AddToInventory(this);
            uIAdministrator.subUIAdminInventory.UpdateText();
        }
        if (city != null)
        {   
            city.shields += shields;
            city.population += population;
            uIAdministrator.subUIAdminCity.ActivatePanelCityInformation(city);  
            //Decirle al town que borre ese recurso disponible
        }

        Destroy(gameObject);
    }


    public bool ThisResourceGoToInvetory()
    {
        return population == 0 && shields == 0;
    }

}
