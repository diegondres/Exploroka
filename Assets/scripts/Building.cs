using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private string buildingName;
    private List<string> tags;
    private float cost;
    private int populationRequired;
    private int servicesRequired;
    private int governance;
    private bool canBeBuildInTown = true;    
    //resto de propiedades de una construccion se tienen que definir con el tiempo.
    
    public void SetInitialValues(string buildingName, float cost, int populationRequired, int servicesRequired, int governance, bool canBeBuildInTown){
        this.buildingName = buildingName;
        this.cost = cost;
        this.populationRequired = populationRequired;
        this.servicesRequired = servicesRequired;
        this.governance = governance;
        this.canBeBuildInTown = canBeBuildInTown;
    }

    public void PrintBuildingValues(){
        Debug.Log("Name: " + buildingName+";\n" +"Cost: " + cost+";\n" +"Population required: " + populationRequired +";\n" + "Services Required: " + servicesRequired +";\n" + "Governance: " + governance +";\n" + "Can be built in a town? " + canBeBuildInTown);
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    
}
