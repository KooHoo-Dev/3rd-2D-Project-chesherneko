using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    #region FIELDS
    private static WaitForSeconds waitForStartLag;
    private static WaitForSeconds waitForEndLag;

    [Title("Components")]
    [SerializeField] private CircleCollider2D muzzleRange;

    [Title("Settings")]
    [SerializeField] private float beamOffset = 0.25f;

    [Title("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform beam;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private bool isFollowTarget = false;
    [ShowInInspector, ReadOnly] private Transform target;
    [ShowInInspector, ReadOnly] private Vector2 muzzleDirection;
    #endregion

    #region UNITY METHODS
    private void Update()
    {
        if (isFollowTarget == false) return;

        muzzleDirection = FollowTarget();
    }
    #endregion

    #region PRIVATE METHODS
    private Vector2 FollowTarget()
    {
        Vector2 center = muzzleRange.bounds.center;
        Vector2 targetPos = target.position;

        Vector2 dir = targetPos - center;

        if (dir.sqrMagnitude <= 0.0001f)
            return Vector2.zero;

        dir = dir.normalized;

        float radius = muzzleRange.radius * Mathf.Max(
            muzzleRange.transform.lossyScale.x,
            muzzleRange.transform.lossyScale.y
        );

        Vector2 pupilPos = center + dir * radius;
        muzzle.position = pupilPos;

        return dir;
    }

    private IEnumerator ShootCoroutine()
    {
        yield return waitForStartLag;

        Vector2 offset = beamOffset * muzzleDirection;
        beam.position = (Vector2)muzzle.position + offset;
        beam.up = muzzleDirection * -1f;
        beam.gameObject.SetActive(true);

        yield return waitForEndLag;
        beam.gameObject.SetActive(false);
    }
    #endregion

    #region PUBLIC METHODS
    public void Spawn(Vector2 spawnPos)
    {
        transform.localPosition = spawnPos;
        gameObject.SetActive(true);
    }

    public void RequestFollowTarget(Transform target)
    {
        if (target == null) return;

        this.target = target;
        isFollowTarget = true;
    }

    public void ReadyToShoot(float startLag, float endLag)
    {
        waitForStartLag ??= new WaitForSeconds(startLag);
        waitForEndLag ??= new WaitForSeconds(endLag);

        isFollowTarget = false;
        StartCoroutine(ShootCoroutine());
    }
    #endregion
}
