using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ResourcesClass
{
  public string name;
  public List<string> models;
  public List<GameObject> modelsPrefab;

  public ResourcesClass Clone(){
        ResourcesClass newResourcesClass = new()
        {
            name = name,
            models = models,
            modelsPrefab = modelsPrefab
        };

        return newResourcesClass;
    }
}

[System.Serializable]
public class Reglas
{
  public List<ResourcesClass> resources;
}


public class TerrainAdministrator : MonoBehaviour
{
  [SerializeField]
  private GameObject prefabFrontier;
  private ObjetsAdministrator objetsAdministrator;
  private SubTerrainAdmGeneration subTerrainAdmGeneration;

  public int modelScale = 80;
  public List<GameObject> Figuras3D = new();

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
        int k = 0;
        foreach (ResourcesClass rec in reglas.resources)
        {
            foreach (string mod in rec.models) {
                GameObject goPref = AgregarModelo(mod, 80);
                Figuras3D.Add(goPref);
                //Agregar modelo
                if(!modelosRecursos.ContainsKey(rec.name))
                {
                    modelosRecursos[rec.name] = new List<int>();
                }
                modelosRecursos[rec.name].Add(k);
                k++;
            }
        }
  }

  GameObject AgregarModelo(string dir, float escala = 1)
  {
    Texture2D tex = Resources.Load("Modelos/" + dir, typeof(Texture2D)) as Texture2D;

    // Probando sin resources publicos
    GameObject go = new GameObject();
    go.AddComponent<MeshRenderer>();
    MeshFilter filter = go.AddComponent<MeshFilter>();
    filter.mesh = (Mesh)Resources.Load("Modelos/" + dir, typeof(Mesh));

    go.GetComponent<Renderer>().material.mainTexture = tex;
    go.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");


    go.transform.localScale = new Vector3(-0.1f * escala, 0.1f * escala, 0.1f * escala);
    go.transform.rotation = Quaternion.Euler(0, 180, 0);

    go.transform.position = new Vector3(0, 0, -500);
    go.transform.tag = "bloquePrefab";

    return go;
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
      foreach (Terreno item2 in item.neighboors)
      {
        subTerrainAdmGeneration.FillNeighborhood(item2);
      }
    }
  }
  private IEnumerator ReturnToOriginal(Tuple<int, Terreno> tuple)
  {
    yield return new WaitForSeconds(1f);
    tuple.Item2.ReturnPixelToOriginal(tuple.Item1);
  }
}
