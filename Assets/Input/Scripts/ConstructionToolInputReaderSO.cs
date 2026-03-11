using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName =  "ConstructionToolInputReaderSO", menuName = "Scriptable Objects/Input/ConstructionToolInputReaderSO")]
public class ConstructionToolInputReaderSO : ScriptableObject
{
    [SerializeField] private InputSourceSO inputSource;
    
    public event Action OnPlaceObjectPerformed;
    public event Action OnRemoveObjectPerformed;

    private void OnEnable()
    {
        if (!Application.isPlaying) { return; }

        inputSource.PlayerInput.ConstructionTool.PlaceObject.performed += OnPlaceObject;
        inputSource.PlayerInput.ConstructionTool.RemoveObject.performed += OnRemoveObject;
    }
    
    private void OnDisable()
    {
        inputSource.PlayerInput.ConstructionTool.PlaceObject.performed -= OnPlaceObject;
        inputSource.PlayerInput.ConstructionTool.RemoveObject.performed -= OnRemoveObject;
    }

    public void EnableInput()
    {
        inputSource.PlayerInput.ConstructionTool.Enable();
    }

    public void DisableInput()
    {
        inputSource.PlayerInput.ConstructionTool.Disable();
    }
    
    private void OnRemoveObject(InputAction.CallbackContext obj)
    {
        OnRemoveObjectPerformed?.Invoke();
    }

    private void OnPlaceObject(InputAction.CallbackContext obj)
    {
        OnPlaceObjectPerformed?.Invoke();
    }
}
