using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubUIAdminResources : MonoBehaviour
{
    [Header("Recursos")]
    public GameObject panelSelectCityResource;
    public TMP_Dropdown dropdownCitiesResource;
    private List<City> citiesNearResource;
    private City citySelectedResource = null;
    private bool isCityResourceSelected = false;
    private ResourcesClass resource;
    private UIAdministrator uIAdministrator;
    
    [Header("Referencias")]
    public SubResourcesObjAdmin subResourcesObjAdmin;

    // Start is called before the first frame update
    void Start()
    {
        dropdownCitiesResource.onValueChanged.AddListener(HandleCityResourceDropdown);
        
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();
        uIAdministrator.panels.Add(panelSelectCityResource);
    }



    public void ActivatePanelResourceDestination(List<City> cities, ResourcesClass resource)
    {
        citiesNearResource = cities;
        this.resource = resource;
        List<string> cityNames = cities.Select(city => city.nameCity).ToList();

        dropdownCitiesResource.ClearOptions();
        List<TMP_Dropdown.OptionData> options = dropdownCitiesResource.options;
        options.Insert(0, new TMP_Dropdown.OptionData(""));
        dropdownCitiesResource.AddOptions(cityNames);
        dropdownCitiesResource.value = 0;

        panelSelectCityResource.SetActive(true);
    }

    public void HandleCityResourceDropdown(int index)
    {
        isCityResourceSelected = true;
        index -= 1;
        citySelectedResource = citiesNearResource[index];
    }

    public void ResourceDestinationSelected()
    {
        if (isCityResourceSelected)
        {
            subResourcesObjAdmin.Consume(resource, false, citySelectedResource);
            panelSelectCityResource.SetActive(false);
            isCityResourceSelected = false;
        }
    }
}
