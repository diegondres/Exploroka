using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Reglas
{
  public List<ResourcesClass> resources;
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
    Debug.Log("Nombre: " + name + "\nIs Carvable: " + carvable + "\npopulation: " + population + "\nshields: " + shields + "\nModels: " + models +"\nTags: " + tags.Count);
  }

}
