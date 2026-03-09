using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "MovementInputManagerSO", menuName = "Scriptable Objects/Input/MovementInputManagerSO")]
public class MovementInputManagerSO : ScriptableObject
{
    public Vector2 InputVector => playerInput.Movement.Move.ReadValue<Vector2>();
    public bool JumpWasPerformed => playerInput.Movement.Jump.WasPerformedThisFrame();
    public bool SprintPressed => playerInput.Movement.Sprint.IsPressed();
    
    private PlayerActions playerInput;
    
    private void OnEnable()
    {
        playerInput = new PlayerActions();
        playerInput.Movement.Enable();
    }
    
    private void OnDisable()
    {
        playerInput.Movement.Disable();
    }
}
