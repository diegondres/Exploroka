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
    private Inventory inventory;
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


    [NonSerialized]
    public int indexInDict;

    //EXTRACCION
    public bool extractionInInfluenceZone = false;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
    }


    public void SetInitialValues(string ResourceName, int quantity, bool canRunOut, bool extractionInInfluenceZone)
    {
        this.ResourceName = ResourceName;
        this.quantity = quantity;
        this.canRunOut = canRunOut;
        this.extractionInInfluenceZone = extractionInInfluenceZone;
    }

    public void PrintResourceValues()
    {
        Debug.Log("Name: " + ResourceName + ";\n" + "quantity: " + quantity + ";\n" + "Can Run Out?: " + canRunOut + " Tag: " + tags + " Tag2: " + tags2);
    }
    public void PreConsume()
    {
        if (tags2 != Tags.no_tallable) //Esto es por si nos encontramos con una pieda o similar que no hace nada
        {
            if (ThisResourceGoToInvetory())
            {
                Consume(true);
            }
            else
            {
                int numericIndex = SubTerrainAdmReference.GetNumericIndexFromGlobalPosition(transform.position);
                if (SubTerrainAdmReference.influencedEscaques.ContainsKey(numericIndex))
                {
                    City city = SubTerrainAdmReference.influencedEscaques[numericIndex].city;
                    Consume(false, city);
                }
                else
                {
                    List<City> citiesAround = SubTerrainAdmReference.DetectCity(transform.position, 20);
                    uIAdministrator.subUIAdminResources.ActivatePanelResourceDestination(citiesAround, this);
                }
            }
        }
    }

    public void Consume(bool toInventory, City city = null)
    {
        if (toInventory)
        {
            inventory.AddToInventory(this);
        }
        if (city != null)
        {
            city.shields += shields;
            city.population += population;
        }
        uIAdministrator.subUIAdminInventory.UpdateText();

        Destroy(gameObject);
    }


    public bool ThisResourceGoToInvetory()
    {
        return population == 0 && shields == 0;
    }

}
