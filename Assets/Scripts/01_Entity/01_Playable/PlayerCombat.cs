using UnityEngine;

public class PlayerCombat : CombatEntity
{
    public static PlayerCombat LocalPlayer { get; private set; }

    public override void TakeDamage(int damage)
    {
        Debug.Log($"{damage}만큼 데미지 받음!");
    }

    public override void TakeHeal(int heal)
    {

    }
}
