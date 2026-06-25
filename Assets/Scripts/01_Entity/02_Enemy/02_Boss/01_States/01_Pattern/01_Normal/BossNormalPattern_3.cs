using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_3 : BossPatternStateBase
{
    private WaitForSeconds waitForStartLag;
    private WaitForSeconds waitForActiveLag;
    private WaitForSeconds waitForEndLag;

    [Title("Settings")]
    [SerializeField] private float startLag = 0.1f;
    [SerializeField] private float activeLag = 0.2f;
    [SerializeField] private float endLag = 2f;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private GameObject[] sparks;

    private void Awake()
    {
        InitYieldInstructions();
        InitSparkObjects();
    }

    protected override void OnEnable()
    {
        owner.ChangeState(typeof(BossNormalPattern_3));

        sparks.Shuffle();
        StartCoroutine(PatternCoroutine());
    }

    protected override void OnDisable() { }

    [PropertySpace, Button(ButtonHeight = 40)]
    private void InitYieldInstructions()
    {
        waitForStartLag = new WaitForSeconds(startLag);
        waitForActiveLag = new WaitForSeconds(activeLag);
        waitForEndLag = new WaitForSeconds(endLag);
    }

    private void InitSparkObjects()
    {
        int count = transform.childCount;

        sparks = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            sparks[i] = child.gameObject;
        }
    }

    private IEnumerator PatternCoroutine()
    {
        for (int i = 0; i < sparks.Length; i++)
        {
            yield return waitForStartLag;
            sparks[i].SetActive(true);

            yield return waitForActiveLag;
            sparks[i].SetActive(false);
        }

        yield return waitForEndLag;
        owner.ChangeState(typeof(BossIdleState));
    }
}
