using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

public enum MinionType
{
    Warrior = 0,
    Archer,
    Spearman,
    Bomber
}

public class BossMinionSpawner : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;

    [Title("Settings")]
    [SerializeField] private int initialSpawnCount;

    [Space]
    [SerializeField] private float fadeInDuration;

    [Title("References")]
    [SerializeField] EnemyController[] prefabs;
    
    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] public int RemainMinionCount { get; private set; } = 0;

    private MinionType[] minionTypes;
    private readonly Dictionary<MinionType, Queue<EnemyController>> minionQueues = new();

    private void Awake()
    {
        InitQueues();
    }

    private void InitQueues()
    {
        minionTypes = (MinionType[])Enum.GetValues(typeof(MinionType));

        for (int i = 0; i < minionTypes.Length; i++)
            minionQueues[minionTypes[i]] = new Queue<EnemyController>();

        EnemyController[] preLoaded = GetComponentsInChildren<EnemyController>(true);

        for (int i = 0; i < preLoaded.Length; i++)
        {
            EnemyController minion = preLoaded[i];

            MinionType type = minion.Type;
            minionQueues[type].Enqueue(minion);
        }
    }

    private void SpawnInitialRandomMinions()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            int rand = Random.Range(0, minionTypes.Length);
            MinionType type = minionTypes[rand];

            EnemyController minion = GetMinion(type);
        }
    }

    private EnemyController GetMinion(MinionType type)
    {
        Queue<EnemyController> queue = minionQueues[type];

        if (queue.Count > 0) 
            return queue.Dequeue();

        int idx = (int)type;
        EnemyController minion = Instantiate(prefabs[idx], transform);
        
        return minion;
    }

    public IEnumerator SpawnCoroutine()
    {
        Tween t1 = renderer.FadeIn(fadeInDuration);
        yield return t1.WaitForCompletion();
    }

    public void Execute()
    {
        SpawnInitialRandomMinions();
        //시작 시점에 일단 잡몹을 n마리 랜덤한 위치에 생성
        //이후 bool트리거를 on으로 하여 update에서 일정 시간마다 잡몹 생성 시작
    }
}
