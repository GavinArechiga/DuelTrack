using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private static readonly int SpeedID = Animator.StringToHash("Speed");
    private static readonly int MotionSpeedID = Animator.StringToHash("MotionSpeed");
    private static readonly int JumpID = Animator.StringToHash("Jump");
    private static readonly int Grounded = Animator.StringToHash("Grounded");


    private PlayerController playerController;

    private float lastBlendSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        if (playerController)
        {
            playerController.OnPlayerMove += SetMovementBlend;
            playerController.OnPlayerLand += PlayLandAnimation;
            playerController.OnPlayerJump += PlayJumpAnimation;
        }
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
        animator.SetBool(Grounded, false);
        animator.SetBool(JumpID, true);
    }

    private void PlayLandAnimation()
    {
        animator.SetBool(JumpID, false);
        animator.SetBool(Grounded, true);
    }
}
