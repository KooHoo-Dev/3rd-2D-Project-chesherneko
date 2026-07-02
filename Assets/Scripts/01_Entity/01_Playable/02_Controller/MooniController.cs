using UnityEngine;

public class MooniController : PlayerController
{
    protected override PlayerAnimationStateBase CreateSkillState() => null;
    protected override PlayerAnimationStateBase CreateSkillEndState()
        => new MooniSkillEndState(this);
}
