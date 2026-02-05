using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private static readonly int SpeedID = Animator.StringToHash("Speed");
    private static readonly int MotionSpeedID = Animator.StringToHash("MotionSpeed");
    private Animator animator;
    private PlayerController playerController;

    private float lastBlendSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        if (playerController)
        {
            playerController.OnPlayerMove += SetMovementBlend;
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

    private void OnFootstep()
    {
        
    }
}
