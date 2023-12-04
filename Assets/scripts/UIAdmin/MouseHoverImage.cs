using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject test;
    public TextMeshProUGUI text;
    private bool isOver = false;
    [SerializeField]
    private string textHover;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOver)
        {
            test.SetActive(true);
            text.text = textHover;
            test.transform.position = transform.position + new Vector3(-200,0,0);
        }
        else
        {
            test.SetActive(false);

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("AAAAAAAAAAAAAAAAA");
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        Debug.Log("BBBBBBBBBBBBBBB");
    }
}
