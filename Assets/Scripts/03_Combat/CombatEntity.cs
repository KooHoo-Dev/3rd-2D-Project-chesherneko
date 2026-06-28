using UnityEngine;
using Sirenix.OdinInspector;

public abstract class CombatEntity : MonoBehaviour
{
    [field: HideLabel, HideReferenceObjectPicker]
    [field: SerializeField] public CombatEntityStat Stat { get; private set; }

    protected virtual void Awake()
    {
        Stat.ResetHP();
    }
    
    #region PUBLIC METHOD
    public virtual bool CanTakeDamage(CombatEntity sender, CombatEvent @event) => true;

    public abstract void TakeDamage(int damage);
    public abstract void TakeHeal(int heal);

    public void ResetHP() => Stat.ResetHP();
    public void SetATKMultiplier(float mult) => Stat.SetATKMultiplier(mult);
    public void ResetATKMultiplier() => Stat.ResetATKMultiplier();
    #endregion
}
