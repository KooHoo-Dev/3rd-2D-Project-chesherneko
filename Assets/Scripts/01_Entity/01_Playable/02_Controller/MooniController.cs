using UnityEngine;

public class MooniController : PlayerController
{
    protected override PlayerAnimationStateBase CreateSkillState()
        => new MooniSkillState(this);
}
