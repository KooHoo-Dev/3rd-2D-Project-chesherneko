using UnityEngine;

public class PlayerAttackEndState : PlayerAnimationStateBase
{
    public PlayerAttackEndState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.IsLockMoveDirection = true;

        Animator.SetBool(AnimationParam.IS_ATTACKING, true);
        Animator.SetBool(AnimationParam.IS_ATTACK, false);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleAirMoveInput();
        Owner.HandleDashInput();
    }

    public override void OnExit()
    {
        Owner.IsLockMoveDirection = false;
        Animator.SetBool(AnimationParam.IS_ATTACKING, false);
    }
}
