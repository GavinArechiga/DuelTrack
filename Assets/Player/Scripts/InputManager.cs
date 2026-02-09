using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public Vector2 InputVector { get; private set; }
    // Button actions
    public InputAction JumpAction { get; private set; }
    public InputAction SprintAction { get; private set; }
    
    private PlayerInput playerInput;
    private const string JUMP_ACTION_NAME = "Jump";
    private const string SPRINT_ACTION_NAME = "Sprint";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        JumpAction = playerInput.actions.FindAction(JUMP_ACTION_NAME);
        SprintAction = playerInput.actions.FindAction(SPRINT_ACTION_NAME);
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
