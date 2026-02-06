using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    public event Action<float> OnPlayerMove;
    public event Action OnPlayerJump;
    public event Action OnPlayerLand;
    public event Action OnPlayerFall;
    
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

    private bool wasGrounded;
    private bool fallTriggered;

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
        Fall();
        Jump();
    }
    
    #region Movement
    private void Move()
    {
        CalculateSpeed();
        var movement = new Vector3();

        if (inputManager.InputVector != Vector2.zero)
        {
            // creates camera relative movement by multiplying the cameras right and forward axis by the input 
            Vector3 playerCamForward = playerCam.right * inputManager.InputVector.x + playerCam.forward * inputManager.InputVector.y;
            // gets rid of the y so you don't walk on air
            movement = Vector3.ProjectOnPlane(playerCamForward, Vector3.up).normalized;
            
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
        
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

    // increments speed over multiple frames to create a smoother transition
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
        if (!inputManager.JumpAction.WasPressedThisFrame() || !characterController.isGrounded) { return; }

        // calculates the required velocity so that the player reaches the jump height at the peak of the jump
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        OnPlayerJump?.Invoke();
    }
    
    private void Fall()
    {
        const float safetyFallThreshold = -0.5f;
        switch (characterController.isGrounded)
        {
            case true when velocity.y < 0 && !wasGrounded:
                velocity.y = 0;
                OnPlayerLand?.Invoke();
                break;
            case false when wasGrounded:
                OnPlayerFall?.Invoke();
                break;
            // safety check for rare case when walking off a ledge and wasGrounded is not set correctly 
            case false when !fallTriggered && velocity.y < safetyFallThreshold:
                fallTriggered = true;
                OnPlayerFall?.Invoke();
                break;
        }
        
        wasGrounded = characterController.isGrounded;
        velocity.y += gravity * Time.deltaTime;
    }

    #endregion
}
