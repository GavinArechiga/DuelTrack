using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolWheelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIInputReaderSO inputReader;
    [SerializeField] private BoolEventChannel toggleCameraInputEventChannel;
    [SerializeField] private ToolTypeEventChannel switchToolEventChannel;
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
        inputReader.OnToolSelectedPerformed += SwitchTool;
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
        CalculateSelectedSegment();
    }
    
    private void ToggleToolWheel()
    {
        toolWheelEnabled = !toolWheelEnabled;
        wheelContainer.SetActive(toolWheelEnabled);
        // camera input starts enabled so we need to flip the enable/disable logic
        toggleCameraInputEventChannel.Raise(!toolWheelEnabled);

        if (toolWheelEnabled)
        {
            inputReader.DisableAllButUI();
            inputReader.EnableCursor();
        }
        else
        {
            inputReader.ReEnableMovement();
            inputReader.DisableCursor();
        }
    }
    
    private void CloseToolWheel()
    {
        toolWheelEnabled = false;
        wheelContainer.SetActive(false);
        toggleCameraInputEventChannel.Raise(true);
        inputReader.ReEnableMovement();
        inputReader.DisableCursor();
    }
    
    private void CalculateSelectedSegment()
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
    
    private void SwitchTool()
    {
        if (!toolWheelEnabled) { return; }
        switchToolEventChannel.Raise(segmentArray[lastSegmentIndex].ToolType);
        CloseToolWheel();
    }
    
}