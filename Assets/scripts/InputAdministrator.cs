using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdministrator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private SubResourcesObjAdmin subResourcesObjAdmin;
    [SerializeField]
    private UIAdministrator uIAdministrator;
    [SerializeField]
    private ObjetsAdministrator objetsAdministrator;
    [SerializeField]
    private Camera camara;
    [Header("Hero cosas")]
    [SerializeField]
    private Heroe heroe;
    [SerializeField]
    private float minimumTime = 0.0f;
    //RECURSOS
    bool isAResourceSelected = false;
    ResourcesClass resourceSelected;
    Vector3 destino;

    private float acumulatedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!uIAdministrator.IsAnyPanelOpen())
        {
            acumulatedTime += Time.deltaTime;
            heroe.ArrowMoving();

            //MOUSE
            if (Input.GetMouseButtonDown(1))  //Mouse click left for hero moving
            {
                Ray rayo = camara.ScreenPointToRay(Input.mousePosition);

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
                Ray rayo = camara.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayo, out RaycastHit hit, 1000))
                {
                    Vector3 destino = hit.point;
                    Vector3 relativePosition = SubTerrainAdmReference.terrainOfHero.GetRelativePositionInVertices(destino);
                    Tuple<int, Terreno> globalIndex = SubTerrainAdmReference.terrainOfHero.GetIndexGlobal(relativePosition);

                    //TODO: es posible que queramos mejorar esta logica en el futuro, si es que hay mas tipos de cosas mas alla de recursos y construcciones.
                    Tuple<GameObject, int> thing = SubObjectsAdmReferences.IsSomethingBuiltInHere(globalIndex);

                    if (thing != null)
                    {
                        if(thing.Item2 == 0){ //0 is for buildings!
                            thing.Item1.GetComponent<Building>().PrintBuildingValues();
                        }
                        else if(thing.Item2 == 1){
                            resourceSelected = SubResourcesObjAdmin.GetResourceInfo(globalIndex);
                            resourceSelected.PrintValues();
                            isAResourceSelected = true;
                        }
                    }
                    SubObjectsAdmReferences.SelectEscaqueToBuildIn(globalIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.E) && isAResourceSelected)
            {
                isAResourceSelected = false;
                subResourcesObjAdmin.PreConsume(resourceSelected.numericIndex);
            }
        }

    }
}
