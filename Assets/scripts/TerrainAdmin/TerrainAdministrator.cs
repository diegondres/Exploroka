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
  [NonSerialized]
  public List<Tuple<Tuple<int, Terreno>, int>> newInfluenceEscaques = new();

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

    if (newInfluenceEscaques.Count > 0)
    {
      foreach (Tuple<Tuple<int, Terreno>, int> tuple in newInfluenceEscaques)
      {
        Vector3 relativePosition = tuple.Item1.Item2.GetRelativePositionFromGlobalIndex(tuple.Item1);
        Tuple<int, Terreno> indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(-1, 0, 0));

        if (!SubTerrainAdmReference.influencedEscaques.ContainsKey(SubObjectsAdmReferences.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(-9, 0, 0), Quaternion.identity);
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(1, 0, 0));
        if (!SubTerrainAdmReference.influencedEscaques.ContainsKey(SubObjectsAdmReferences.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(9, 0, 0), Quaternion.identity);
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(0, 0, -1));
        if (!SubTerrainAdmReference.influencedEscaques.ContainsKey(SubObjectsAdmReferences.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(0, 0, -9), Quaternion.Euler(new Vector3(0, -90, 0)));
        }

        indexSides = tuple.Item1.Item2.GetIndexGlobal(relativePosition + new Vector3(0, 0, 1));
        if (!SubTerrainAdmReference.influencedEscaques.ContainsKey(SubObjectsAdmReferences.GetNumericIndex(indexSides)))
        {
          PutFrontierInEscaque(tuple, new Vector3(0, 0, 9), Quaternion.Euler(new Vector3(0, 90, 0)));
        }
      }
      newInfluenceEscaques.Clear();
    }
  }

  private void PutFrontierInEscaque(Tuple<Tuple<int, Terreno>, int> tuple, Vector3 offset, Quaternion rotation)
  {
    Vector3 position = tuple.Item1.Item2.GetGlobalPositionFromGlobalIndex(tuple.Item1) + offset;
    if (SubObjectsAdmReferences.frontiers.ContainsKey(tuple.Item2))
    {
      SubObjectsAdmReferences.frontiers[tuple.Item2].Add(Instantiate(prefabFrontier, position, rotation));
    }
    else
    {
      List<GameObject> fronteritas = new()
            {
                Instantiate(prefabFrontier, position,rotation)
            };
      SubObjectsAdmReferences.frontiers[tuple.Item2] = fronteritas;
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
      Tuple<int, Terreno> globalIndex = SubObjectsAdmReferences.GetIndexFromNumeric(pair.Key, this);
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
