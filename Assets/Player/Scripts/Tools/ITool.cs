using UnityEngine;

public interface ITool
{
    void Initialize(PlayerController playerController);
    void Enter();
    void Exit();

}
