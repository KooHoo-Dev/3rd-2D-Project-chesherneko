using UnityEngine;

public class ReaperSkillState : PlayerAnimationStateBase
{
    private float skillKeepDuration = 2f;
    private float skillKeepTime = 0f;

    public ReaperSkillState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        //Apply Skill
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleMoveInput();

        if (stateInfo.normalizedTime < 1f) return;

        skillKeepTime += Time.deltaTime;

        if (skillKeepTime > skillKeepDuration)
        {
            Animator.SetBool(AnimationParam.IS_USE_SKILL, false);
            return;
        }

        //만약 스킬 키를 재 입력 받았다면
        //Animator.SetTrigger(AnimationParam.SKILL_END);
    }

    public override void OnExit()
    {
        skillKeepTime = 0f;
        Animator.SetBool(AnimationParam.IS_USE_SKILL, false);
    }
}
