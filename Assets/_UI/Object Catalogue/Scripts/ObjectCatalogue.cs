using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCatalogue : MonoBehaviour
{
    [SerializeField] private GridObjectListSO gridObjectListSO;
    [SerializeField] private GameObject objectContainer;
    [SerializeField] private CatalogueObjectData objectTemplate;

    private void Start()
    {
        foreach (GridObjectData gridObject in gridObjectListSO.gridObjects)
        {
            CatalogueObjectData uiElement = Instantiate(objectTemplate, objectContainer.transform);
            uiElement.NameText.text = gridObject.name;
            uiElement.Image.sprite = gridObject.sprite;
            uiElement.gameObject.SetActive(true);
           
        }
    }
    
    
}
