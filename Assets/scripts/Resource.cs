using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    //REFERENCIAS
    private Inventory inventory;

    //COSAS DE RECURSOS
    public string ResourceName;
    public readonly List<string> tags = new();

    public readonly string[] predefinedTags = new string[] { "árbol", "piedra", "tallable", "animal", "planta", "medicinal", "hongo" };

    //AGOTAMIENTO
    public int quantity;
    public bool canRunOut = false;
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

        int random = Random.Range(0, predefinedTags.Length);
        tags.Add(predefinedTags[random]);
    }

    public void PrintResourceValues()
    {
        Debug.Log("Name: " + ResourceName + ";\n" + "quantity: " + quantity + ";\n" + "Can Run Out?: " + canRunOut);
        foreach (string tag in tags)
        {
            Debug.Log(" recursos: " + tag);
        }
    }

    public void Consume()
    {
        inventory.AddToInventory(this);
        Destroy(gameObject);
    }


}
