using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ToolWheelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIInputReaderSO inputReader;
    [SerializeField] private GameObject wheelContainer;
    [SerializeField] private SegmentData[] segmentArray;
    
    [Header("Settings")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel toggleToolWheelEventChannel;
    [SerializeField] private ToolTypeEventChannel switchToolEventChannel;
    
    private bool toolWheelEnabled;
    private int selectedSegmentIndex;
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
        toggleToolWheelEventChannel.Raise(!toolWheelEnabled);

        if (toolWheelEnabled)
        {
            inputReader.DisableAllButUI();
            UIInputReaderSO.EnableCursor();
        }
        else
        {
            inputReader.ReEnableMovement();
            UIInputReaderSO.DisableCursor();
        }
    }
    
    private void CloseToolWheel()
    {
        toolWheelEnabled = false;
        wheelContainer.SetActive(false);
        toggleToolWheelEventChannel.Raise(true);
        inputReader.ReEnableMovement();
    }
    
    private void CalculateSelectedSegment()
    {
        Vector2 centerPosition = wheelContainer.transform.position;
        Vector2 distance = inputReader.PointerPosition - centerPosition;
        float angle = Mathf.Atan2(distance.x, distance.y) * Mathf.Rad2Deg;
        
        angle = (angle + 360f) % 360f;
        int currentSegmentIndex = Mathf.FloorToInt(angle / segmentAngle);

        if (selectedSegmentIndex != currentSegmentIndex)
        {
            segmentArray[selectedSegmentIndex].SetColor(normalColor);
            segmentArray[currentSegmentIndex].SetColor(selectedColor);
            
            selectedSegmentIndex = currentSegmentIndex;
        }
    }
    
    private void SwitchTool()
    {
        if (!toolWheelEnabled) { return; }
        
        UIInputReaderSO.DisableCursor();
        switchToolEventChannel.Raise(segmentArray[selectedSegmentIndex].ToolType);
        CloseToolWheel();
    }
    
}