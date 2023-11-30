using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubUIAdminCity : MonoBehaviour
{
    [Header("Fundar")]
    public GameObject panelConfirmationBuildTown;
    public TextMeshProUGUI textBuildingTownPanel;
    public TextMeshProUGUI nameCity;
    public GameObject inputCity;


    [Header("Fundar y Anexar")]
    public GameObject panelBuildOrConectTown;
    public TextMeshProUGUI textBuildOrConectTownPanel;
    public TMP_Dropdown dropdownCities;

    //REFERENCES
    private Construction construction;
    private List<City> cities;
    private City citySelected = null;
    private bool isCitySelected = false;

    private UIAdministrator uIAdministrator;
    // Start is called before the first frame update
    void Start()
    {
        construction = FindAnyObjectByType<Construction>();
        uIAdministrator = FindAnyObjectByType<UIAdministrator>();

        dropdownCities.onValueChanged.AddListener(HandleCityConnectionDropdown);
        uIAdministrator.panels.Add(panelBuildOrConectTown);
        uIAdministrator.panels.Add(panelConfirmationBuildTown);
    }

    // Update is called once per frame
    void Update()
    {
        if (uIAdministrator.IsAnyPanelOpen()) SubTerrainAdmReference.terrainOfHero.IdleTime(0);

    }

    public void BuildTown()
    {
        panelConfirmationBuildTown.SetActive(false);
        construction.BuildTown(nameCity.text, citySelected);
        citySelected = null;
        nameCity.text = "";
    }

    public void ActivatePanelConfirmationBuildTown()
    {
        if (isCitySelected)
        {
            textBuildingTownPanel.text = "¿Esta seguro que quiere crear un PUEBLO por 1 de gobernabilidad [Icon_gov]?";
            inputCity.SetActive(false);
        }
        else
        {
            textBuildingTownPanel.text = "¿Esta seguro que quiere fundar una CIUDAD por 5 de gobernabilidad [Icon_gov]?";
            inputCity.SetActive(true);

        }

        isCitySelected = false;
        panelConfirmationBuildTown.SetActive(true);
    }

    public void ActivatePanelBuildOrConectTown(List<City> cities)
    {
        this.cities = cities;
        List<string> cityNames = cities.Select(city => city.nameCity).ToList();

        dropdownCities.ClearOptions();
        List<TMP_Dropdown.OptionData> options = dropdownCities.options;
        options.Insert(0, new TMP_Dropdown.OptionData(""));
        dropdownCities.AddOptions(cityNames);
        dropdownCities.value = 0;

        panelBuildOrConectTown.SetActive(true);
        textBuildOrConectTownPanel.text = "Elige entre fundar una nueva ciudad o elegir una ciudad a la que anexar nueva ciudad";
    }

    public void HandleCityConnectionDropdown(int index)
    {
        isCitySelected = true;
        index -= 1;
        citySelected = cities[index];
        panelBuildOrConectTown.SetActive(false);
        ActivatePanelConfirmationBuildTown();
    }
}
