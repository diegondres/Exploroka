using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
  public Transform protagonista; // Referencia al transform del protagonista
  public Vector3 offset = new Vector3(-1.5f, 18f, -9f); // Ajusta la posición relativa de la cámara
  public float suavidad = 5f; // Controla la suavidad del seguimiento
  private Camera camara;

  void Start(){
    camara = FindAnyObjectByType<Camera>();
    camara.backgroundColor = Color.blue;
    
  }
  
  private void LateUpdate()
  {
    if (protagonista != null)
    {
      

      // Calcula la posición objetivo de la cámara
      Vector3 posicionObjetivo = protagonista.position + offset;

      // Interpola suavemente la posición de la cámara hacia la posición objetivo
      transform.position = Vector3.Lerp(transform.position, posicionObjetivo, suavidad * Time.deltaTime);
    }
    }
}
