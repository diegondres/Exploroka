using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Reglas
{
    public List<ResourcesClass> resources;
    public List<BuildingClass> buildings;
}


[System.Serializable]
public class ResourcesClass
{
    public bool carvable;
    public int shields = 0;
    public int population = 0;
    public string name;
    public List<string> tags = new();
    public List<string> models = new();
    public List<GameObject> modelsPrefab = new();

    public int numericIndex;
    public Vector3 globalPosition;
    public ResourcesClass Clone()
    {
        ResourcesClass newResourcesClass = new()
        {
            carvable = carvable,
            shields = shields,
            population = population,
            name = name,
            models = models,
            tags = tags,
            modelsPrefab = modelsPrefab,
        };

        return newResourcesClass;
    }
    public void PrintValues()
    {
        Debug.Log("Nombre: " + name + "\nIs Carvable: " + carvable + "\npopulation: " + population + "\nshields: " + shields + "\nModels: " + models + "\nTags: " + tags.Count);
    }

}


[System.Serializable]
public class BuildingClass
{
    public string name;
    public List<string> models;
    public List<int> size;
    public int cost;
    public NeedsClass regionalNeeds;
    public int maxBuildingsInTown;
    public bool city;           //Solo la ciudad tiene esto verdadero
    public bool town;           //Solo el pueblo tiene esto verdadero
    public List<ProductionClass> produces;  // Recursos producidos
    public List<ConsumptionClass> consumes; // Tags que necesita para consumir
    public List<string> extracts;           // El extractor tiene acá la lista de tags que puede extraer, es decir, transforma todos los recursos naturales con al menos uno de esos tags en el pueblo en que se encuentra, en recursos inventario
    public List<GameObject> modelsPrefab;   // Inútil en el json, acá almacena los prefabs
}
[System.Serializable]
public class NeedsClass
{
    public List<string> resourceTagsAnd;    //Lista de tags que necesita el recurso, por ejemplo si es ["hierba","medicinal"] solo puede usar recursos con esas dos etiquetas, no recursos con solo una de ellas
    public float q;         //Cantidad de recursos requeridos
    public bool different;  //Si es verdadero, necesita Q recursos con tags resourceTagsAnd (si tag=arbol, Q = 10, entonces pueden ser 7 peumos y 3 pinos)
                            //Si es falso, necesita Q recursos con tags resourceTagsAnd diferentes, 1 de cada uno (si tag=medicinal, Q = 2, entonces puede seleccionar: 1 menta [medicinal] y 1 jengibre [medicinal]
    public bool inTown;     //Si es verdadero, busca esto en el pueblo. Más adelante habrá otras necesidades (en un radio, en la ciudad, en la zona de influencia total)
}
[System.Serializable]
public class ProductionClass
{
    public string resource;
    public float q;
}
[System.Serializable]
public class ConsumptionClass
{
    public List<string> resourceTagsAnd;    //Igual que needsClass, pero esta vez lo pide del inventario
    public float q;
    public bool different;
}
