using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionTool : Tool
{
    [SerializeField] private ConstructionToolInputManagerSO inputManager;

    private void Awake()
    {
        inputManager.PlaceObjectAction.performed += PlaceObject;
        inputManager.RemoveObjectAction.performed += RemoveObject;
    }

    private void OnDestroy()
    {
        inputManager.PlaceObjectAction.performed -= PlaceObject;
        inputManager.RemoveObjectAction.performed -= RemoveObject;
    }

    public override void Enter()
    {
        inputManager.EnableInput();
        GridSystem.Instance.EnableGrid(true);
    }
    
    public override void Exit()
    {
        inputManager.DisableInput();
        GridSystem.Instance.EnableGrid(false);
    }

    public override void ToolUpdate()
    {
        GridSystem.Instance.UpdateGrid(transform.position, playerController.CurrentFacingDirection);
    }
    
    private void PlaceObject(InputAction.CallbackContext ctx)
    {
        GridSystem.Instance.PlaceObject();
    }
    
    private void RemoveObject(InputAction.CallbackContext ctx)
    {
        GridSystem.Instance.RemoveObject();
    }
}
