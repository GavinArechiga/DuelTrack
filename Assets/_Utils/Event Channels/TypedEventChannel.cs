using System;
using UnityEngine;

public abstract class TypedEventChannel<T> : ScriptableObject
{
    private event Action<T> EventHandler;

    public void Raise(T value)
    {
        EventHandler?.Invoke(value);
    }

    public void AddListener(Action<T> listener)
    {
        EventHandler += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        EventHandler -= listener;
    }

    public void ClearListeners()
    {
        EventHandler = null;
    }
}
