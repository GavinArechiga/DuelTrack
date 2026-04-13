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

    private void HandleOnTick(GameTime gameTime)
    {
        dateText.text = $"{gameTime.DayName} {gameTime.Days}";
        timeText.text = $"{gameTime.Hours}:{gameTime.Minutes:00} {(gameTime.IsAm ? "AM" : "PM")}";
    }
}
