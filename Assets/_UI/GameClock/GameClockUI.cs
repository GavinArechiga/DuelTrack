using System;
using TMPro;
using UnityEngine;

public class GameClockUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text timeText;

    private void Start()
    {
        TimeSystem.Instance.OnTick += HandleOnTick;
    }
    
    private void OnDestroy()
    {
        TimeSystem.Instance.OnTick -= HandleOnTick;
    }

    private void HandleOnTick()
    {
        string dayName = TimeSystem.Instance.DayName;
        int days = TimeSystem.Instance.Days;
        int hours = TimeSystem.Instance.Hours;
        int minutes = TimeSystem.Instance.Minutes;
        bool isAm = TimeSystem.Instance.IsAm;
        
        dateText.text = $"{dayName} {days}";
        timeText.text = $"{hours}:{minutes:00} {(isAm ? "AM" : "PM")}";
    }

    

}
