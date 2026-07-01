using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCombat : CombatEntity
{
    public static PlayerCombat LocalPlayer { get; private set; }

    [Title("Components")]
    [SerializeField] private GameObject hurtBox;

    [Title("Settings")]
    [SerializeField] private float dashInvincibleDuration = 0.05f;
    [SerializeField] private float qteGaugeGainMult = 4f;

    [Title("Runtime State")]
    [ShowInInspector, ReadOnly] private bool isDashInvincible = false;
    [ShowInInspector, ReadOnly] private float dashTime = 0f;

    public int CurrentHP => currentHP;

    private void OnEnable()
    {
        LocalPlayer = this;
    }

    private void Update()
    {
        if (isDashInvincible == false) return;

        dashTime += Time.deltaTime;

        if (dashTime > dashInvincibleDuration)
        {
            dashTime = 0f;
            isDashInvincible = false;
        }
    }

    public override bool CanTakeDamage(CombatEntity sender, CombatEvent @event)
    {
        if (isDashInvincible)
        {
            PlayerSwitchManager.Instance.GainQTEGauge(qteGaugeGainMult);
            return false;
        }

        return true;
    }

    public override void TakeDamage(int damage)
    {
        ApplyDamage(damage);
        Debug.Log($"{damage}만큼 데미지 받음!");
    }

    public override void TakeHeal(int heal)
    {
        ApplyHeal(heal);
    }

    public void SyncPrevPlayerState(PlayerCombatSyncData syncData)
    {
        currentHP = syncData.CurrentHP;
    }

    public void OnDashStarted() => isDashInvincible = true;

    public void OnDashFinished()
    {
        dashTime = 0f;
        isDashInvincible = false;
    }

    public void SetHurtBoxActive(bool isActive)
        => hurtBox.SetActive(isActive);
}
