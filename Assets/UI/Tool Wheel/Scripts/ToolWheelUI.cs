using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolWheelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIInputReaderSO inputReader;
    [SerializeField] private VoidEventChannel toggleCameraFreeLookEventChannel;
    [SerializeField] private GameObject wheelContainer;
    [SerializeField] private SegmentData[] segmentArray;
    
    [Header("Settings")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;
    
    private bool toolWheelEnabled;
    private int lastSegmentIndex;
    private float segmentAngle;

    private void Awake()
    {
        inputReader.OnToggleToolWheelPerformed += ToggleToolWheel;
        segmentAngle = 360f / segmentArray.Length;
    }

    private void Start()
    {
        foreach (SegmentData segment in segmentArray)
        {
            segment.SetColor(normalColor);
        }
    }

    private void Update()
    {
        if (!toolWheelEnabled) { return; }
        SetSelectedSegment();
    }
    
    private void ToggleToolWheel()
    {
        toolWheelEnabled = !toolWheelEnabled;
        wheelContainer.SetActive(toolWheelEnabled);
        
        toggleCameraFreeLookEventChannel.Raise();
    }

    private void SetSelectedSegment()
    {
        Vector2 centerPosition = wheelContainer.transform.position;
        Vector2 distance = inputReader.PointerPosition - centerPosition;
        float angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;
        
        angle = (angle + 360f) % 360f;
        int segmentIndex = Mathf.FloorToInt(angle / segmentAngle);

        if (lastSegmentIndex != segmentIndex)
        {
            segmentArray[lastSegmentIndex].SetColor(normalColor);
            segmentArray[segmentIndex].SetColor(selectedColor);
            
            lastSegmentIndex = segmentIndex;
        }
    }
}