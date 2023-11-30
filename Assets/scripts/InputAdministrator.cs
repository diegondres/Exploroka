using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdministrator : MonoBehaviour
{
    [SerializeField]
    private Heroe heroe;
    [SerializeField]
    private float minimumTime = 0.0f;
    private UIAdministrator uIAdministrator;
    private ObjetsAdministrator objetsAdministrator;
    //RECURSOS
    bool isAResourceSelected = false;
    Resource resourceSelected;
    private Camera camara;


    private float acumulatedTime = 0.0f;
    private float distancia = 0.0f;
    private Vector3 destino;



    // Start is called before the first frame update
    void Start()
    {
        objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
        camara = FindAnyObjectByType<Camera>();

    }

    // Update is called once per frame
    void Update()
    {

        acumulatedTime += Time.deltaTime;
        if (!uIAdministrator.IsAnyPanelOpen())
        {
            heroe.ArrowMoving();

            //MOUSE
            if (Input.GetMouseButtonDown(1))  //Mouse click left for hero moving
            {
                Ray rayo = camara.ScreenPointToRay(Input.mousePosition);
                Plane plano = new(Vector3.up, transform.position);
                distancia = 0.0f;

                if (plano.Raycast(rayo, out distancia))
                {
                    destino = rayo.GetPoint(distancia);
                }
                heroe.distanciaEnVector = SubTerrainAdmReference.CalculateDistance(transform.position, destino);
            }
            if (Vector3.Magnitude(heroe.distanciaEnVector) > 2.8f && acumulatedTime >= minimumTime)
            {
                heroe.MouseMoving(heroe.distanciaEnVector);
                heroe.distanciaEnVector = SubTerrainAdmReference.CalculateDistance(transform.position, destino);
                acumulatedTime = 0.0f;
            }
            if (Input.GetMouseButtonDown(0)) //Mouse click left for select something
            {
                Ray rayo = camara.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayo, out RaycastHit hit, 1000))
                {
                    Vector3 destino = hit.point;
                    Vector3 relativePosition = SubTerrainAdmReference.terrainOfHero.GetRelativePositionInVertices(destino);
                    Tuple<int, Terreno> globalIndex = SubTerrainAdmReference.terrainOfHero.GetIndexGlobal(relativePosition);


                    //TODO: es posible que queramos mejorar esta logica en el futuro, si es que hay mas tipos de cosas mas alla de recursos y construcciones.
                    GameObject thing = SubObjectsAdmReferences.IsSomethingBuiltInHere(globalIndex);

                    if (thing != null)
                    {
                        if (thing.GetComponent<Building>() != null) thing.GetComponent<Building>().PrintBuildingValues();
                        else if (thing.GetComponent<Resource>() != null)
                        {
                            resourceSelected = thing.GetComponent<Resource>();
                            resourceSelected.PrintResourceValues();
                            isAResourceSelected = true;
                        }
                    }
                    SubObjectsAdmReferences.SelectEscaqueToBuildIn(globalIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.E) && isAResourceSelected)
            {
                isAResourceSelected = false;
                resourceSelected.PreConsume();             
            }
        }

    }
}
