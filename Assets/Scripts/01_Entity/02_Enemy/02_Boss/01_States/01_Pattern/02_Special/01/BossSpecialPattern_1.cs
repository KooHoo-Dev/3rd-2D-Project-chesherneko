using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

using Random = UnityEngine.Random;

public class BossSpecialPattern_1 : BossPatternStateBase
{
    [Title("Settings")]
    [SerializeField] private Vector2 minSpawnPoint;
    [SerializeField] private Vector2 maxSpawnPoint;
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnInterval = 0.5f;

    [Title("References")]
    [SerializeField] private BossEye prefab;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private BossEye[] eyes;

    protected override void Awake()
    {
        base.Awake();
        InitLasers();
    }

    protected override void OnEnable()
    {
        StartCoroutine(PatternCoroutine());
        //지정한 범위 내에서 일정 간격으로 랜덤한 위치에 '눈'을 소환
        //눈은 계속해서 플레이어를 바라보다가 생성된지 일정 시간이 지났을 때
        //바라보는 것을 멈추고 아주 약간의 시간이 지난 후 플레이어를 향해 빔을 발사
        //빔은 약간의 시간이 지난 후 사라지며 빔이 사라지고 난 뒤 눈이 서서히 사라짐
        //소환한 모든 '눈'이 빔을 쏘고 사라지고 나면 패턴 종료
        //해당 빔으로 보스 몹의 구체를 타격 시, 대량의 피해 발생
    }

    protected override void OnDisable()
    {
        
    }

    private void InitLasers()
    {
        eyes = new BossEye[spawnCount];

        BossEye[] preLoadeds = GetComponentsInChildren<BossEye>(true);

        for (int i = 0; i < preLoadeds.Length; i++)
            eyes[i] = preLoadeds[i];

        for (int i = preLoadeds.Length; i < spawnCount; i++)
        {
            BossEye eye = Instantiate(prefab, transform);
            eyes[i] = eye;

            eye.gameObject.SetActive(false);
        }
    }

    private IEnumerator PatternCoroutine()
    {
        //차례대로 눈을 하나씩 랜덤한 위치에 생성
        for (int i = 0; i < spawnCount; i++)
        {
            BossEye eye = eyes[i];

            float randX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
            float randY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);
            Vector2 spawnPos = new(randX, randY);

            eye.Spawn(spawnPos);
            eye.ExecuteLogic();

            yield return new WaitForSeconds(spawnInterval);
        }

        //모든 눈이 비활성화 상태일 때 까지 대기
        while (true)
        {
            if (HasActiveEye() == false) break;
            yield return null;
        }

        Debug.Log("패턴 종료!");
    }

    private bool HasActiveEye()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            BossEye eye = eyes[i];
            if (eye.isActiveAndEnabled) return true;
        }

        return false;
    }
}
