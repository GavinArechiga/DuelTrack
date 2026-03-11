using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UIInputReaderSO", menuName = "Scriptable Objects/Input/UIInputReaderSO")]
public class UIInputReaderSO : ScriptableObject
{
    [SerializeField] private InputSourceSO inputSource;
    
    public event Action OnToggleToolWheelPerformed;
    public Vector2 PointerPosition => inputSource.PlayerInput.UI.Pointer.ReadValue<Vector2>();
    
    private bool toolWheelEnabled;


    private void OnEnable()
    {
        if (inputSource == null) { return; }
        
        inputSource.PlayerInput.UI.Enable();
        inputSource.PlayerInput.UI.ToggleToolWheel.performed += OnToggleToolWheel;
    }

    private void OnToggleToolWheel(InputAction.CallbackContext ctx)
    {
        OnToggleToolWheelPerformed?.Invoke();
    }

    private void OnDisable()
    {
        inputSource.PlayerInput.UI.Disable();
    }
}
