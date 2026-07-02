using UnityEngine;

public class PlayerDeadState : PlayerAnimationStateBase
{
    public PlayerDeadState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.normalizedTime < 1f) return;

        //체력을 Reset하고, 저장된 가장 최근의 위치로 이동
        //체력을 리셋하는건 별거 아니고...
        //눈에 보이지 않는 SavePoint를 만들고, Trigger를 통해
        //거기에 닿으면 현재 위치가 저장되는 식으로 해야할듯?
    }
}
