using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Tool[] tools;
    private Dictionary<ToolType, ITool> toolLookup;
    private ITool currentTool;
    
    public enum ToolType
    {
        None,
        Construction,
    }

    private void Awake()
    {
        toolLookup = tools.ToDictionary(tool => tool.toolType, tool => (ITool)tool);

        foreach (Tool tool in tools)
        {
            tool.Initialize(playerController);
        }
    }

    private void SwitchTool(ToolType toolType)
    {
        currentTool?.Exit();
        
        if (toolType == ToolType.None)
        {
            currentTool = null;
        }
        else
        {
            currentTool = toolLookup[toolType];
            currentTool.Enter();
        }
        
    }
}
