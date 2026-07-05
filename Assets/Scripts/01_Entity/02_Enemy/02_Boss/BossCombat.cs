using UnityEngine;

public class BossCombat : EnemyCombat
{
    [SerializeField] private BossController controller;

    protected override void OnDead()
    {
        controller.ChangeState(typeof(BossDeadState));
    }
}
