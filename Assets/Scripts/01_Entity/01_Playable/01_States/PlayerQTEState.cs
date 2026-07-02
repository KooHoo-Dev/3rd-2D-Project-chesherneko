using UnityEngine;

public class PlayerQTEState : PlayerAnimationStateBase
{
    public PlayerQTEState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.StopMovementInput();
    }

    public override void OnExit()
    {
        Animator.SetBool(AnimationParam.IS_USING_QTE, false);

        if (Animator.GetBool(AnimationParam.IS_GROUNDED))
            Animator.ResetTrigger(AnimationParam.FALL);
    }
}
