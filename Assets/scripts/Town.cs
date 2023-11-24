using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{

    [NonSerialized]
    public int id;
    [NonSerialized]
    public int cost = 5;
    private TerrainAdministrator terrainAdministrator;
    private ObjetsAdministrator objetsAdministrator;
    private readonly int sizeInfluence = 15;
    public List<Tuple<int, Terreno>> influencedEscaques = new();
    public City city;

    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
        GenerateInfluenceZone();
    }

    void GenerateInfluenceZone()
    {
        Vector3 relativePosition = SubTerrainAdmReference.terrainOfHero.GetRelativePositionInVertices(transform.position);

        for (int i = -sizeInfluence; i <= sizeInfluence; i++)
        {
            for (int j = -sizeInfluence; j <= sizeInfluence; j++)
            {
                if (i * i + j * j < sizeInfluence * sizeInfluence)
                {
                    Vector3 relativePositionIJ = new(relativePosition.x + i, relativePosition.y, relativePosition.z + j);
                    Tuple<int, Terreno> index = SubTerrainAdmReference.terrainOfHero.GetIndexGlobal(relativePositionIJ);
                    int indexNumeric = SubObjectsAdmReferences.GetNumericIndex(index);

                    if (!SubTerrainAdmReference.influencedEscaques.ContainsKey(indexNumeric))
                    {
                        SubTerrainAdmReference.influencedEscaques.Add(indexNumeric, this);
                        influencedEscaques.Add(index);
                    }
                }
            }
        }
        terrainAdministrator.PaintInfluenceTown();
    }
}
