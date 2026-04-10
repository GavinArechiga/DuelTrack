using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ToolType
{
    None,
    Construction,
}

public class ToolManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ToolTypeEventChannel switchToolEventChannel;
    [SerializeField] private Tool[] toolArray;
    
    private Dictionary<ToolType, Tool> toolLookup;
    private Tool currentTool;


    private void Awake()
    {
        toolLookup = toolArray.ToDictionary(tool => tool.toolType, tool => tool);
        switchToolEventChannel.AddListener(SwitchTool);

        foreach (Tool tool in toolArray)
        {
            tool.Initialize(playerController);
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
            currentTool.Exit();
            currentTool.gameObject.SetActive(false);
            currentTool = null;
        }

        if (toolType == ToolType.None) { return; }
        
        currentTool = toolLookup[toolType];
        currentTool.gameObject.SetActive(true);
        currentTool.Enter();
        
    }
}


