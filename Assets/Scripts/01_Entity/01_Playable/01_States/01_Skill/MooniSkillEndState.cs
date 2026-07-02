using UnityEngine;

public class MooniSkillEndState : PlayerAnimationStateBase
{
    private const float SKILL_APPLY_TIME = 0.5f;

    private bool isApplied = false;

    public MooniSkillEndState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.StopMovementInput();
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleDashInput();

        if (isApplied == false && stateInfo.normalizedTime >= SKILL_APPLY_TIME)
        {
            isApplied = true;
            Owner.Combat.TakeHealByMaxHPRatio(0.3f);
        }

        if (stateInfo.normalizedTime < 0.75f) return;
    }

    public override void OnExit()
    {
        isApplied = false;
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, true);
    }
}
