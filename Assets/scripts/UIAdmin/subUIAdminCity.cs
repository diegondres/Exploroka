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

    [Header("City Information")]
    public GameObject panelCityInformation;
    public TextMeshProUGUI panelCityInfoTitle;
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI ShieldsText;
    public TextMeshProUGUI foodConsumptionText;
    public TextMeshProUGUI happinessText;
    public GameObject panelRepeatedCityName;


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
        uIAdministrator = GetComponent<UIAdministrator>();
 
        dropdownCities.onValueChanged.AddListener(HandleCityConnectionDropdown);
        uIAdministrator.panels.Add(panelBuildOrConectTown);
        uIAdministrator.panels.Add(panelConfirmationBuildTown);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildTown()
    {
        if (!SubObjectsAdmReferences.IsNameCityTaken(nameCity.text))
        {
            panelConfirmationBuildTown.SetActive(false);
            panelRepeatedCityName.SetActive(false);
            construction.BuildTown(nameCity.text, citySelected);
            citySelected = null;
            nameCity.text = "";
        }
        else{
            panelRepeatedCityName.SetActive(true);
        }
    }

    public void ActivatePanelConfirmationBuildTown()
    {
        
        nameCity.text = "";

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

    public void ActivatePanelCityInformation(City city)
    {
        panelCityInfoTitle.text = city.nameCity;
        populationText.text = "X " + city.population;
        ShieldsText.text = "X " + city.shields;
        foodConsumptionText.text = "X " + city.foodConsumption;
        happinessText.text = "X " + city.happiness;

        panelCityInformation.SetActive(true);
    }

}
