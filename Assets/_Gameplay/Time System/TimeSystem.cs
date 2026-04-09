using System;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
     [SerializeField, Tooltip("How long one tick is")] private float tickTime = 7;
     
     public static TimeSystem Instance { get; private set; }
     
     public event Action OnTick;
     
     public string DayName => gameTime.DayName;
     public int Days => gameTime.Days;
     public int Hours => gameTime.Hours;
     public int Minutes => gameTime.Minutes;
     public bool IsAm => gameTime.IsAm;
     
     private float elapsedTime;
     private readonly GameTime gameTime = new();


     private void Awake()
     {
         if (Instance != null && Instance != this) 
         { 
             Destroy(this); 
         } 
         else 
         { 
             Instance = this; 
         } 
     }


     private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= tickTime)
        {
           Tick();
        }
    }
    
    private void Tick()
    {
        elapsedTime = 0;
        gameTime.AddMinutes(1);
        OnTick?.Invoke();
    }
}
