using System;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private Material previewMaterial;
    private GameObject previewObject;
    private Renderer[] previewRenderers;
    private Vector3Int lastFrontCellPosition;
    
    // Fixes Z fighting between a placed object and the preview by slightly shifting the previews position by 0.1f
    // on the upwards and backwards directions.

    private void Start()
    {
        PlacementSystem.OnObjectPlaced += () => Destroy(previewObject);
        PlacementSystem.OnObjectRemoved += () => ChangePreviewObjectColor(Color.white);
    }
    
    public void ShowObjectPreview(Vector3Int frontCell, Vector3 worldCellCenter, GameObject prefab, Quaternion rotation)
    {
        
        if (CheckIfSameFrontCell(frontCell))
        {
            return;
        }
        
        previewObject = Instantiate(prefab, worldCellCenter, rotation);
        previewObject.GetComponentInChildren<Collider>().enabled = false;
        
        FixPreviewZFighting();
        //ChangePreviewMaterial(hasOverlap); //TODO: figure out who should own overlap
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

    private void ChangePreviewObjectColor(Color color)
    {
        if (previewRenderers == null) { return; }
        
        color.a = previewMaterial.color.a;
        foreach (Renderer rend in previewRenderers)
        {
            rend.material.color = color;
        }
    }
}
