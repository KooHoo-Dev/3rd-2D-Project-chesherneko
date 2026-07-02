using UnityEngine;
using Sirenix.OdinInspector;

public abstract class CombatEntity : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] protected EntityStat stat = null;

    [ShowInInspector, ReadOnly, TitleGroup("Runtime States")] protected int currentHP;
    [ShowInInspector, ReadOnly, TitleGroup("Runtime States")] protected float atkMult = 1f;

    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Runtime States")] public float HPRatio
    {
        get
        {
            if (stat == null) return 0f;
            return (float)currentHP / stat.MaxHP;
        }
    }
    [ShowInInspector, ReadOnly, TitleGroup("Runtime States")] public int ATK
    {
        get
        {
            if (stat == null) return 0;
            return Mathf.RoundToInt(stat.ATK * atkMult);
        }
    }

    public bool IsDead => currentHP == 0f;

    protected virtual void Awake()
    {
        ResetHP();
    }

    #region INHERITANCE METHODS
    public virtual bool CanTakeDamage(CombatEntity sender, CombatEvent @event) => true;

    public abstract void TakeDamage(int damage);
    public abstract void TakeHeal(int heal);
    #endregion

    #region PUBLIC METHODS
    public void ResetHP() => currentHP = stat.MaxHP;
    public void ResetATKMultiplier() => atkMult = 1f;

    public void SetATKMultiplier(float mult) => atkMult = mult;

    public bool ApplyDamage(int damage)
    {
        int damagedHP = currentHP - damage;
        currentHP = Mathf.Max(damagedHP, 0);

        return IsDead;
    }

    public void ApplyHeal(int healAmount)
    {
        int healedHP = currentHP + healAmount;
        currentHP = Mathf.Min(healedHP, stat.MaxHP);

        Debug.Log($"{name}의 체력 {healAmount}만큼 회복!");
    }
    #endregion
}
