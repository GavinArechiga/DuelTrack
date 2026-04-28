using System.Collections.Generic;
using UnityEngine;

public class ObjectCatalogue : MonoBehaviour
{
    [Header("References")] 
    [field: SerializeField] public CatalogueObjectData ObjectTemplate { get; private set; }
    [SerializeField] private GameObject catalogueContainer;
    [SerializeField] private List<CatalogueTheme> themeList;
    
    private CatalogueTheme selectedTheme;
    private string searchString;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel toggleToolWheelEventChannel;
    
    private bool constructionToolIsActive;
    
    private void Start()
    {
       ToolManager.OnToolEntered += OnToolEntered;
       ToolManager.OnToolExited += OnToolExited;
       
        toggleToolWheelEventChannel.AddListener(ToggleCatalogue);
        
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
        ToolManager.OnToolEntered -= OnToolEntered;
        ToolManager.OnToolExited -= OnToolExited;
        
        toggleToolWheelEventChannel.RemoveListener(ToggleCatalogue);
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

    private void OnToolEntered(ToolType toolType)
    {
        if (toolType != ToolType.Construction) { return; }
        constructionToolIsActive = true;
        ToggleCatalogue(true);
    }

    private void OnToolExited(ToolType toolType)
    {
        if (toolType != ToolType.Construction) { return; }
        constructionToolIsActive = false;
        ToggleCatalogue(false);
    }
    
    private void ToggleCatalogue(bool enable)
    {
        if (constructionToolIsActive)
        {
            catalogueContainer.SetActive(enable);
        }
    }
}
