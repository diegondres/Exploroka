using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using UnityEditor.AppleTV;
using UnityEngine;

public class City : MonoBehaviour
{
    public int id = 0;
    public string nameCity;
    public int population = 0;
    public int shields = 0;

    public List<Tuple<int, Terreno>> influencedEscaquesOfCity;
    private ObjetsAdministrator objetsAdministrator;
    private TerrainAdministrator terrainAdministrator;
    void Start(){
        objetsAdministrator = FindObjectOfType<ObjetsAdministrator>();
        terrainAdministrator =  FindAnyObjectByType<TerrainAdministrator>();
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
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

    public void GenerateNewInfluenceZone(){
                Debug.Log("asdsasdasdsadasdasdasd");

        objetsAdministrator.DeleteAllFrontiersCity(id);
        List<Tuple<int, Terreno>> allInfluencedEscaques = new();

        foreach (Town town in GetTownsOfThisCity())
        {
            allInfluencedEscaques.AddRange(town.influencedEscaques);
        }
        
        SubTerrainAdmReference.CheckNewInfluencedEscaques(allInfluencedEscaques, id, terrainAdministrator);
    }

}
