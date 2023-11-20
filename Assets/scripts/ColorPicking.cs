using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicking : MonoBehaviour
{
    // Referencia al material compartido
    public Material materialCompartido;
    public Color initialColor;
    public Color finalColor;

    void Start()
    {
        // Obtener una instancia del material compartido para este objeto
        Material materialInstancia = Instantiate(materialCompartido);

        // Asignar un color diferente a cada instancia del material
        materialInstancia.color = ObtenerColorAleatorio();

        // Aplicar el material modificado al objeto
        GetComponent<Renderer>().material = materialInstancia;
    }

    Color ObtenerColorAleatorio()
    {
        // Generar un color aleatorio con componentes RGB ligeramente diferentes
        return Color.Lerp(initialColor, finalColor, Random.Range(0f, 1f));
    }


    }
