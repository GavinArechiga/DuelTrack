using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName =  "ConstructionToolInputManagerSO", menuName = "Scriptable Objects/Input/ConstructionToolInputManagerSO")]
public class ConstructionToolInputManagerSO : ScriptableObject
{
    public InputAction PlaceObjectAction => playerInput.ConstructionTool.PlaceObject;
    public InputAction RemoveObjectAction => playerInput.ConstructionTool.RemoveObject;
    
    private PlayerActions playerInput;

    private void OnEnable()
    {
        playerInput = new PlayerActions();
    }

    public void EnableInput()
    {
        playerInput.ConstructionTool.Enable();
    }

    public void DisableInput()
    {
        playerInput.ConstructionTool.Disable();
    }
}
