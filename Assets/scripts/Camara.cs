using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform protagonista; // Referencia al transform del protagonista
    public Vector3 offset = new Vector3(-1.5f, 18f, -9f); // Ajusta la posición relativa de la cámara
    public float suavidad = 5f; // Controla la suavidad del seguimiento
    private Camera camara;

    void Start()
    {
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            offset = new Vector3(0, 300, 0);
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            camara.orthographic = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            offset = new Vector3(0, 200, -275);
            transform.rotation = Quaternion.Euler(new Vector3(27, 0, 0));
            camara.orthographic = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            offset = new Vector3(100, 350, -600);
            transform.rotation = Quaternion.Euler(new Vector3(30, -10, 0));
            camara.orthographic = true;
            camara.orthographicSize = 90;
        }


    }
}
