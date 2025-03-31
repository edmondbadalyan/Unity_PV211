using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    
    private float currentLifeTime;
    private Rigidbody2D rb;
    private PlayerShooting playerShooting;
    private bool isFacingRight;
    private SpriteRenderer playerSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerShooting = FindAnyObjectByType<PlayerShooting>();
        playerSprite = playerShooting.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        currentLifeTime = lifeTime;
        // Get the player's facing direction from the firePoint
        Vector2 direction = playerSprite.flipX ? Vector2.left : Vector2.right;
        
        rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0f)
        {
            playerShooting.ReturnBullet(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            playerShooting.ReturnBullet(gameObject);
            Destroy(collision.gameObject);
        }
    }
}