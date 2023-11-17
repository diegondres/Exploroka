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
    private ObjetsAdministrator objetsAdministrator;
    Tuple<int, Terreno> globalIndexCity;

    // Start is called before the first frame update
    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
        GenerateInfluenceZone();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateInfluenceZone()
    {
        Vector3 relativePosition = terrainAdministrator.terrenoOfHero.GetRelativePositionInVertices(transform.position);
        globalIndexCity = terrainAdministrator.terrenoOfHero.GetIndexGlobal(relativePosition);

        for (int i = -12; i <= 12; i++)
        {
            for (int j = -12; j <= 12; j++)
            {
                if (i * i + j * j < 144)
                {
                    Vector3 relativePositionIJ = new(relativePosition.x + i, relativePosition.y, relativePosition.z + j);
                    Tuple<int, Terreno> index = terrainAdministrator.terrenoOfHero.GetIndexGlobal(relativePositionIJ);
                    int indexNumeric = objetsAdministrator.GetNumericIndex(index);
                    if (!terrainAdministrator.influencedEscaques.ContainsKey(indexNumeric)) terrainAdministrator.influencedEscaques.Add(indexNumeric, this);
                }
            }
        }
        terrainAdministrator.PaintInfluence();
    }
}
