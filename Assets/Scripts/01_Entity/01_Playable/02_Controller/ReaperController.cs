using UnityEngine;

public class ReaperController : PlayerController
{
    protected override PlayerAnimationStateBase CreateSkillState()
        => new ReaperSkillState(this);

    protected override void InitStateMap()
    {
        base.InitStateMap();
        stateMap.Add(Animator.StringToHash(AnimationTag.ANIM_TAG_SKILL_END), new ReaperSkillEndState(this));
    }
}
