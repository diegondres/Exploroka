using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private string ResourceName;
    private List<string> tags;

    //AGOTAMIENTO
    private int quantiity;
    private bool canRunOut = false;

    //EXTRACCION
    private bool extractionInInfluenceZone = false;

    public void SetInitialValues(string ResourceName, int quantiity, bool canRunOut, bool extractionInInfluenceZone)
    {
        this.ResourceName = ResourceName;
        this.quantiity = quantiity;
        this.canRunOut = canRunOut;
        this.extractionInInfluenceZone = extractionInInfluenceZone;
    }

    public void PrintResourceValues()
    {
        Debug.Log("Name: " + ResourceName + ";\n" + "quantity: " + quantiity + ";\n" + "Can Run Out?: " + canRunOut);
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
