using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    
    private static readonly int SpeedID = Animator.StringToHash("Speed");
    private static readonly int MotionSpeedID = Animator.StringToHash("MotionSpeed");
    private static readonly int JumpID = Animator.StringToHash("Jump");
    private static readonly int GroundedID = Animator.StringToHash("Grounded");
    private static readonly int FreeFallID = Animator.StringToHash("FreeFall");
    private static readonly int OnCleanupToolEnteredID = Animator.StringToHash("OnCleanupToolEntered");
    private static readonly int CleanupToolExitedID = Animator.StringToHash("OnCleanupToolExited");
    
    private float lastBlendSpeed;

    private void Awake()
    {
        playerController.OnPlayerMove += SetMovementBlend;
        playerController.OnPlayerLand += PlayLandAnimation;
        playerController.OnPlayerJump += PlayJumpAnimation;
        playerController.OnPlayerFall += PlayFreeFallAnimation;
        
        CleanupTool.OnToggleMount += HandleToggleMount;
    }

    private void OnDestroy()
    {
        playerController.OnPlayerMove -= SetMovementBlend;
        playerController.OnPlayerLand -= PlayLandAnimation;
        playerController.OnPlayerJump -= PlayJumpAnimation;
        playerController.OnPlayerFall -= PlayFreeFallAnimation;
        
        CleanupTool.OnToggleMount -= HandleToggleMount;
    }

    private void Start()
    {
        animator.SetFloat(MotionSpeedID, 1);
    }

    private void SetMovementBlend(float speed)
    {
        if (!Mathf.Approximately(speed, lastBlendSpeed))
        {
            lastBlendSpeed = speed;
            animator.SetFloat(SpeedID, speed);
        }
    }

    private void PlayJumpAnimation()
    {
        animator.SetBool(GroundedID, false);
        animator.SetBool(JumpID, true);
    }

    private void PlayLandAnimation()
    {
        animator.SetBool(JumpID, false);
        animator.SetBool(FreeFallID, false);
        animator.SetBool(GroundedID, true);
    }

    private void PlayFreeFallAnimation()
    {
        animator.SetBool(GroundedID, false);
        animator.SetBool(FreeFallID, true);
    }
    
    // Clean up tool

    private void HandleToggleMount(bool toggle)
    {
        animator.SetTrigger(toggle ? OnCleanupToolEnteredID : CleanupToolExitedID);
    }
}
