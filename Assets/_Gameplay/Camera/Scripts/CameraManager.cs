using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private BoolEventChannel toggleCameraInputEventChannel;
    [SerializeField] private CinemachineInputAxisController inputComponent;

    private void Awake()
    {
        toggleCameraInputEventChannel.AddListener(ToggleCameraInput);
    }

    private void ToggleCameraInput(bool enable)
    {
        inputComponent.enabled = enable;
    }
}
