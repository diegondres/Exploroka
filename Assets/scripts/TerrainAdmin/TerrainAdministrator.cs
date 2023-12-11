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

  public List<GameObject> Figuras3D;
  public Dictionary<string, List<int>> modelosRecursos;

    [System.Serializable]
    public class ResourcesClass
    {
        public string name;
        public List<string> models;
        public bool carvable;
        public float population;
        public float shields;
        public List<GameObject> modelsPrefab;
    }
    [System.Serializable]
    public class Reglas
    {
        public List<ResourcesClass> resources;
    }

 

  void Awake()
  {
    objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
    subTerrainAdmGeneration = FindAnyObjectByType<SubTerrainAdmGeneration>();
    subTerrainAdmGeneration.SetNeighboorsReference();
    subTerrainAdmGeneration.CreateFirstTerrain();
    SubObjectsAdmReferences.Inicializate();
        modelosRecursos = new Dictionary<string, List<int>>();
         LoadModels();

  }
void LoadModels()
    {
        Figuras3D = new List<GameObject>();
        string path = "Reglas/reglas";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        Reglas reglas = JsonUtility.FromJson<Reglas>(jsonFile.text);
        int k = 0;
        foreach (ResourcesClass rec in reglas.resources)
        {
            foreach (string mod in rec.models) {
                GameObject goPref = AgregarModelo(mod, 80);
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
        // Lo antiguo que funcionaba si quiero que cargue los datos de un archivo
        /*
		go = new GameObject ();
		Mesh holderMesh = new Mesh();
		ObjImporter newMesh = new ObjImporter();
		//holderMesh = newMesh.ImportFile("Assets/Resources/Tresde/" + dir + ".obj");
        holderMesh = FastObjImporter.Instance.ImportFile("Assets/Resources/TresDe/" + dir + ".obj");
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
		MeshFilter filter = go.AddComponent<MeshFilter>();
		filter.mesh = holderMesh;
		//filter.mesh = Resources.Load<Mesh>(dir);
		*/
        // Fin funcionar

        Texture2D tex = null;

        tex = Resources.Load("Modelos/" + dir, typeof(Texture2D)) as Texture2D;

        if(tex == null)
        {
            tex = Resources.Load("Modelos/Texture", typeof(Texture2D)) as Texture2D;
        }


        // Probando sin resources p�blicos
        GameObject go = new GameObject();
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        MeshFilter filter = go.AddComponent<MeshFilter>();
        filter.mesh = (Mesh)Resources.Load("Modelos/" + dir, typeof(Mesh));

        go.GetComponent<Renderer>().material.mainTexture = tex;
        go.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");


        go.transform.localScale = new Vector3(-0.1f * escala, 0.1f * escala, 0.1f * escala);
        go.transform.rotation = Quaternion.Euler(0, 180, 0);
        
        go.transform.position = new Vector3(0, 0, -500);
        go.transform.tag = "bloquePrefab";

        Figuras3D.Add(go);
        return go;
    }
  void Update()
  {
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
