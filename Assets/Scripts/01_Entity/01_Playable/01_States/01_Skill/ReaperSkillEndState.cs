using UnityEngine;

public class ReaperSkillEndState : PlayerAnimationStateBase
{
    public ReaperSkillEndState(PlayerController owner) : base(owner) { }

    public override void OnExit()
    {
        Animator.SetBool(AnimationParam.IS_USING_SKILL, false);
    }
}
