using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CombatEntityStat
{
    [field: SerializeField, TitleGroup("Stat")] public int MaxHP { get; private set; }
    [field: SerializeField, TitleGroup("Stat")] public int ATK { get; private set; }

    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Stat")] public int HP { get; private set; }
    [ShowInInspector, ReadOnly, TitleGroup("Stat")] public bool IsDead => HP <= 0f;

    [Title("Stat Multiplier")]
    [ShowInInspector, ReadOnly] private float atkMult = 1f;

    public bool ApplyDamage(int damage)
    {
        HP = Mathf.Max(HP - damage, 0);
        return IsDead;
    }

    public void ApplyHeal(int healAmount)
    {
        HP = Mathf.Min(HP + healAmount, MaxHP);
    }

    public void ResetHP() => HP = MaxHP;
    public void ResetATKMultiplier() => atkMult = 1f;

    public float GetHPRatio() => (float)HP / MaxHP;
    public int GetFinalATK() => Mathf.RoundToInt(ATK * atkMult);

    public void SetATKMultiplier(float mult) => atkMult = mult;
}
