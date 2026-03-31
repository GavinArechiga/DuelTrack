using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CatalogueTheme : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private ThemeObjectListSO themeObjectListSO;
    [SerializeField] private ObjectCatalogue objectCatalogue;
    
    private TextMeshProUGUI buttonText;
    private readonly List<CatalogueObjectData> spawnedThemeObjects = new() ;
    
    public void Initialize()
    {
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = themeObjectListSO.ThemeName;
        
        foreach (GridObjectData gridObject in themeObjectListSO.GridObjects)
        {
            CatalogueObjectData objectUI = Instantiate(objectCatalogue.ObjectTemplate, transform);
            spawnedThemeObjects.Add(objectUI);
            objectUI.NameText.text = gridObject.Name;
            objectUI.Image.sprite = gridObject.Sprite;
            
            // future proofing for when this needs to be run when the UI is enabled and not just in start
            objectUI.Button.onClick.RemoveAllListeners();
            objectUI.Button.onClick?.AddListener(() => GridSystem.Instance.SetSelectedObject(gridObject.Prefab));
        }
        
        SetGridTheme();
    }

    public void SetButtonOnClick(Action buttonAction)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(buttonAction.Invoke);
    }

    public void FilterThemeList(string searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            foreach (CatalogueObjectData themeObject in spawnedThemeObjects)
            {
                themeObject.gameObject.SetActive(true);
            }
            
            return;
        }
        
        List<CatalogueObjectData> filteredList = spawnedThemeObjects.Where(
                themeObject =>
                themeObject.NameText.text.ToLower()
                    .Contains(searchString.ToLower())).ToList();

        foreach (CatalogueObjectData themeObject in spawnedThemeObjects)
        {
            themeObject.gameObject.SetActive(filteredList.Contains(themeObject));
        }
    }

    public void SetGridTheme()
    {
        GridSystem.Instance.SetSelectedTheme(themeObjectListSO);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
