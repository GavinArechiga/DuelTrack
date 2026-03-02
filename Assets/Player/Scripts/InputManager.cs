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
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }
    
    private PlayerInput playerInput;
    private const string JUMP_ACTION_NAME = "Jump";
    private const string SPRINT_ACTION_NAME = "Sprint";
    
    
    //Tools
    public InputAction PrimaryToolAction { get; private set; }
    public InputAction SecondaryToolAction { get; private set; }
    
    private const string PRIMARY_TOOL_ACTION_NAME = "PrimaryToolAction";
    private const string SECONDARY_TOOL_ACTION_NAME = "SecondaryToolAction";
    

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        
        JumpAction = playerInput.actions.FindAction(JUMP_ACTION_NAME);
        SprintAction = playerInput.actions.FindAction(SPRINT_ACTION_NAME);
        
        PrimaryToolAction = playerInput.actions.FindAction(PRIMARY_TOOL_ACTION_NAME);
        SecondaryToolAction = playerInput.actions.FindAction(SECONDARY_TOOL_ACTION_NAME);
    }
    
    public static Vector3 DirectionToVector3(Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector3.forward,
            Direction.NorthEast => Vector3.forward + Vector3.right,
            Direction.East => Vector3.right,
            Direction.SouthEast => Vector3.back + Vector3.right,
            Direction.South => Vector3.back,
            Direction.SouthWest => Vector3.back + Vector3.left,
            Direction.West => Vector3.left,
            Direction.NorthWest => Vector3.forward + Vector3.left,
            _ => Vector3.zero,
        };
    }
    
    public static Vector3Int DirectionToVector3Int(Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector3Int.forward,
            Direction.NorthEast => Vector3Int.forward + Vector3Int.right,
            Direction.East => Vector3Int.right,
            Direction.SouthEast => Vector3Int.back + Vector3Int.right,
            Direction.South => Vector3Int.back,
            Direction.SouthWest => Vector3Int.back + Vector3Int.left,
            Direction.West => Vector3Int.left,
            Direction.NorthWest => Vector3Int.forward + Vector3Int.left,
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
