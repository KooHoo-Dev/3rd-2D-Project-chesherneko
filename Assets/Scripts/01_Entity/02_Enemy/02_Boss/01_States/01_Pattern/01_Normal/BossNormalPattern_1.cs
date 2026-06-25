using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_1 : BossPatternStateBase
{
    private WaitForSeconds waitForStartLag;
    private WaitForSeconds waitForEndLag;

    [Title("Components")]
    [SerializeField] private Animator animator;

    [Title("Settings")]
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private Vector3 followOffset;

    [Space]
    [SerializeField] private float startLag = 0.25f;
    [SerializeField] private float endLag = 1.5f;

    private void Awake()
    {
        waitForStartLag = new WaitForSeconds(startLag);
        waitForEndLag = new WaitForSeconds(endLag);
    }

    protected override void OnEnable()
    {
        owner.ChangeState(typeof(BossNormalPattern_1));
        StartCoroutine(ExecuteCoroutine());
    }

    protected override void OnDisable() { }

    private IEnumerator ExecuteCoroutine()
    {
        float time = 0f;
        while (true)
        {
            if (time > followDuration) break;

            time += Time.deltaTime;

            Transform playerTrans = PlayerCombat.LocalPlayer.transform;
            Vector3 playerPos = playerTrans.position;

            transform.position = playerPos + followOffset;

            yield return null;
        }

        yield return waitForStartLag;
        animator.SetTrigger(AnimationParam.HIT);

        yield return waitForEndLag;
        owner.ChangeState(typeof(BossIdleState));
    }
}
