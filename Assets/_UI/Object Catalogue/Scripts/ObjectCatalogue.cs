using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCatalogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridObjectListSO gridObjectListSO;
    [SerializeField] private GameObject catalogueContainer;
    [SerializeField] private GameObject objectContainer;
    [SerializeField] private CatalogueObjectData objectTemplate;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel constructionToolActivatedEventChannel;

    private void Start()
    {
        constructionToolActivatedEventChannel.AddListener(ToggleCatalogue);
        
        foreach (GridObjectData gridObject in gridObjectListSO.gridObjects)
        {
            CatalogueObjectData objectUI = Instantiate(objectTemplate, objectContainer.transform);
            objectUI.NameText.text = gridObject.name;
            objectUI.Image.sprite = gridObject.sprite;
            
            // future proofing for when this needs to be run when the UI is enabled and not just in start
            objectUI.Button.onClick.RemoveAllListeners();
            objectUI.Button.onClick?.AddListener(() => GridSystem.Instance.SetSelectedObject(gridObject.prefab));

            objectUI.gameObject.SetActive(true);
        }
    }

    private void ToggleCatalogue(bool enable)
    {
        catalogueContainer.SetActive(enable);
    }
    
    
}
