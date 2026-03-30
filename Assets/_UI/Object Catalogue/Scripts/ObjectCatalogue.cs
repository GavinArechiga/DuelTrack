using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCatalogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject catalogueContainer;
    [SerializeField] private List<CatalogueTheme> themeList;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel constructionToolActivatedEventChannel;
    
    private void Start()
    {
        constructionToolActivatedEventChannel.AddListener(ToggleCatalogue);
        
        if (themeList.Count == 0) { return; }

        foreach (CatalogueTheme theme in themeList)
        {
            theme.SetButtonOnClick(() => SwitchTheme(theme));
        }
    }

    private void SwitchTheme(CatalogueTheme newTheme)
    {
        foreach (CatalogueTheme theme in themeList.Where(theme => theme != newTheme))
        {
            theme.gameObject.SetActive(false);
        }
        
        newTheme.gameObject.SetActive(true);
    }
    
    private void ToggleCatalogue(bool enable)
    {
        catalogueContainer.SetActive(enable);
    }
}
