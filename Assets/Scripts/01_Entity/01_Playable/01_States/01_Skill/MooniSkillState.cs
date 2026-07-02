using UnityEngine;

public class MooniSkillState : PlayerAnimationStateBase
{
    private const float SKILL_APPLY_TIME = 0.5f;

    private bool isApplied = false;

    public MooniSkillState(PlayerController owner) : base(owner) { }

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

        Animator.SetBool(AnimationParam.IS_USING_SKILL, false);
    }

    public override void OnExit()
    {
        isApplied = false;
        Animator.SetBool(AnimationParam.IS_USING_SKILL, false);
    }
}
