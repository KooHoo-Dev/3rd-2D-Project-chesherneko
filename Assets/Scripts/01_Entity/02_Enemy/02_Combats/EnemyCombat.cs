using Sirenix.OdinInspector;
using UnityEngine;

public enum HitReactionType
{
    Animation,
    Shader,
    Both
}

public class EnemyCombat : CombatEntity
{
    [Title("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private HitEffect hitEffect;

    [Title("Settings")]
    [SerializeField] private HitReactionType hitReactionType;

    public override void TakeDamage(int damage)
    {
        bool isDead = Stat.ApplyDamage(damage);

        if (isDead)
        {
            animator.SetTrigger(AnimationParam.DEAD);
            return;
        }

        switch (hitReactionType)
        {
            case HitReactionType.Animation:
                animator.SetTrigger(AnimationParam.HIT);
                break;
            case HitReactionType.Shader:
                hitEffect.Play();
                break;
            case HitReactionType.Both:
                animator.SetTrigger(AnimationParam.HIT);
                hitEffect.Play();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public override void TakeHeal(int heal)
    {
        
    }
}
