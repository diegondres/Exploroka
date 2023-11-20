using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
     public enum Tags
    {
        arbol,
        piedra,
        tallable,
        no_tallable,
        animal,
        planta,
        medicinal,
        hongo
    }
    //REFERENCIAS
    private Inventory inventory;

    //COSAS DE RECURSOS
    public string ResourceName;
    [SerializeField]
    private Tags tags;
    [SerializeField]
    private Tags tags2;
    public int population = 0;  
    public int shields = 0;

    //AGOTAMIENTO
    public int quantity;
    public bool canRunOut = false;
    [NonSerialized]
    public int indexDict;

    //EXTRACCION
    public bool extractionInInfluenceZone = false;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {

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
        inventory.AddToInventory(this);
        Destroy(gameObject);
    }


}
