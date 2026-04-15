using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting")]
    [SerializeField] private Light sunLight;
    // graphs the intensity throughout the day, set in the inspector.
    // x-axis = current time out of 24 hours
    // y-axis = intensity value. Max intensity should be 1
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float minDarknessLevel = 0.2f;
    [Header("Color")]
    [SerializeField] private Color dayColor;
    [SerializeField] private Color nightColor;
    // graphs the color of the direction light throughout the day, set in the inspector.
    // x-axis = current time out of 24 hours.
    // y-axis = what percentage we are at from the day color to the night color.
    // 1 would be full night color and 0 would be full-day color
    [SerializeField] private AnimationCurve colorCurve;
    
    private bool hasInitialized;
    private float previousDayPercentage;
    private float currentDayPercentage;

    private void Start()
    {
        TimeSystem.Instance.OnTick += HandleOnTick;
    }

    private void OnDestroy()
    {
        TimeSystem.Instance.OnTick -= HandleOnTick;
    }

    private void Update()
    {
        // The day/night cycle looks really choppy if we completely tie it to the tick rate, and we have a low tick rate.
        // to fix this, we calculate an interpolated percentage every frame up until the current percentage that is tied to the tick rate.
        // When the percentage changes from the tick rate, we then lerp from the old percentage to the new one based on tick progress. 
        if (!hasInitialized) { return; }
        
        float tickProgress = TimeSystem.Instance.TickProgress;
        
        float previousPercentage = previousDayPercentage;
        float currentPercentage = currentDayPercentage;

        // small fix for when we wrap from PM -> AM
        if (currentPercentage < previousPercentage)
        {
            currentPercentage += 1f;
        }
        
        // % 1 wraps the value around back to 0 if it is greater than 1
        float interpolatedPercentage = Mathf.Lerp(previousPercentage, 
            currentPercentage, tickProgress) % 1f;
        
        CalculateSunRotation(interpolatedPercentage);
        CalculateLightIntensity(interpolatedPercentage);
        
        sunLight.color = Color.Lerp(
            dayColor,
            nightColor,
            colorCurve.Evaluate(interpolatedPercentage * 24f)
        );
    }

    private void HandleOnTick(GameTime gameTime)
    {
        float newPercentage = CalculateDayPercentage(gameTime);
        
        if (!hasInitialized)
        {
           previousDayPercentage = newPercentage;
           currentDayPercentage = newPercentage;
           hasInitialized = true;
           return;
        }
        
        previousDayPercentage = currentDayPercentage;
        currentDayPercentage = newPercentage;
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
        float intensity = intensityCurve.Evaluate(dayPercentage * 24);
        
        sunLight.intensity = intensity;
        sunLight.bounceIntensity = intensity;

        float ambientIntensity = Mathf.Clamp(intensity, minDarknessLevel, 1);
        
        // because of URP we need to set the ambient and reflection intensity to make the scene darker at night
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.reflectionIntensity = ambientIntensity;
    }
}
