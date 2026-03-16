using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera topDownCamera;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel toggleCameraInputEventChannel;

    public event Action<CameraType> OnCameraChange;
    
    private CinemachineInputAxisController inputComponent;

    private void Awake()
    {
        toggleCameraInputEventChannel.AddListener(TogglePlayerCameraInput);
        inputComponent = playerCamera.GetComponent<CinemachineInputAxisController>();
    }
    
    public void SwitchCamera(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.PlayerCamera:
                EnablePlayerCamera();
                break;
            case CameraType.TopDownCamera:
                EnableTopDownCamera();
                break;
            default:
                goto case CameraType.PlayerCamera;
        }
        
        OnCameraChange?.Invoke(cameraType);
    }
    private void EnableTopDownCamera()
    {
        // Switch cameras
        topDownCamera.Priority = 1;
        playerCamera.Priority = 0;
        
        topDownCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
    }
    private void EnablePlayerCamera()
    {
        // Switch cameras
        topDownCamera.Priority = 0;
        playerCamera.Priority = 1;
        
        topDownCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        
        // Recenter camera
        var playerCamOrbitalFollow = playerCamera.gameObject.GetComponent<CinemachineOrbitalFollow>();
        playerCamOrbitalFollow.HorizontalAxis.Value = 0;
    }

    private void TogglePlayerCameraInput(bool enable)
    {
        inputComponent.enabled = enable;
    }
}