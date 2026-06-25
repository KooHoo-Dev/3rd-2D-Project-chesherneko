using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_2 : BossPatternStateBase
{
    private WaitForSeconds waitForStartLag;
    private WaitForSeconds waitForEndLag;

    [Title("Settings")]
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnRange = 25f;
    [SerializeField] private float spawnInterval = 1f;

    [Space]
    [SerializeField] private float startLag = 2f;
    [SerializeField] private float endLag = 2f;

    [Title("References")]
    [SerializeField] private Animator prefab;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private Animator[] attackObjects;
    [ShowInInspector, ReadOnly] private Vector2[] spawnPoints;

    private void Awake()
    {
        waitForStartLag = new WaitForSeconds(startLag);
        waitForEndLag = new WaitForSeconds(endLag);

        InitAttackObjects();
        InitSpawnPoints();
    }

    protected override void OnEnable()
    {
        owner.ChangeState(typeof(BossNormalPattern_2));

        spawnPoints.Shuffle();
        StartCoroutine(PatternCoroutine());
    }

    protected override void OnDisable() { }

    private void InitAttackObjects()
    {
        attackObjects = new Animator[spawnCount];

        Animator[] objs = GetComponentsInChildren<Animator>(true);

        for (int i = 0; i < objs.Length; i++)
            attackObjects[i] = objs[i];

        for (int i = objs.Length; i < spawnCount; i++)
        {
            Animator atkObj = Instantiate(prefab, transform);
            atkObj.gameObject.SetActive(false);

            attackObjects[i] = atkObj;
        }
    }

    private void InitSpawnPoints()
    {
        int count = (int)(spawnRange / spawnInterval) + 1;

        spawnPoints = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPoint = new(spawnInterval * i, 0f);
            spawnPoints[i] = spawnPoint;
        }
    }

    private IEnumerator PatternCoroutine()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Animator obj = attackObjects[i];

            Vector2 spawnPoint = spawnPoints[i];
            obj.transform.localPosition = spawnPoint;

            obj.gameObject.SetActive(true);
        }

        yield return waitForStartLag;

        for (int i = 0; i < spawnCount; i++)
        {
            Animator anim = attackObjects[i];
            anim.SetTrigger(AnimationParam.HIT);
        }

        yield return waitForEndLag;

        owner.ChangeState(typeof(BossIdleState));
    }
}
