using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum Tags
    {
        arbol,piedra,tallable,no_tallable,animal,planta,medicinal,hongo
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

    public void Consume()
    {
        //TODO: detectar ciudad mas cercana o a la que queremos que vayan los recursos como los escudos o poblacion.
        //Si es un objeto que entregue este tipo de recursos no se debe mandar al inventario.
        List<City> citiesAround = SubTerrainAdmReference.DetectCity(transform.position, 20);
        Debug.Log(citiesAround.Count);
        

        if (tags2 != Tags.no_tallable)
        {
            if (population == 0 && shields == 0)
            {
                inventory.AddToInventory(this);
            }

            inventory.shields += shields;
            inventory.population += population;
            uIAdministrator.UpdateText();   
            Destroy(gameObject);
        }

    }

}
