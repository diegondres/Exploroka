using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainAdministrator : MonoBehaviour
{
  [SerializeField]
  private GameObject prefabFrontier;
  private ObjetsAdministrator objetsAdministrator;
  private SubTerrainAdmGeneration subTerrainAdmGeneration;
 

  void Awake()
  {
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
    subTerrainAdmGeneration = FindAnyObjectByType<SubTerrainAdmGeneration>();
    subTerrainAdmGeneration.SetNeighboorsReference();
    subTerrainAdmGeneration.CreateFirstTerrain();
    SubObjectsAdmReferences.Inicializate();
  }

  void Update()
  {
    if (SubTerrainAdmReference.terrenosWithoutResources.Count > 0)
    {
      foreach (Terreno terreno in SubTerrainAdmReference.terrenosWithoutResources)
      {
        StartCoroutine(InvokeBueno(terreno));
      }
      SubTerrainAdmReference.terrenosWithoutResources.Clear();
    }
  }
  
  public void PutFrontierInEscaque(Tuple<int, Terreno> index, Vector3 offset, Quaternion rotation, int city)
  {
    Vector3 position = index.Item2.GetGlobalPositionFromGlobalIndex(index) + offset;

    if (SubObjectsAdmReferences.frontiers.ContainsKey(city))
    {
      SubObjectsAdmReferences.frontiers[city].Add(Instantiate(prefabFrontier, position, rotation, SubObjectsAdmReferences.containerFrontiers.transform));
    }
    else
    {
      List<GameObject> fronteritas = new()
            {
                Instantiate(prefabFrontier, position,rotation, SubObjectsAdmReferences.containerFrontiers.transform)
            };
      SubObjectsAdmReferences.frontiers[city] = fronteritas;
    }
  }

  public Terreno GetTerrenoScriptFromId(int id)
  {
    return subTerrainAdmGeneration.GetTerrenoScriptFromId(id);
  }
  public void PaintInfluenceTown()
  {
    foreach (var pair in SubTerrainAdmReference.influencedEscaques)
    {
      Tuple<int, Terreno> globalIndex = SubTerrainAdmReference.GetIndexFromNumeric(pair.Key, this);
      globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.magenta);
      StartCoroutine(ReturnToOriginal(globalIndex));
    }
  }
  public void SetTerrenoOfHero(Terreno terreno)
  {
    SubTerrainAdmReference.terrainOfHero = terreno;
    subTerrainAdmGeneration.FillNeighborhood(terreno);

    foreach (Terreno item in terreno.neighboors)
    {
      subTerrainAdmGeneration.FillNeighborhood(item);
    }
  }
  private IEnumerator InvokeBueno(Terreno terreno)
  {
    yield return new WaitForSeconds(1f);
    objetsAdministrator.GenerateRandomResource(terreno);
  }
  private IEnumerator ReturnToOriginal(Tuple<int, Terreno> tuple)
  {
    yield return new WaitForSeconds(1f);
    tuple.Item2.ReturnPixelToOriginal(tuple.Item1);
  }
}
