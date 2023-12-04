using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TerrainAdministrator : MonoBehaviour
{
    [SerializeField]
    private GameObject containerTerrenos;
    [SerializeField]
    private GameObject containerResources;
    private ObjetsAdministrator objetsAdministrator;

    [Header("Generacion Procedural")]
    [SerializeField]
    private GameObject Terreno;
    [SerializeField]
    private TerrainType[] terrainTypes;
    public float heightMultiplier;
    [SerializeField]
    private AnimationCurve heightCurve;
    [SerializeField]
    private Wave[] waves;
    private readonly Dictionary<Vector3, GameObject> terrainDict = new();
    private int countTerrain = 0;

    [Header("Heroe")]
    public int sizeOfTerrain = 200;
    [NonSerialized]
    public Terreno terrenoOfHero;
    private int sizeEscaque;
    private int sizeTerrainInVertices;
    private Dictionary<int, Vector3> vecindario = new();
    public List<Tuple<int, Terreno>> sorroundingEscaques = new();

    [Header("Recursos")]
    public int probabilidadRecursos = 50;
    [SerializeField]
    public GameObject resourcePrefab;

    private List<Terreno> terrenosWithoutResources = new();

    public List<GameObject> Figuras3D;


    [System.Serializable]
    public class ResourcesClass
    {
        public string name;
        public List<string> models;
        public List<GameObject> modelsPrefab;
    }
    [System.Serializable]
    public class Reglas
    {
        public List<ResourcesClass> resources;
    }


    void Awake()
    {
        objetsAdministrator = GetComponent<ObjetsAdministrator>();

        sizeEscaque = sizeOfTerrain / 20;
        sizeTerrainInVertices = sizeOfTerrain / 20;
        SetNeighboorsReference();

        CreateFirstTerrain();

        // Cargar objetos
        LoadModels();
    }

    void Update()
    {
        /*
   if(terrenosWithoutResources.Count > 0){
     foreach (Terreno terreno in terrenosWithoutResources)
    {
      StartCoroutine(InvokeBueno(terreno));
    }
    terrenosWithoutResources.Clear();
   } 
        */
    }
    private IEnumerator InvokeBueno(Terreno terreno)
    {
        yield return new WaitForSeconds(1f);
        GenerateRandomResource(terreno);
    }

    void LoadModels()
    {
        Figuras3D = new List<GameObject>();
        string path = "Reglas/reglas";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        Reglas reglas = JsonUtility.FromJson<Reglas>(jsonFile.text);
        foreach (ResourcesClass rec in reglas.resources)
        {
            foreach (string mod in rec.models) {
                GameObject goPref = AgregarModelo(mod, 80);
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




        // Probando sin resources públicos
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



    void SetNeighboorsReference()
    {
        vecindario.Add(0, new Vector3(-sizeOfTerrain, 0, -sizeOfTerrain));
        vecindario.Add(1, new Vector3(-sizeOfTerrain, 0, 0));
        vecindario.Add(2, new Vector3(-sizeOfTerrain, 0, sizeOfTerrain));
        vecindario.Add(3, new Vector3(0, 0, -sizeOfTerrain));
        vecindario.Add(4, new Vector3(0, 0, sizeOfTerrain));
        vecindario.Add(5, new Vector3(sizeOfTerrain, 0, -sizeOfTerrain));
        vecindario.Add(6, new Vector3(sizeOfTerrain, 0, 0));
        vecindario.Add(7, new Vector3(sizeOfTerrain, 0, sizeOfTerrain));
    }

    /// <summary>
    /// Creacion del primer grupo de planos que entregan la formacion inicial de terreno
    /// </summary>
    private void CreateFirstTerrain()
    {
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                CreateTerrain(new(0 + i * sizeOfTerrain, 0, 0 + j * sizeOfTerrain));
            }
        }
    }

    /// <summary>
    /// Funcion que retorna el terreno en el que esta parado el objeto que llamo a la funcion 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Terreno InWhatTerrenoAmI(Vector3 position)
    {
        Vector3 relativePosition = position / sizeOfTerrain;
        Vector3 terrainPosition = new((int)relativePosition.x * sizeOfTerrain, 0, (int)relativePosition.z * sizeOfTerrain);

        if (terrainDict.ContainsKey(terrainPosition))
        {
            terrenoOfHero = terrainDict[terrainPosition].GetComponent<Terreno>();
            return terrenoOfHero;
        }

        return null;
    }

    /// <summary>
    /// En la creacion de un terreno, agregar todos los vecinos que este tenga. Reconociendo por su posicion
    /// </summary>
    /// <param name="scripTerreno">Instancia del script del terreno recien creado</param>
    /// <param name="posNewTerreno">posicion del nuevo terreno</param>
    /// <returns></returns>
    private void ConnectWithNeighboors(Terreno scripTerreno, Vector3 posNewTerreno)
    {
        Terreno[] terr = scripTerreno.neighboors;

        for (int i = 0; i < terr.Length; i++)
        {
            if (terr[i] == null)
            {
                if (terrainDict.ContainsKey(posNewTerreno + vecindario[i]))
                {
                    Terreno temp = terrainDict[posNewTerreno + vecindario[i]].GetComponent<Terreno>();
                    terr[i] = temp;
                    temp.neighboors[7 - i] = scripTerreno;
                }
            }
        }
    }

    public Vector3 CalculateDistance(Vector3 actualPosition, Vector3 destiny)
    {
        return terrenoOfHero.CalculateDistance(actualPosition, destiny);
    }

    public Vector3 MoveHero(Vector3 position, Vector3 movement)
    {
        return terrenoOfHero.Move(position, movement);
    }

    public void SetTerrenoOfHero(Terreno terreno)
    {
        terrenoOfHero = terreno;
        FillNeighborhood(terreno);

        foreach (Terreno item in terreno.neighboors)
        {
            FillNeighborhood(item);
            foreach (Terreno item2 in item.neighboors)
            {
                FillNeighborhood(item2);
            }
        }


    }

    private void FillNeighborhood(Terreno terreno)
    {
        Terreno[] neighboor = terreno.neighboors;
        Vector3 position = terreno.GetPosition();

        for (int i = 0; i < neighboor.Length; i++)
        {
            if (neighboor[i] == null)
            {
                CreateTerrain(position + vecindario[i]);
            }
        }

    }
    public bool IsTerrainActive(Terreno terreno)
    {
        return terrenoOfHero.id == terreno.id;
    }

    public bool IsThisEscaqueVisited(Tuple<int, Terreno> visitedEscaque)
    {
        List<Tuple<int, Terreno>> visitedEscaques = sorroundingEscaques;
        foreach (Tuple<int, Terreno> escaque in visitedEscaques)
        {
            if (CompareTwoEscaques(visitedEscaque, escaque))
            {
                return true;
            }
        }
        return false;
    }

    private void CreateTerrain(Vector3 position)
    {
        GameObject newTerreno = Instantiate(Terreno, position, Quaternion.identity);
        newTerreno.transform.SetParent(containerTerrenos.transform);
        newTerreno.transform.localScale = new Vector3(sizeTerrainInVertices, sizeTerrainInVertices, sizeTerrainInVertices);

        Terreno scriptNewTerreno = newTerreno.GetComponent<Terreno>();
        ConnectWithNeighboors(scriptNewTerreno, position);
        scriptNewTerreno.id = countTerrain;
        terrainDict.Add(position, newTerreno);

        TerrainGeneration scriptGeneration = newTerreno.GetComponent<TerrainGeneration>();
        scriptGeneration.heightTerrainTypes = terrainTypes;
        scriptGeneration.heightMultiplier = heightMultiplier;
        scriptGeneration.heightCurve = heightCurve;
        scriptGeneration.waves = waves;

        countTerrain++;

        terrenosWithoutResources.Add(scriptNewTerreno);
    }

    public void GenerateRandomResource(Terreno terreno)
    {

        List<float> probs = new();
        do
        {
            int location = UnityEngine.Random.Range(0, 400);
            Vector3 position = terreno.GetGlobalPositionFromGlobalIndex(new Tuple<int, Terreno>(location, terreno));

            GameObject resource = Instantiate(resourcePrefab, position, Quaternion.identity);
            resource.transform.SetParent(containerResources.transform);
            objetsAdministrator.AddResource(resource, terreno);

            Resource resourceScript = resource.GetComponent<Resource>();
            resourceScript.SetInitialValues("Cosita", location, false, false);

            probs.Add(UnityEngine.Random.Range(0, 100));
        } while (probs.Average() > probabilidadRecursos);

    }

    public bool CompareTwoEscaques(Tuple<int, Terreno> escaque1, Tuple<int, Terreno> escaque2)
    {
        bool item1 = false;
        bool item2 = false;

        if (escaque1.Item1 == escaque2.Item1) item1 = true;
        if (escaque1.Item2.id == escaque1.Item2.id) item2 = true;

        return item1 && item2;
    }
    public int GetSizeEscaque()
    {
        return sizeEscaque;
    }
    public int GetSizeTerrainInVertices()
    {
        return sizeTerrainInVertices;
    }

}
