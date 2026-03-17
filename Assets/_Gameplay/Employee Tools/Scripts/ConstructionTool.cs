using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionTool : Tool
{
    [Header("References")]
    [SerializeField] private ConstructionToolInputReaderSO inputReader;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Transform heldItemParent;
    
    private GameObject heldItem;

    private void Awake()
    {
        inputReader.OnPlaceObjectPerformed += PlaceObject;
        inputReader.OnRemoveObjectPerformed += RemoveObject;

        GridSystem.Instance.OnCurrentObjectChanged += HandleCurrentObjectChanged;
    }

    private void OnDestroy()
    {
        inputReader.OnPlaceObjectPerformed -= PlaceObject;
        inputReader.OnRemoveObjectPerformed -= RemoveObject;
    }

    public override void Enter()
    {
        inputReader.EnableInput();
        cameraManager.SwitchCamera(CameraType.TopDownCamera);
        GridSystem.Instance.EnableGrid(true);
    }
    
    public override void Exit()
    {
        inputReader.DisableInput();
        cameraManager.SwitchCamera(CameraType.PlayerCamera);
        GridSystem.Instance.EnableGrid(false);
    }

    public override void ToolUpdate()
    {
        GridSystem.Instance.UpdateGrid(transform.position, 
            playerController.CurrentFacingDirection);
    }
    
    private void PlaceObject()
    {
        GridSystem.Instance.PlaceObject();
    }
    
    private void RemoveObject()
    {
        GridSystem.Instance.RemoveObject();
    }
    
    private void HandleCurrentObjectChanged(GameObject gameObject)
    {
        if (heldItem != null)
        {
            Destroy(heldItem);
        }

        if (gameObject == null) { return; }
        
        heldItem = Instantiate(
            gameObject,
            heldItemParent.transform.position, 
            playerController.transform.rotation, 
            heldItemParent);
        
        heldItem.transform.localScale *= 0.3f;
    }
}
