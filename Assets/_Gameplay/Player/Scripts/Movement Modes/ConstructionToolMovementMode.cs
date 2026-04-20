using UnityEngine;

public class ConstructionToolMovementMode : MovementMode
{
    public override Vector3 Move(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y).normalized;
    }
}
