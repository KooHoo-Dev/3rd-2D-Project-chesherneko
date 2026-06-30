using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private CombatEntity owner;

    [field: TitleGroup("Settings")]
    [field: SerializeField] public float DamageMult { get; private set; }
    [field: SerializeField] private LayerMask HitLayerMask { get; set; }

    private readonly HashSet<Collider2D> checkList = new();

    private void Awake()
    {
        owner = GetComponentInParent<CombatEntity>();

        if (owner == null)
        {
            Debug.LogError($"{name} : 부모 계층에서 CombatEntity를 찾지 못했습니다.");
            Destroy(gameObject);
        }
    }

    protected virtual void OnDisable()
    {
        checkList.Clear();
        gameObject.SetActive(false);
    }

    protected virtual bool TryHit(Collider2D collision)
    {
        if (HitLayerMask.Contains(collision) == false) return false;
        if (checkList.Contains(collision)) return false;

        if (CombatManager.Instance
            .TryGetHurtBox(collision, out HurtBox hurtBox) == false) return false;

        checkList.Add(collision);

        CombatEntity sender = owner;
        CombatEntity receiver = hurtBox.Owner;

        int damage = Mathf.FloorToInt(sender.Stat.GetFinalATK() * DamageMult);

        CombatEvent @event;
        @event.EventType = CombatEventType.Damage;
        @event.Amount = damage;
        @event.Position = Vector2.zero;

        CombatManager.Instance.To(sender, receiver, @event);

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
        => TryHit(collision);

    private void OnTriggerStay2D(Collider2D collision)
        => TryHit(collision);
}
