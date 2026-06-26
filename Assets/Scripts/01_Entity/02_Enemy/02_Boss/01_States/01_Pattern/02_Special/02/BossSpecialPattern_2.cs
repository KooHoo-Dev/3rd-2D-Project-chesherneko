using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossSpecialPattern_2 : BossPatternStateBase
{
    //잡몹 생성 패턴?
    //맵의 가운데에 구조물 생성 (플레이어의 이동에 방해를 주진 않음)
    //그와 동시에 랜덤한 타입의 Minion을 n마리 스폰
    //일정 시간 안에 구조물과 생성된 Minion을 전부 처리하지 못했을 경우
    //일정량의 데미지 발생
    //구조물은 일정 시간 간격으로 Minion을 스폰
    //모든 구조물과 Minion을 처리했을 경우, 보스에게 데미지 발생
    [Title("Components")]
    [SerializeField] private BossMinionSpawner minionSpawner;

    [Title("Settings")]
    [SerializeField] private float maxPatternTime;

    protected override void OnEnable()
    {
        StartCoroutine(PatternCoroutine());
    }

    protected override void OnDisable()
    {
        
    }

    private IEnumerator PatternCoroutine()
    {
        //BossMinionSpawner의 생성 시간까지 대기
        //이후 로직은 BossMinionSpawner에서 처리
        //PatternState는 종료 조건만 업데이트
        yield return minionSpawner.SpawnCoroutine();
        minionSpawner.Execute();

        //패턴 시간 업데이트
        float time = 0f;
        while (true)
        {
            if (minionSpawner.RemainMinionCount == 0) break;
            if (time > maxPatternTime) break;

            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("2번 특수 패턴 종료!");
    }
}
