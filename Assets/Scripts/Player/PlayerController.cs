using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    public event Action<float> OnPlayerMove;

    [SerializeField] private Transform playerCam;

    #region Speed Variables
    [SerializeField] private float walkSpeed;
    private float idleSpeed;
    private float currentSpeed;
    private Coroutine speedCoroutine;
    #endregion
    
    private CharacterController characterController;
    private InputManager inputManager;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        Move();
    }
    
    #region Movement
    private void Move()
    {
        CalculateSpeed();

        if (inputManager.InputVector != Vector2.zero)
        {
            // creates camera relative movement by multiplying the cameras right and forward axis by the input 
            Vector3 playerCamForward = playerCam.right * inputManager.InputVector.x + playerCam.forward * inputManager.InputVector.y;
            // gets rid of the y so you don't walk on air
            Vector3 movement = Vector3.ProjectOnPlane(playerCamForward, Vector3.up).normalized;
            
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
            characterController.Move(movement * (currentSpeed * Time.deltaTime));
        }
        
        OnPlayerMove?.Invoke(currentSpeed);
    }
    
    private void CalculateSpeed()
    {
        // ? = if else
        float desiredSpeed = inputManager.InputVector != Vector2.zero ? walkSpeed : idleSpeed;
        const float transitionSpeed = 0.2f;

        if (speedCoroutine == null & !Mathf.Approximately(currentSpeed, desiredSpeed))
        {
            speedCoroutine = StartCoroutine(LerpSpeed(desiredSpeed, transitionSpeed));
        }
    }

    // Slowly increments speed over time
    private IEnumerator LerpSpeed(float targetSpeed, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            float interpolationValue = elapsedTime / duration;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, interpolationValue);
            yield return null;
        }
        
        currentSpeed = targetSpeed;
        speedCoroutine = null;
    }
    #endregion
}
