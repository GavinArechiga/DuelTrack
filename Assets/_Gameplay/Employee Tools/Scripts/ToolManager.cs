using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ToolType
{
    None,
    Construction,
    Cleanup,
}

public class ToolManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private ToolTypeEventChannel switchToolEventChannel;
    [SerializeField] private Tool[] toolArray;
    
    public static event Action<ToolType> OnToolEntered;
    public static event Action<ToolType> OnToolExited;
    
    private Dictionary<ToolType, Tool> toolLookup;
    private Tool currentTool;
    
    private void Awake()
    {
        toolLookup = toolArray.ToDictionary(tool => tool.toolType, tool => tool);
        switchToolEventChannel.AddListener(SwitchTool);

        foreach (Tool tool in toolArray)
        {
            tool.Initialize(playerController, playerVisual);
        }
    }

    private void OnDestroy()
    {
        switchToolEventChannel.RemoveListener(SwitchTool);
    }

    private void Update()
    {
        if (currentTool)
        {
            currentTool.ToolUpdate();
        }
    }
    
    private void SwitchTool(ToolType toolType)
    {
        if (currentTool != null)
        {
            OnToolExited?.Invoke(currentTool.toolType);
            currentTool.Exit();
            currentTool.gameObject.SetActive(false);
            currentTool = null;
        }

        if (toolType == ToolType.None) { return; }
        
        currentTool = toolLookup[toolType];
        currentTool.gameObject.SetActive(true);
        OnToolEntered?.Invoke(currentTool.toolType);
        currentTool.Enter();
    }
}