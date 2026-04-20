using UnityEngine;

public class ThirdPersonMovementMode : MovementMode
{

    public override Vector3 Move(Vector2 input)
    {
        Transform playerCamTransform = CameraManager.Instance.PlayerCameraTransform;
        
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamTransform.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
        
        return camRight * input.x + camForward * input.y;
    }
}
