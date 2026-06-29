using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BossMinionSpawner : MonoBehaviour
{
    private static readonly int OUTLINE_ALPHA = Shader.PropertyToID("_OutlineAlpha");

    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private GameObject hurtBox;

    [Title("Settings")]
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnHalfRange;
    [SerializeField] private float spawnOffsetY;

    [Space]
    [SerializeField] private float fadeInDuration;

    [Title("References")]
    [SerializeField] private GameObject[] prefabs;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] public int RemainMinionCount = 0;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new();
    }

    private void OnDisable()
    {
        hurtBox.SetActive(false);
        SetOutlineAlpha(isActive: false);
    }

    public IEnumerator InitCoroutine()
    {
        gameObject.SetActive(true);

        yield return renderer
            .FadeIn(fadeInDuration)
            .WaitForCompletion();

        for (int i = 0; i < spawnCount; i++)
            SpawnRandomMinion();
    }

    public void NotifyMinionDestroyed()
    {
        RemainMinionCount -= 1;

        if (RemainMinionCount == 0)
        {
            hurtBox.SetActive(true);
            SetOutlineAlpha(isActive: true);
        }
    }

    private void SpawnRandomMinion()
    {
        RemainMinionCount += 1;

        int randIdx = Random.Range(0, prefabs.Length);
        GameObject prefab = prefabs[randIdx];

        GameObject minion = Instantiate(prefab, transform);
        Transform minionTrans = minion.transform;

        float randX = Random.Range(-spawnHalfRange, spawnHalfRange);
        minionTrans.localPosition = new Vector2(randX, spawnOffsetY);

        float lookDir = randX < 0f ? 1f : -1f;
        Vector3 tempScale = minionTrans.localScale;
        tempScale.x = lookDir * Mathf.Abs(tempScale.x);
        minionTrans.localScale = tempScale;

        minion.SetActive(true);
    }

    private void SetOutlineAlpha(bool isActive)
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(OUTLINE_ALPHA, isActive ? 1f : 0f);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
