using System;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannel toggleFreeLookEventChannel;
    [SerializeField] private CinemachineOrbitalFollow freeLookComponent;
    private bool freeLookEnabled = true;

    private void Awake()
    {
        toggleFreeLookEventChannel.AddListener(ToggleFreeLook);
    }

    private void ToggleFreeLook()
    {
        freeLookEnabled = !freeLookEnabled;
        freeLookComponent.enabled = freeLookEnabled;
    }
}
