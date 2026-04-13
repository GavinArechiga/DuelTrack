using System;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
     [SerializeField, Tooltip("How long one tick is")] private float tickTime = 1;
     
     public static TimeSystem Instance { get; private set; }
     
     public event Action<GameTime> OnTick;
     public event Action OnNewDay;
     
     // It is important that these properties stay read-only.
     // if they are set directly, then it con break the time calculation. Use the public methods instead. 
     
     // The DayName property ensures the day name and the index are always in sync.
     // This value cannot be set directly,
     // instead change the current-day index, and the day name will update automatically.
     public string DayName => dayNames[currentDayIndex];
     public int Days { get; private set; } = 1;
     public int Hours { get; private set; } = 6;
     public int Hours24 => GetHours24();
     public int Minutes  { get; private set; }
     public bool IsAm { get; private set; } = true;
     public bool IsPaused { get; private set; }
     
     private float elapsedTime;
     private int currentDayIndex;
     
     private readonly string[] dayNames =
     {
         "Monday",
         "Tuesday",
         "Wednesday",
         "Thursday",
         "Friday",
         "Saturday",
         "Sunday",
     };
     
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
        if (IsPaused) { return; }
        
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= tickTime)
        {
           Tick();
        }
    }
    
    private void Tick()
    {
        elapsedTime = 0;
        AddMinutes(1);

        var gameTime = new GameTime
        {
            DayName = this.DayName,
            Days = this.Days,
            Hours = this.Hours,
            Hours24 =  this.Hours24,
            Minutes = this.Minutes,
            IsAm = this.IsAm,
        };
        
        OnTick?.Invoke(gameTime);
    }
    
    private void CalculateTime()
    {
        if (Minutes >= 60)
        {
            Minutes = 0;
            Hours += 1;
        }
        
        if (Hours > 12)
        {
            Hours = 1;
        }
        
        CalculateDayRollover();
    }

    private void CalculateDayRollover()
    {
        if (Hours == 12 && Minutes == 0)
        {
            IsAm = !IsAm;
            
            // if just switched from PM -> AM, it's a new day
            if (IsAm)
            {
                Days += 1;

                if (Days > 30)
                {
                    Days = 1;
                }

                currentDayIndex = (currentDayIndex + 1) % dayNames.Length;
                OnNewDay?.Invoke();
            }
        }
    }

    private int GetHours24()
    {
        // Returns hours converted to 24-hour time
        
        int hours24;
        
        if (IsAm)
        {
            hours24 = (Hours == 12) ? 0 : Hours;
        }
        else
        {
            hours24 = (Hours == 12) ? 12 : Hours + 12;
        }
        
        return hours24;
    }
    
    // some of these functions are not called and were added to future-proof the time system.
    // if they are not needed, then you can remove them.
    
    public void AddDays(int days)
    {
        Days += days;
        CalculateTime();
    }

    public void AddHours(int hours)
    {
        Hours += hours;
        CalculateTime();
    }
    
    public void AddMinutes(int minutes)
    {
        Minutes += minutes;
        CalculateTime();
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void UnPause()
    {
        IsPaused = false;
    }
}
