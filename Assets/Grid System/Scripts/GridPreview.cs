using System;
using System.Collections.Generic;
using UnityEngine;

public class GridPreview : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Material previewMaterial;
    private GameObject previewObject;
    private Renderer[] previewRenderers;
    private Vector3Int lastFrontCellPosition;
    
    private void Update()
    {
        ShowObjectPreview();
    }

    private void Start()
    {
        gridSystem.OnObjectPlaced += () => Destroy(previewObject);
        gridSystem.OnObjectRemoved += () => ChangePreviewObjectColor(Color.white);
    }

    private void ShowObjectPreview()
    {
        PlacementData data = gridSystem.GetPlacementData();
        
        if (CheckIfSameFrontCell(data.FrontCell))
        {
            return;
        }
        
        previewObject = Instantiate(gridSystem.CurrentlySelectedObject, data.CellCenter, data.Rotation);
        previewObject.name = $"{gridSystem.CurrentlySelectedObject.name} Preview";
        previewObject.GetComponentInChildren<Collider>().enabled = false;
        
        FixPreviewZFighting();
        ChangePreviewMaterial(data.HasOverlap);
    }
    
    private void FixPreviewZFighting()
    {
        Vector3 upOffset = Vector3.up * 0.01f;
        Vector3 backOffset = -previewObject.transform.forward * 0.01f;
        
        previewObject.transform.position += upOffset + backOffset;
    }
    
    private void ChangePreviewMaterial(bool hasOverlap)
    {
        Material previewMaterialInstance = Instantiate(previewMaterial);
        previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in previewRenderers)
        {
            rend.material = previewMaterialInstance;
        }
        
        ChangePreviewObjectColor(hasOverlap ? Color.red : Color.white);
    }
    
    private void ChangePreviewObjectColor(Color color)
    {
        if (!previewObject) { return; }
        
        color.a = previewMaterial.color.a;
        foreach (Renderer rend in previewRenderers)
        {
            rend.material.color = color;
        }
    }
    
    private bool CheckIfSameFrontCell(Vector3Int frontCell)
    {
        if (lastFrontCellPosition == frontCell) {
            return true;
        }

        lastFrontCellPosition = frontCell;
        
        if (previewObject)
        {
            Destroy(previewObject);
        }

        return false;
    }
}
