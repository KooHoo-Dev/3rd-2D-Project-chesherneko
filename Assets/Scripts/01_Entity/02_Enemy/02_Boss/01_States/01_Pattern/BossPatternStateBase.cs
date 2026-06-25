using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BossPatternStateBase : BossStateBase
{
    protected WaitForSeconds waitForStartLag;
    protected WaitForSeconds waitForEndLag;

    [SerializeField, TitleGroup("Lag")] protected float startLag;
    [SerializeField, TitleGroup("Lag")] protected float endLag;

    protected virtual void Awake()
    {
        InitYieldInstructions();
    }

    [PropertySpace, Button(ButtonHeight = 40)]
    protected virtual void InitYieldInstructions()
    {
        waitForStartLag = new WaitForSeconds(startLag);
        waitForEndLag = new WaitForSeconds(endLag);
    }
}
