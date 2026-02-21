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
    
    public enum Direction
    {
        Forward,
        Backward,
        Left,
        Right,
    }
    
    private PlayerInput playerInput;
    private const string JUMP_ACTION_NAME = "Jump";
    private const string SPRINT_ACTION_NAME = "Sprint";
    
    
    //Tools
    public InputAction PrimaryToolAction { get; private set; }
    
    private const string PRIMARY_TOOL_ACTION_NAME = "PrimaryToolAction";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        JumpAction = playerInput.actions.FindAction(JUMP_ACTION_NAME);
        SprintAction = playerInput.actions.FindAction(SPRINT_ACTION_NAME);
        
        PrimaryToolAction = playerInput.actions.FindAction(PRIMARY_TOOL_ACTION_NAME);
    }
    
    public static Vector3 DirectionToVector3(Direction direction)
    {
        return direction switch
        {
            Direction.Forward  => Vector3.forward,
            Direction.Backward => Vector3.back,
            Direction.Left     => Vector3.left,
            Direction.Right    => Vector3.right,
            _ => Vector3.zero,
        };
    }
    
    public static Vector3Int DirectionToVector3Int(Direction direction)
    {
        return direction switch
        {
            Direction.Forward  => Vector3Int.forward,
            Direction.Backward => Vector3Int.back,
            Direction.Left     => Vector3Int.left,
            Direction.Right    => Vector3Int.right,
            _ => Vector3Int.zero,
        };
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
