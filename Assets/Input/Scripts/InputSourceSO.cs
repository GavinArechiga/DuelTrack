using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSourceSO", menuName = "Scriptable Objects/Input/InputSourceSO")]
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
