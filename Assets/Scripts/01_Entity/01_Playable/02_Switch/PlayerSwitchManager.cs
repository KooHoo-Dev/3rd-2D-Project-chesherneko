using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitchManager : MonoBehaviour
{
    [System.Serializable]
    private class PlayerSlot
    {
        public GameObject Root;
        public PlayerController Controller;
        public PlayerMove Move;
        public PlayerCombat Combat;
    }

    [SerializeField] private new CinemachineCamera camera;

    [SerializeField] private PlayerSlot currentPlayer;
    [SerializeField] private PlayerSlot[] sparePlayers;

    [SerializeField] private float switchJumpHeight = 3f;
    [SerializeField] private float switchJumpDuration = 0.5f;
    [SerializeField] private float screenOutDistance = 10f;

    private bool isSwitching = false;

    private void Update()
    {
        if (isSwitching) return;

        if (Key.Digit1.WasPressed())
            StartCoroutine(SwitchCoroutine(0));
    }

    private IEnumerator SwitchCoroutine(int nextIdx)
    {
        isSwitching = true;

        PlayerSlot curr = currentPlayer;
        PlayerSlot next = sparePlayers[nextIdx];

        curr.Controller.LockControl();
        curr.Combat.SetHurtBoxActive(false);

        Transform currTrans = curr.Root.transform;

        Vector2 switchPoint = currTrans.position;
        float dir = currTrans.localScale.x > 0f ? 1f : -1f;

        Vector2 goalPoint = switchPoint + dir * screenOutDistance * Vector2.left;

        camera.Follow = null;

        next.Root.transform.position = goalPoint;
        next.Root.transform.localScale = curr.Root.transform.localScale;
        next.Root.SetActive(true);
        next.Controller.LockControl();
        next.Combat.SetHurtBoxActive(false);

        bool switchOutDone = false;
        bool switchInDone = false;

        StartCoroutine(SwitchOutCoroutine(curr, switchPoint, goalPoint, () => switchOutDone = true));
        StartCoroutine(SwitchInCoroutine(next, goalPoint, switchPoint, () => switchInDone = true));

        yield return new WaitUntil(() => switchOutDone && switchInDone);

        currentPlayer.Root.SetActive(false);

        currentPlayer = next;
        sparePlayers[nextIdx] = curr;

        isSwitching = false;
    }

    private IEnumerator SwitchOutCoroutine(PlayerSlot curr,
        Vector2 start, Vector2 end, Action onDone)
    {
        yield return WaitUntilSwitchOutReady(curr.Controller);

        yield return curr.Move.SwitchJump(
            start, end,
            switchJumpHeight, switchJumpDuration
        );

        onDone?.Invoke();
    }

    private IEnumerator SwitchInCoroutine(PlayerSlot next,
        Vector2 start, Vector2 end, Action onDone)
    {
        yield return next.Move.SwitchJump(
            start, end,
            switchJumpHeight, switchJumpDuration
        );

        onDone?.Invoke();

        camera.Follow = next.Root.transform;

        next.Controller.ReleaseControl();
        next.Combat.SetHurtBoxActive(true);
    }

    private IEnumerator WaitUntilSwitchOutReady(PlayerController player)
    {
        player.LockControl();

        while (true)
        {
            if (player.IsBusyForSwitch() == false) break;
            if (player.IsCurrentAnimationFinished()) break;

            yield return null;
        }
    }
}
