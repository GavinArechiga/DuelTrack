using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera buildCamera;
    
    [Header("Events")]
    [SerializeField] private BoolEventChannel toggleCameraInputEventChannel;
    
    public static CameraManager Instance  { get; private set; }

    public event Action<CameraType> OnCameraChange;
    
    private CinemachineInputAxisController inputComponent;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        
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
        buildCamera.Priority = 1;
        playerCamera.Priority = 0;
        
        buildCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        
    }
    private void EnablePlayerCamera()
    {
        // Switch cameras
        buildCamera.Priority = 0;
        playerCamera.Priority = 1;
        
        buildCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
    }

    private void TogglePlayerCameraInput(bool enable)
    {
        inputComponent.enabled = enable;
    }
}