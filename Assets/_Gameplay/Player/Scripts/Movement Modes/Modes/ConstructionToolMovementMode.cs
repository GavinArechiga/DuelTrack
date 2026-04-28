using UnityEngine;

public class ConstructionToolMovementMode : MovementMode
{
    public override MovementData Move(MovementData data)
    {
        data.MoveDirection = new Vector3(data.Input.x, 0, data.Input.y).normalized;
        return data;
    }
}
