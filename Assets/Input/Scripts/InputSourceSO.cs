using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSourceSO", menuName = "Scriptable Objects/Input/InputSourceSO")]
[DefaultExecutionOrder(-1)] // causes issues if this is not run before the input readers
public class InputSourceSO : ScriptableObject
{
    public PlayerActions PlayerInput { get; private set; }

    private void OnEnable()
    {
        PlayerInput = new PlayerActions();
        PlayerInput.Enable();
    }

    private void OnDisable()
    {
        PlayerInput.Disable();
    }
}
