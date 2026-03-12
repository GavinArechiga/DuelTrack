using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UIInputReaderSO", menuName = "Scriptable Objects/Input/UIInputReaderSO")]
public class UIInputReaderSO : ScriptableObject
{
    [SerializeField] private InputSourceSO inputSource;
    
    public event Action OnToggleToolWheelPerformed;
    public event Action OnToolSelectedPerformed;
    
    public Vector2 PointerPosition => inputSource.PlayerInput.UI.Pointer.ReadValue<Vector2>();


    private void OnEnable()
    {
        if (inputSource == null) { return; }
        
        inputSource.PlayerInput.UI.Enable();
        inputSource.PlayerInput.UI.ToggleToolWheel.performed += OnToggleToolWheel;
        inputSource.PlayerInput.UI.SelectTool.performed += OnToolSelected;
        DisableCursor();
    }
    
    private void OnDisable()
    {
        if (inputSource == null) { return; }
        
        inputSource.PlayerInput.UI.Disable();
    }

    public void DisableAllButUI()
    {
        inputSource.PlayerInput.Disable();
        inputSource.PlayerInput.UI.Enable();
    }

    public void ReEnableMovement()
    {
        inputSource.PlayerInput.Movement.Enable();
    }
    
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnToolSelected(InputAction.CallbackContext obj)
    {
        OnToolSelectedPerformed?.Invoke();
    }

    private void OnToggleToolWheel(InputAction.CallbackContext ctx)
    {
        OnToggleToolWheelPerformed?.Invoke();
    }
}
