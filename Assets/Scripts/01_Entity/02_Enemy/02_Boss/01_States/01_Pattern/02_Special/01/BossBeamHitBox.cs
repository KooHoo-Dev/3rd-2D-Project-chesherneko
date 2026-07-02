using Sirenix.OdinInspector;
using UnityEngine;

public class BossBeamHitBox : HitBox
{
    [Title("Components")]
    [SerializeField, PropertyOrder(-1)] private SpriteRenderer beamRenderer;

    private void Update()
    {
        transform.localScale = new Vector3(1f, beamRenderer.size.y, 1f);
    }
}
