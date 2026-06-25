using UnityEngine;

public abstract class BossStateBase : MonoBehaviour
{
    protected BossController owner;

    public void Initialize(BossController owner)
        => this.owner = owner;

    protected abstract void OnEnable();
    protected abstract void OnDisable();
}
