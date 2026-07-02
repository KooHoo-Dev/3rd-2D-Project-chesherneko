using UnityEngine;

public class PlayerAttackState : PlayerAnimationStateBase
{
    public PlayerAttackState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.IsLockMoveDirection = true;

        Animator.SetBool(AnimationParam.IS_ATTACKING, true);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleAirMoveInput();
        Owner.HandleDashInput();
        Owner.HandleSkillInput();

        //직전 공격 입력을 제거
        if (stateInfo.normalizedTime < INPUT_RESET_TIME)
        {
            Animator.SetBool(AnimationParam.IS_ATTACK, false);
            return;
        }

        if (stateInfo.normalizedTime >= COMBO_INPUT_END_TIME)
            return;

        Owner.HandleComboAttackInput();
    }

    public override void OnExit()
    {
        Owner.IsLockMoveDirection = false;
        Animator.SetBool(AnimationParam.IS_ATTACKING, false);
    }
}