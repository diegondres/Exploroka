using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase pensada en manjer toda la generalidad de los objetos que esten sobre el mapa, como construcciones, recursos, etc. 
/// </summary>
public class ObjetsAdministrator : MonoBehaviour
{
    //LAS LISTAS DE OBJETOS Y LA LISTA DE LISTAS
    private List<List<Tuple<int, Terreno, GameObject>>> allObjects = new();
    private List<Tuple<int, Terreno, GameObject>> constructions = new();
    private List<Tuple<int, Terreno, GameObject>> resources = new();

    //CONSTRUCCION
    private Tuple<int, Terreno> buildingGlobalIndex;
    public bool isBuildingLocationSelected = false;

    //COSAS
    private TerrainAdministrator terrainAdministrator;

    // Start is called before the first frame update
    void Start()
    {
        terrainAdministrator = GetComponent<TerrainAdministrator>();
    }

    public void SelectEscaqueToBuildIn(Tuple<int, Terreno> globalIndex)
    {
        if (terrainAdministrator.IsThisEscaqueVisited(globalIndex))
        {
            if (isBuildingLocationSelected)
            {
                if (globalIndex.Item1 == buildingGlobalIndex.Item1)
                {
                    buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);
                    isBuildingLocationSelected = false;
                    return;
                }
                buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);
            }
            isBuildingLocationSelected = true;
            SetBuildingLocation(globalIndex);
            globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.gray);
        }
    }

    public void SetBuildingLocation(Tuple<int, Terreno> pos)
    {
        buildingGlobalIndex = pos;
    }
    public Vector3 GetBuildingLocation()
    {
        isBuildingLocationSelected = false;
        buildingGlobalIndex.Item2.PaintPixelInfluence(buildingGlobalIndex.Item1, Color.red);

        return buildingGlobalIndex.Item2.GetGlobalPositionFromGlobalIndex(buildingGlobalIndex);
    }

    public void AddBuilding (GameObject building){
        Terreno terreno = terrainAdministrator.terrenoOfHero;
        Vector3 relativePositionBuilding = terreno.GetRelativePositionInVertices(building.transform.position);
        Tuple<int, Terreno> indexBuilding = terreno.GetIndexGlobal(relativePositionBuilding);
        
        constructions.Add(new Tuple<int, Terreno, GameObject>(indexBuilding.Item1, indexBuilding.Item2, building));
    }
}
