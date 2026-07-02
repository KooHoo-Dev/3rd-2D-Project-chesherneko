using UnityEngine;

public class ReaperSkillState : PlayerAnimationStateBase
{
    private readonly float skillKeepDuration = 2f;
    private float skillKeepTime = 0f;

    private bool isTriggered = false;

    public ReaperSkillState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.StopMovementInput();
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.normalizedTime < 0.95f) return;
        if (isTriggered) return;

        skillKeepTime += Time.deltaTime;

        Owner.HandleMoveInput();

        if (Owner.WasPressedSkillKey() || skillKeepTime > skillKeepDuration)
        {
            Owner.StopMovementInput();

            isTriggered = true;
            Animator.SetTrigger(AnimationParam.SKILL_END);
        }
    }
    
    public override void OnExit()
    {
        skillKeepTime = 0f;
        isTriggered = false;
    }
}
