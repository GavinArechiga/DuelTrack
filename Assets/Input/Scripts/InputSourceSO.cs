using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSourceSO", menuName = "Scriptable Objects/Input/InputSourceSO")]
// The input source is referenced by the input readers.
// By default, the input readers get loaded before the input source which creates an error because player input will be null.
//To fix this we manually set the execution order to a lower value so the input source is loaded first.  
[DefaultExecutionOrder(-1)]
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
