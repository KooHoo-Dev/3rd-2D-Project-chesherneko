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

    protected override int PatternStartTrigger => Animator.StringToHash("SpecialAttack2");
    private readonly int patternEndTrigger = Animator.StringToHash("SpecialAttack2_End");

    protected override void OnDisable()
    {
        
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        yield return minionSpawner.InitCoroutine();

        float time = 0f;

        while (true)
        {
            //가정 1. 정상적으로 패턴을 종료했다면
            //가정 2. 제한 시간 내에 패턴을 끝내지 못 했다면
            if (minionSpawner.gameObject.activeSelf == false) break;
            if (time > maxPatternTime) break;

            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("2번 특수 패턴 종료!");
        yield return null;
    }
}
