using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CatalogueTheme : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private ThemeObjectListSO themeObjectListSO;
    [SerializeField] private CatalogueObjectData objectTemplate;
    
    private TextMeshProUGUI buttonText;
    
    private void Start()
    {
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = themeObjectListSO.ThemeName;
        
        foreach (GridObjectData gridObject in themeObjectListSO.GridObjects)
        {
            CatalogueObjectData objectUI = Instantiate(objectTemplate, transform);
            objectUI.NameText.text = gridObject.Name;
            objectUI.Image.sprite = gridObject.Sprite;
            
            // future proofing for when this needs to be run when the UI is enabled and not just in start
            objectUI.Button.onClick.RemoveAllListeners();
            objectUI.Button.onClick?.AddListener(() => GridSystem.Instance.SetSelectedObject(gridObject.Prefab));

            objectUI.gameObject.SetActive(true);
        }
    }

    public void SetButtonOnClick(Action buttonAction)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(buttonAction.Invoke);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
