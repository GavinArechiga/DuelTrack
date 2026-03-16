using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionTool : Tool
{
    [SerializeField] private ConstructionToolInputReaderSO inputReader;
    [SerializeField] private CameraManager cameraManager;

    private void Awake()
    {
        inputReader.OnPlaceObjectPerformed += PlaceObject;
        inputReader.OnRemoveObjectPerformed += RemoveObject;
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
}
