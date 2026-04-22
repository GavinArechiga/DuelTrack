using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName =  "CleanupToolInputReaderSO", menuName = "Scriptable Objects/Input/CleanupToolInputReaderSO")]
public class CleanupToolInputReaderSO : ScriptableObject
{
    [SerializeField] private InputSourceSO inputSource;

    public event Action OnToggleMount;

    private void OnEnable()
    {
        if (inputSource == null) { return; }
        inputSource.PlayerInput.CleanupTool.ToggleMount.performed += HandleToggleMount;
    }

    private void HandleToggleMount(InputAction.CallbackContext obj)
    {
        OnToggleMount?.Invoke();
    }

    private void OnDisable()
    {
        if (inputSource == null) { return; }
        inputSource.PlayerInput.CleanupTool.ToggleMount.performed -= HandleToggleMount;
    }
    
    public void EnableInput()
    {
        inputSource.PlayerInput.CleanupTool.Enable();
    }

    public void DisableInput()
    {
        inputSource.PlayerInput.CleanupTool.Disable();
    }
}
