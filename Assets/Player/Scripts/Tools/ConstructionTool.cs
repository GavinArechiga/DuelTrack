using UnityEngine;

public class ConstructionTool : Tool
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
       base.Exit();
    }

    private void Update()
    {
        
    }
    
    private void PlaceObject()
    {
        GridSystem.Instance.PlaceObject();
    }
    
    private void RemoveObject()
    {
        GridSystem.Instance.RemoveObject();
    }
}
