using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
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

    public InputManager.Direction CurrentFacingDirection { get; private set; } = InputManager.Direction.Forward;
    [SerializeField] private Transform playerCam;
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
    private InputManager inputManager;
    
    //TODO: move to construction tool
    [SerializeField] private GameObject testGridPrefab;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        velocity = characterController.velocity;
        previousRotation = transform.rotation;

        //TODO: Move to construction tool
        inputManager.PrimaryToolAction.performed += _ => PlaceObject();
        GridSystem.Instance.PlayerController = this;
    }

    //TODO: move to construction tool
    private void PlaceObject()
    {
        GridSystem.Instance.PlaceObject(testGridPrefab);
    }

    private void Update()
    {
        Move();
        Fall();
        Jump();
        
        //TODO: Move this to construction costume script once that is implemented
        GridSystem.Instance.UpdateCellPosition(transform.position);
    }
    
    #region Movement
    private void Move()
    {
        Vector2 input = inputManager.InputVector.normalized;

        // removes the y so the camera tilt does not affect movement. Also makes sure both forward and right are perpendicular to avoid having any skew. 
        Vector3 camForward = Vector3.ProjectOnPlane(playerCam.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.Cross(Vector3.up, camForward);
        
        // creates camera relative movement by multiplying the cameras right and forward axis by the input 
        Vector3 moveDir = camRight * input.x + camForward * input.y;

        // only rotate player if pressing button
        const float rotationInputThreshold = 0.1f;
        if (moveDir.sqrMagnitude > rotationInputThreshold)
        {
            RotatePlayer(moveDir);
            UpdateFacingDirection();
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

        // Vector3.Dot() returns the dot product of two vectors which is a float you get from multiplying the two vectors
        // if the dot product is 1 then both vectors face the same direction. if the dot product is -1 then they face opposite directions.
        // if the dot product is 0 then they are at a 90-degree angle
        // There are also in between values but for this function we only care if it's close to 1 so we know what direction the player is facing 
        if (Vector3.Dot(forward, Vector3.forward) > 0.5f)
        {
            CurrentFacingDirection = InputManager.Direction.Forward;
        }
        else if (Vector3.Dot(forward, Vector3.back) > 0.5f)
        {
            CurrentFacingDirection = InputManager.Direction.Backward;
        }
        else if (Vector3.Dot(forward, Vector3.right) > 0.5f)
        {
            CurrentFacingDirection  = InputManager.Direction.Right;
        }
        else if (Vector3.Dot(forward, Vector3.left) > 0.5f)
        {
            CurrentFacingDirection = InputManager.Direction.Left;
        }
    }

    private void CalculateSpeed(Vector3 movement)
    {
        const float transitionSpeed = 0.2f;
        // ? = if else
        float desiredSpeed = inputManager.InputVector != Vector2.zero ? walkSpeed : idleSpeed;

        if (inputManager.SprintAction.IsPressed() & movement != Vector3.zero)
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
            yield return null;
        }
        
        transform.rotation = targetRotation;
        previousRotation = targetRotation;
        rotationCoroutine = null;
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
