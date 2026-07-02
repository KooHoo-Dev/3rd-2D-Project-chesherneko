using UnityEngine;

public class ReaperController : PlayerController
{
    protected override PlayerAnimationStateBase CreateSkillState()
        => new ReaperSkillState(this);

    protected override PlayerAnimationStateBase CreateSkillEndState()
        => new ReaperSkillEndState(this);
}
