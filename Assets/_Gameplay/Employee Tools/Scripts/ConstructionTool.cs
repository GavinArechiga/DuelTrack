using System;
using UnityEngine;

public class ConstructionTool : Tool
{
    [Header("References")]
    [SerializeField] private ConstructionToolInputReaderSO inputReader;
    [SerializeField] private Transform heldItemParent;

    [Header("Events")]
    [SerializeField] private BoolEventChannel constructionToolActivatedEventChannel;
    
    private GameObject heldItem;
    private bool isCursorOverUI;
    private bool ignoreNextClick;

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

    private void OnApplicationFocus(bool hasFocus)
    {
        // fixes issue where if you click outside the window and then back in
        // you will place an object even if you clicked the UI
        ignoreNextClick = hasFocus;
    }
    

    public override void Enter()
    {
        inputReader.EnableInput();
        CameraManager.Instance.SwitchCamera(CameraType.TopDownCamera);
        GridSystem.Instance.EnableGrid(true);
        constructionToolActivatedEventChannel.Raise(true);
        UIInputReaderSO.EnableCursor();
    }
    
    public override void Exit()
    {
        inputReader.DisableInput();
        CameraManager.Instance.SwitchCamera(CameraType.PlayerCamera);
        GridSystem.Instance.EnableGrid(false);
        constructionToolActivatedEventChannel.Raise(false);
    }

    public override void ToolUpdate()
    {
        GridSystem.Instance.UpdateGrid(transform.position, 
            playerController.CurrentFacingDirection);
        // Unity does not like it when you call this from input action callbacks so we are doing it in
        // update instead and cashing the value.
        isCursorOverUI = UIInputReaderSO.IsCursorOverUI();
    }
    
    private void PlaceObject()
    {
        if (ignoreNextClick)
        {
            ignoreNextClick = false;
            return;
        }
        
        if (isCursorOverUI) { return; }
        
        GridSystem.Instance.PlaceObject();
    }
    
    private void RemoveObject()
    {
        if (ignoreNextClick)
        {
            ignoreNextClick = false;
            return;
        }
        
        if (isCursorOverUI) { return; }
        
        ignoreNextClick = false;
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
