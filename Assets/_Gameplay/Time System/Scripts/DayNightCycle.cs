using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    //TODO: Interpolate between ticks instead of only updating every tick
    
    [SerializeField] private Light sunLight;
    [SerializeField] private float minDarknessLevel = 0.2f;
    [Header("Color")]
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;
    [SerializeField] private AnimationCurve colorCurve;

    private void Start()
    {
        TimeSystem.Instance.OnTick += HandleOnTick;
    }

    private void HandleOnTick(GameTime gameTime)
    {
        float dayPercentage = CalculateDayPercentage(gameTime);
        
        CalculateSunRotation(dayPercentage);
        CalculateLightIntensity(dayPercentage);
        
        sunLight.color = Color.Lerp(dayColor, nightColor, colorCurve.Evaluate(
            gameTime.Hours24 + gameTime.Minutes / 60f));
    }

    private float CalculateDayPercentage(GameTime gameTime)
    {
        float timeOfDay = gameTime.Hours24 + (gameTime.Minutes / 60f);
        return timeOfDay / 24f;
    }
    
    private void CalculateSunRotation(float dayPercentage)
    {
        // Need to subtract 90 degrees to get a more realistic sun angle
        const float offset = 90f;
        float angle = (dayPercentage * 360f) - offset;
        
        // divide the angle by half on the y to create an arc
        sunLight.transform.rotation = Quaternion.Euler(angle, angle / 2, 0f);
    }
    
    private void CalculateLightIntensity(float dayPercentage)
    {
        // Increases from 0 -> 1 during the day and 1 -> 0 during the night
        float intensity = Mathf.Sin(dayPercentage * Mathf.PI);
        intensity = Mathf.Clamp(intensity, 0f, 1f);
        
        sunLight.intensity = intensity;
        sunLight.bounceIntensity = intensity;

        float ambientIntensity = Mathf.Clamp(intensity, minDarknessLevel, 1);
        
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.reflectionIntensity = ambientIntensity;
    }
}
