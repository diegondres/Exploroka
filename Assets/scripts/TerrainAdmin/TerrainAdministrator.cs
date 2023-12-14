using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainAdministrator : MonoBehaviour
{
  [SerializeField]
  private GameObject prefabFrontier;
  private ObjetsAdministrator objetsAdministrator;
  private SubTerrainAdmGeneration subTerrainAdmGeneration;

  public int modelScale = 80;

  public List<Tuple<GameObject, ResourcesClass>> prefabsResources = new();
  public Dictionary<string, List<int>> modelosRecursos = new();

  void Awake()
  {
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
    subTerrainAdmGeneration = FindAnyObjectByType<SubTerrainAdmGeneration>();
    subTerrainAdmGeneration.SetNeighboorsReference();
    subTerrainAdmGeneration.CreateFirstTerrain();
    SubObjectsAdmReferences.Inicializate();

    LoadModels();
  }
  void LoadModels()
  {
    string path = "Reglas/reglas";
    TextAsset jsonFile = Resources.Load<TextAsset>(path);
    Reglas reglas = JsonUtility.FromJson<Reglas>(jsonFile.text);
    int iterator = 0;

    foreach (ResourcesClass resource in reglas.resources)
    {
      foreach (string model in resource.models)
      {
        prefabsResources.Add(new Tuple<GameObject, ResourcesClass>(GeneratePrefab(model, 80), resource));

        if (!modelosRecursos.ContainsKey(resource.name))
        {
          modelosRecursos[resource.name] = new List<int>();
        }

        modelosRecursos[resource.name].Add(iterator);
        iterator++;
      }
    }
  }

  GameObject GeneratePrefab(string dir, float escala = 1)
  {
    Texture2D tex = Resources.Load("Modelos/" + dir, typeof(Texture2D)) as Texture2D ?? Resources.Load("Modelos/Texture", typeof(Texture2D)) as Texture2D;

        // Probando sin resources publicos
        GameObject prefab = new GameObject();
    prefab.AddComponent<MeshRenderer>();
    MeshFilter filter = prefab.AddComponent<MeshFilter>();
    filter.mesh = (Mesh)Resources.Load("Modelos/" + dir, typeof(Mesh));

    prefab.GetComponent<Renderer>().material.mainTexture = tex;
    prefab.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");

    prefab.transform.localScale = new Vector3(-0.1f * escala, 0.1f * escala, 0.1f * escala);
    prefab.transform.SetPositionAndRotation(new Vector3(0, 0, -500), Quaternion.Euler(0, 180, 0));
    prefab.transform.tag = "bloquePrefab";

    return prefab;
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
  public void SetTerrenoOfHero(Terreno terreno)
  {
    SubTerrainAdmReference.terrainOfHero = terreno;
    subTerrainAdmGeneration.FillNeighborhood(terreno);

    foreach (Terreno item in terreno.neighboors)
    {
      subTerrainAdmGeneration.FillNeighborhood(item);
      foreach (Terreno item2 in item.neighboors)
      {
        subTerrainAdmGeneration.FillNeighborhood(item2);
      }
    }
  }

  //--------------------------------------------------------------------//
  //---------------------METODOS DEPRECADOS-----------------------------//
  //--------------------------------------------------------------------//
  /*
  public void PaintInfluenceTown()
  {
    foreach (var pair in SubTerrainAdmReference.influencedEscaques)
    {
      Tuple<int, Terreno> globalIndex = SubTerrainAdmReference.GetIndexFromNumeric(pair.Key, this);
      globalIndex.Item2.PaintPixelInfluence(globalIndex.Item1, Color.magenta);
      StartCoroutine(ReturnToOriginal(globalIndex));
    }
  }
  private IEnumerator ReturnToOriginal(Tuple<int, Terreno> tuple)
  {
    yield return new WaitForSeconds(1f);
    tuple.Item2.ReturnPixelToOriginal(tuple.Item1);
  }*/
}
