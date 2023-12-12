using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdministrator : MonoBehaviour
{
    [SerializeField]
    private Heroe heroe;
    public Transform protagonista; // Referencia al transform del protagonista
    [SerializeField]
    private float minimumTime = 0.0f;
    public Vector3 offset = new Vector3(-1.5f, 18f, -9f); // Ajusta la posición relativa de la cámara
    public float suavidad = 5f; // Controla la suavidad del seguimiento

    private UIAdministrator uIAdministrator;
    private ObjetsAdministrator objetsAdministrator;
    //RECURSOS
    bool isAResourceSelected = false;
    Resource resourceSelected;
    private Camera cameraGameObject;
    private Camara camara; 
    Vector3 destino;

    private float acumulatedTime = 0.0f;




    // Start is called before the first frame update
    void Start()
    {
        objetsAdministrator = FindAnyObjectByType<ObjetsAdministrator>();
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
        cameraGameObject = FindAnyObjectByType<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!uIAdministrator.IsAnyPanelOpen())
        {
            acumulatedTime += Time.deltaTime;
            heroe.ArrowMoving();

           

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                camara=cameraGameObject.GetComponent<Camara>();
                
                camara.offset = new Vector3(0, 300, 0);
                cameraGameObject.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                cameraGameObject.orthographic = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                camara.offset = new Vector3(0, 200, -275);
                cameraGameObject.transform.rotation = Quaternion.Euler(new Vector3(27, 0, 0));
                cameraGameObject.orthographic = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                camara.offset = new Vector3(100, 350, -600);
                cameraGameObject.transform.rotation = Quaternion.Euler(new Vector3(30, -10, 0));
                cameraGameObject.orthographic = true;
                cameraGameObject.orthographicSize = 145;
                cameraGameObject.farClipPlane = 10000;
            }

            //MOUSE
            if (Input.GetMouseButtonDown(1))  //Mouse click left for hero moving
            {
                Ray rayo = cameraGameObject.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayo, out RaycastHit hit, 1000))
                {
                    destino = hit.point;
                    heroe.GenerateRoute(destino);
                }
            }
            if (!heroe.IsRouteFinish && acumulatedTime >= minimumTime && !heroe.isMoving && heroe.route.Count > 0)
            {
                heroe.MoveThroughRoute();
                acumulatedTime = 0.0f;
            }
            if (Input.GetMouseButtonDown(0)) //Mouse click left for select something
            {
                Ray rayo = cameraGameObject.ScreenPointToRay(Input.mousePosition);

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
