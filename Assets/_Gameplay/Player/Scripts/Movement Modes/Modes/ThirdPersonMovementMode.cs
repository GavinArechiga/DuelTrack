using UnityEngine;

public class ThirdPersonMovementMode : MovementMode
{

    public override MovementData Move(MovementData data)
    {
        Transform playerCamTransform = CameraManager.Instance.PlayerCameraTransform;
        
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamTransform.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
        
        data.MoveDirection = camRight * data.Input.x + camForward * data.Input.y;
        
        return data;
    }
}
