using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannel : ScriptableObject
{
    private event Action EventHandler;

    public void Raise()
    {
        EventHandler?.Invoke();
    }

    public void AddListener(Action listener)
    {
        EventHandler += listener;
    }

    public void RemoveListener(Action listener)
    {
        EventHandler -= listener;
    }
    
    public void ClearListeners()
    {
        EventHandler = null;
    }
}
