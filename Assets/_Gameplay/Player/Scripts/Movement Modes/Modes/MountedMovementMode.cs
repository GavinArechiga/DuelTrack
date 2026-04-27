using UnityEngine;

public class MountedMovementMode : ThirdPersonMovementMode
{
    public override MovementData Move(MovementData data)
    {
        // Sets the velocity to 0 and then calls the third persons movement mode Move methode passing in this movement modes data.
        // The mounted movement mode is very similar to the third-person movement mode, so it is easier to just inherit from it.
        data.Velocity = Vector3.zero;
        return base.Move(data);
    }
}
