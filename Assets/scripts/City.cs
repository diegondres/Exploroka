using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public int id = 0;
    public string nameCity;
    public int population = 0;
    public int shields = 0;
    public float foodConsumption = 0;
    public float happiness = 0;
    public int services;

    public List<Tuple<int, Terreno>> influencedEscaquesOfCity;
    private ObjetsAdministrator objetsAdministrator;
    private TerrainAdministrator terrainAdministrator;
    void Start()
    {
        objetsAdministrator = FindObjectOfType<ObjetsAdministrator>();
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    }


    public List<Town> GetTownsOfThisCity()
    {
        List<Town> towns = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            towns.Add(transform.GetChild(i).gameObject.GetComponent<Town>());
        }
        return towns;
    }

    public void GenerateNewInfluenceZone()
    {
        objetsAdministrator.DeleteAllFrontiersCity(id);
        List<Tuple<int, Terreno>> allInfluencedEscaques = new();    

        foreach (Town town in GetTownsOfThisCity())
        {
            allInfluencedEscaques.AddRange(town.influencedEscaques);
        }

        SubTerrainAdmReference.CheckNewInfluencedEscaques(allInfluencedEscaques, id, terrainAdministrator);
    }
    
}
