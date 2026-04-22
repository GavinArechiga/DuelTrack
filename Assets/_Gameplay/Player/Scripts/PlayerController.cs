using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Events
    // player controller -> player visual events
    public event Action<float> OnPlayerMove;
    public event Action OnPlayerJump;
    public event Action OnPlayerLand;
    public event Action OnPlayerFall;
    #endregion
    
    #region Movement Variables

    public Direction CurrentFacingDirection { get; private set; } = Direction.North;

    [Header("References")]
    [SerializeField] private MovementInputReaderSO inputReader;
    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float runSpeed = 6;
    
    private float idleSpeed;
    private float currentSpeed;
    private Quaternion previousRotation;
    private Coroutine rotationCoroutine;
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
    private MovementMode currentMovementMode;
    
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        SwitchMovementMode(MovementModeType.ThirdPerson);
    }
    
    private void Start()
    {
        CameraManager.Instance.OnCameraChange += HandleOnCameraChange;
        velocity = characterController.velocity;
        previousRotation = transform.rotation;
    }

    private void OnDestroy()
    {
        CameraManager.Instance.OnCameraChange -= HandleOnCameraChange;
    }

    private void Update()
    {
        Move();
        Fall();
        Jump();
    }
    
    private void HandleOnCameraChange(CameraType cameraType)
    {
        switch (cameraType)
        {
            case CameraType.PlayerCamera:
                SwitchMovementMode(MovementModeType.ThirdPerson);
                break;
            case CameraType.BuildCamera:
                SwitchMovementMode(MovementModeType.ConstructionTool);
                break;
            default:
                SwitchMovementMode(MovementModeType.ThirdPerson);
                Debug.LogWarning("The passed in camera type does not exist. Defaulting to ThirdPerson movement mode.");
                break;
        }
    }
    
    #region Movement

    public void SwitchMovementMode(MovementModeType mode)
    {
        currentMovementMode = mode switch
        {
            MovementModeType.ThirdPerson => new ThirdPersonMovementMode(),
            MovementModeType.ConstructionTool => new ConstructionToolMovementMode(),
            _ => currentMovementMode,
        };
    }
    
    private void Move()
    {
        Vector2 input = inputReader.InputVector.normalized;
        Vector3 moveDir = currentMovementMode.Move(input);
        
        //TODO: Move other parts of the move function into movement modes as needed
        
        // only rotate player if pressing button
        const float rotationInputThreshold = 0.1f;
        if (moveDir.sqrMagnitude > rotationInputThreshold)
        {
            RotatePlayer(moveDir);
        }
        
        CalculateSpeed(moveDir);
        
        Vector3 finalMove = (moveDir * currentSpeed) + (velocity.y * Vector3.up);
        characterController.Move(finalMove *  Time.deltaTime);
        
        OnPlayerMove?.Invoke(currentSpeed);
    }
    private void RotatePlayer(Vector3 movement)
    {
        Quaternion newRotation = Quaternion.LookRotation(movement, Vector3.up);

        if (newRotation == previousRotation || rotationCoroutine != null) { return; }
        
        float angle = Quaternion.Angle(newRotation, previousRotation);
        
        // makes the transition longer for larger angle differences 
        const float shortTransition = 0.1f;
        const float longTransition = 0.2f;
        
        float duration = Mathf.Lerp(shortTransition, longTransition, angle / 180f);
        rotationCoroutine = StartCoroutine(SmoothRotation(newRotation, duration));
    }

    private void UpdateFacingDirection()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

        //Keeps angle positive 
        if (angle < 0f)
        {
            angle += 360f;
        }
        
        // splits player direction into 8 segments each equaling 45 degrees.
        // By getting the remainder of the players (current angle / 45) / 8 we get the index of which direction they are facing. We then convert the index to the direction enum.
        //This methode is more accurate than relying on the dot product of the players forward vector 
        int sector = Mathf.RoundToInt(angle / 45) % 8;
        CurrentFacingDirection = (Direction)sector;
    }

    private void CalculateSpeed(Vector3 movement)
    {
        const float transitionSpeed = 0.2f;
        // ? = if else
        float desiredSpeed = inputReader.InputVector != Vector2.zero ? walkSpeed : idleSpeed;

        if (inputReader.SprintPressed & movement != Vector3.zero)
        {
            desiredSpeed = runSpeed;
        }

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

    private IEnumerator SmoothRotation(Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float interpolationValue = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, interpolationValue);
            UpdateFacingDirection();
            yield return null;
        }
        
        transform.rotation = targetRotation;
        previousRotation = targetRotation;
        UpdateFacingDirection();
        rotationCoroutine = null;
    }
    
    private void Jump()
    {
        if (!inputReader.JumpWasPerformed || !characterController.isGrounded) { return; }

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
