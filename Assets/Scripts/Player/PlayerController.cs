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
    public event Action OnPlayerJump;
    public event Action OnPlayerLand;
    
    #region Movement Variables
    [SerializeField] private Transform playerCam;
    [SerializeField] private float walkSpeed;
    private float idleSpeed;
    private float currentSpeed;
    private Coroutine speedCoroutine;
    #endregion

    #region Jump Variables

    [SerializeField] private float jumpHeight;
    private readonly float gravity = Physics.gravity.y;
    private Vector3 velocity;

    #endregion
    
    private CharacterController characterController;
    private InputManager inputManager;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        velocity = characterController.velocity;
    }

    private void Update()
    {
        Move();
    }
    
    #region Movement
    private void Move()
    {
        CalculateSpeed();
        Vector3 movement = new Vector3();

        if (inputManager.InputVector != Vector2.zero)
        {
            // creates camera relative movement by multiplying the cameras right and forward axis by the input 
            Vector3 playerCamForward = playerCam.right * inputManager.InputVector.x + playerCam.forward * inputManager.InputVector.y;
            // gets rid of the y so you don't walk on air
            movement = Vector3.ProjectOnPlane(playerCamForward, Vector3.up).normalized;
            
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
        
        Jump();
        
        Vector3 finalMove = (movement * currentSpeed) + (velocity.y * Vector3.up);
        characterController.Move(finalMove *  Time.deltaTime);
        
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
    
    private void Jump()
    {
        if (characterController.isGrounded && velocity.y < 0f)
        {
            velocity.y = 0;
            OnPlayerLand?.Invoke();
        }

        if (inputManager.JumpAction.WasPressedThisFrame() && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            OnPlayerJump?.Invoke();
        }

        velocity.y += gravity * Time.deltaTime;
    }

    #endregion
}
