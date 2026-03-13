using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "MovementInputReaderSO", menuName = "Scriptable Objects/Input/MovementInputReaderSO")]
public class MovementInputReaderSO : ScriptableObject
{
    [SerializeField] private InputSourceSO inputSource;
    
    public Vector2 InputVector => inputSource.PlayerInput.Movement.Move.ReadValue<Vector2>();
    public bool JumpWasPerformed => inputSource.PlayerInput.Movement.Jump.WasPerformedThisFrame();
    public bool SprintPressed => inputSource.PlayerInput.Movement.Sprint.IsPressed();
    
    private void OnEnable()
    {
        if (inputSource == null) { return; }
        inputSource.PlayerInput.Movement.Enable();
    }
    
    private void OnDisable()
    {
        if (inputSource == null) { return; }
        inputSource.PlayerInput.Movement.Disable();
    }
}
