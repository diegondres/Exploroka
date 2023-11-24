using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Clase pensada en manjer toda la generalidad de los objetos que esten sobre el mapa, como construcciones, recursos, etc. 
/// </summary>
public class ObjetsAdministrator : MonoBehaviour
{
    //LAS LISTAS DE OBJETOS Y LA LISTA DE LISTAS
    public GameObject containerConstructions;
    [SerializeField]
    private GameObject containerResources;
    [SerializeField]
    private GameObject[] resourcePrefab;
    public int probabilidadRecursos = 50;

    //COSITAS
    private TerrainAdministrator terrainAdministrator;

    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
    }

    public void GenerateRandomResource(Terreno terreno)
    {
        List<float> probs = new();
        do
        {
            int localPosition = UnityEngine.Random.Range(0, 400);
            Vector3 position = terreno.GetGlobalPositionFromGlobalIndex(new Tuple<int, Terreno>(localPosition, terreno));

            int random = UnityEngine.Random.Range(0, resourcePrefab.Length);
            GameObject resource = Instantiate(resourcePrefab[random], position, Quaternion.identity);
            resource.transform.SetParent(containerResources.transform);
            SubObjectsAdmReferences.AddResource(resource, terreno);

            probs.Add(UnityEngine.Random.Range(0, 100));
        } while (probs.Average() > probabilidadRecursos);
    }

    public void DeleteAllFrontiersCity(int city)
    {
        if (SubObjectsAdmReferences.frontiers.ContainsKey(city))
        {
            foreach (GameObject frontier in SubObjectsAdmReferences.frontiers[city])
            {
                Destroy(frontier);
            }

        }
    }

}
