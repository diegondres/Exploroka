using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private string ResourceName;
    private List<string> tags;

    //AGOTAMIENTO
    private int quantity;
    private bool canRunOut = false;
    public int indexDict;

    //EXTRACCION
    private bool extractionInInfluenceZone = false;

    public void SetInitialValues(string ResourceName, int quantity, bool canRunOut, bool extractionInInfluenceZone)
    {
        this.ResourceName = ResourceName;
        this.quantity = quantity;
        this.canRunOut = canRunOut;
        this.extractionInInfluenceZone = extractionInInfluenceZone;
    }

    public void PrintResourceValues()
    {
        Debug.Log("Name: " + ResourceName + ";\n" + "quantity: " + quantity + ";\n" + "Can Run Out?: " + canRunOut);
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
