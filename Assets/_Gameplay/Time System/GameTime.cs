public class GameTime
{
    // property keeps day name synced with index
    public string DayName => dayNames[currentDayIndex];
    public int Days { get; private set; } = 1;
    public int Hours { get; private set; } = 6;
    public int Minutes  { get; private set; }
    public bool IsAm { get; private set; } = true;
    
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
    
    public void AddMinutes(int amount)
    {
        Minutes += amount;
        CalculateTime();
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
            }
        }
    }
}
