using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 InputVector { get; private set; }
    public InputAction JumpAction { get; private set; }
    
    private PlayerInput playerInput;
    private const string JUMP_ACTION_NAME = "Jump";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        JumpAction = playerInput.actions.FindAction(JUMP_ACTION_NAME);
    }

    private void OnMove(InputValue value)
    {
        InputVector = value.Get<Vector2>();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
