using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ObjectCatalogue : MonoBehaviour
{
    [Header("References")] 
    [field: SerializeField] public CatalogueObjectData ObjectTemplate { get; private set; }
    [SerializeField] private GameObject catalogueContainer;
    [SerializeField] private List<CatalogueTheme> themeList;
    
    private CatalogueTheme selectedTheme;
    private string searchString;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel constructionToolActivatedEventChannel;
    
    private void Start()
    {
        constructionToolActivatedEventChannel.AddListener(ToggleCatalogue);
        
        if (themeList.Count == 0) { return; }
        
        foreach (CatalogueTheme theme in themeList)
        {
            theme.Initialize();
            theme.SetButtonOnClick(() => SwitchTheme(theme));
        }
        
        selectedTheme = themeList[0];
    }

    private void OnDestroy()
    {
        constructionToolActivatedEventChannel.RemoveListener(ToggleCatalogue);
    }

    public void Search(string searchString)
    {
        this.searchString = searchString;
        selectedTheme.FilterThemeList(searchString);
    }

    private void SwitchTheme(CatalogueTheme newTheme)
    {
       selectedTheme.gameObject.SetActive(false);
       newTheme.gameObject.SetActive(true);
       
       selectedTheme = newTheme;
       selectedTheme.SetGridTheme();
       Search(searchString);
    }
    
    private void ToggleCatalogue(bool enable)
    {
        catalogueContainer.SetActive(enable);
    }
}
