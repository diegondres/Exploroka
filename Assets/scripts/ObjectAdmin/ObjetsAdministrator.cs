using UnityEngine;
/// <summary>
/// Clase pensada en manjer toda la generalidad de los objetos que esten sobre el mapa, como construcciones, recursos, etc. 
/// </summary>
public class ObjetsAdministrator : MonoBehaviour
{
    //LAS LISTAS DE OBJETOS Y LA LISTA DE LISTAS
    
    public GameObject containerResources;
    [SerializeField]
    private GameObject containerConstructions;
    [SerializeField]
    private GameObject containerFrontiers;
    
    //COSITAS
    private TerrainAdministrator terrainAdministrator;
    [SerializeField]

    void Start()
    {
        terrainAdministrator = FindAnyObjectByType<TerrainAdministrator>();
        SubObjectsAdmReferences.InicializateContainerReferences(containerConstructions, containerFrontiers);
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
