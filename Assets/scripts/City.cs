using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.AppleTV;
using UnityEngine;

public class City : MonoBehaviour
{
    public int id;
    public int cost = 5;
    private TerrainAdministrator terrainAdministrator;
    Tuple<int, Terreno> globalIndexCity;

    // Start is called before the first frame update
    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        GenerateInfluenceZone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateInfluenceZone(){
        Vector3 relativePosition = terrainAdministrator.terrenoOfHero.GetRelativePositionInVertices(transform.position);
        globalIndexCity = terrainAdministrator.terrenoOfHero.GetIndexGlobal(relativePosition);    
        
        for (int i = -10; i < 10; i++)
        {
            for (int j = -10; j < 10; j++)
            {
                Vector3 relativePositionIJ = new Vector3(relativePosition.x + i, relativePosition.y, relativePosition.z + j);
                Tuple<int, Terreno> index = terrainAdministrator.terrenoOfHero.GetIndexGlobal(relativePositionIJ);    
                terrainAdministrator.influencedEscaques.Add(new Tuple<Tuple<int, Terreno>, City> (index, this));
            }
        }
        terrainAdministrator.PaintInfluence();
    }
}
