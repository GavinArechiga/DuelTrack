using UnityEngine;

public class CleanupTool : Tool
{

    //TODO: Replace player visual calls with events
    
    public override void Enter()
    {
        playerVisual.OnCleanupToolEntered();
    }

    public override void Exit()
    {
        playerVisual.OnCleanupToolExited();
    }

    public override void ToolUpdate()
    {
        
    }
}
