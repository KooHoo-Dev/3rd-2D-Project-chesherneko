using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private Rigidbody2D body;

    [Title("Settings")]
    [SerializeField] private float shootSpeed = 3f;
    [SerializeField] private float shootDuration = 2f;
    [SerializeField] private LayerMask hitLayerMask;

    [Title("Runtime State")]
    [ShowInInspector, ReadOnly] private bool isShooted = false;
    [ShowInInspector, ReadOnly] private Vector2 shootDirection;

    [Space]
    [ShowInInspector, ReadOnly] private float shootTime = 0f;

    private ProjectileShooter shooter;

    private void Update()
    {
        if (isShooted == false) return;
        UpdateShootTime();
    }

    private void FixedUpdate()
    {
        if (isShooted == false) return;
        UpdatePosition();
    }

    private void OnDisable()
    {
        isShooted = false;
        shootTime = 0f;
    }

    public void InitShooter(ProjectileShooter shooter)
        => this.shooter = shooter;

    public void Shoot(Vector2 dir)
    {
        isShooted = true;
        shootDirection = dir;

        gameObject.SetActive(true);

        transform.SetParent(null);
        transform.localScale = Vector3.one;
        transform.right = shootDirection;
    }

    private void UpdateShootTime()
    {
        shootTime += Time.deltaTime;

        if (shootTime > shootDuration)
            shooter.ReturnProjectile(this);
    }

    private void UpdatePosition()
    {
        float dist = shootSpeed * Time.fixedDeltaTime;
        Vector2 delta = dist * shootDirection;

        body.MovePosition(body.position + delta);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitLayerMask.Contains(collision) == false) 
            return;

        shooter.ReturnProjectile(this);
    }
}
