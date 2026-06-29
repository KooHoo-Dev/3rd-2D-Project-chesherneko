using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossSpecialPattern_2 : BossPatternStateBase
{
    [Title("Components")]
    [SerializeField] private BossMinionSpawner minionSpawner;
    [SerializeField] private GameObject patternSuccessObj;
    [SerializeField] private GameObject patternFailObj;

    [Title("Settings")]
    [SerializeField] private float maxPatternTime;

    [SerializeField, TitleGroup("Lag")] private float patternSuccessLag;
    [SerializeField, TitleGroup("Lag")] private float patternFailLag;

    private WaitForSeconds waitForPatternSuccessLag;
    private WaitForSeconds waitForPatternFailLag;

    protected override int PatternStartTrigger => Animator.StringToHash("SpecialAttack2");
    private readonly int patternEndTrigger = Animator.StringToHash("SpecialAttack2_End");

    protected override void OnDisable()
    {
        patternSuccessObj.SetActive(false);
        patternFailObj.SetActive(false);
    }

    protected override void InitYieldInstructions()
    {
        base.InitYieldInstructions();
        waitForPatternSuccessLag = new(patternSuccessLag);
        waitForPatternFailLag = new(patternFailLag);
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        yield return minionSpawner.InitCoroutine();

        float time = 0f;
        bool isPatternSuccessed = true;

        while (true)
        {
            if (minionSpawner.IsDead) break;
            if (time > maxPatternTime)
            {
                isPatternSuccessed = false;
                yield return minionSpawner.KillAllMinions();
                break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitUntil(() => minionSpawner.gameObject.activeSelf == false);
        yield return new WaitForSeconds(0.5f);

        Animator.SetTrigger(patternEndTrigger);
        yield return waitForEndLag;

        if (isPatternSuccessed)
        {
            patternSuccessObj.SetActive(true);
            yield return waitForPatternSuccessLag;
            patternSuccessObj.SetActive(false);
        }
        else
        {
            patternFailObj.SetActive(true);
            yield return waitForPatternFailLag;
            patternFailObj.SetActive(false);
        }

        Debug.Log("2번 특수 패턴 종료!");
    }
}
