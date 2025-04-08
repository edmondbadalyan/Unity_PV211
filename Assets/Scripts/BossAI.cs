using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform playerTransform; // Ссылка на трансформ игрока
    public float detectionRadius = 15f; // Радиус, в котором босс замечает игрока
    public GameObject projectilePrefab; // Префаб снаряда, которым стреляет босс
    public Transform firePoint; // Точка, откуда вылетают снаряды
    public float fireRate = 1f; // Выстрелов в секунду
    public float projectileSpeed = 10f; // Скорость снаряда

    private float nextFireTime = 0f; // Время следующего возможного выстрела
    private SpriteRenderer spriteRenderer; // Ссылка на SpriteRenderer

    void Start()
    {
        // Получаем компонент SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("BossAI: SpriteRenderer component not found!");
            enabled = false;
        }

        // Попытка найти игрока по тегу "Player", если он не назначен вручную
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("BossAI: Player not found! Make sure the player GameObject has the tag 'Player' or assign it manually.");
                enabled = false; // Отключаем скрипт, если игрок не найден
            }
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("BossAI: Projectile Prefab is not assigned!");
            enabled = false;
        }

        if (firePoint == null)
        {
            Debug.LogError("BossAI: Fire Point is not assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        if (playerTransform == null) return; // Выход, если игрок все еще не найден

        // Вычисляем дистанцию до игрока
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Проверяем, находится ли игрок в радиусе обнаружения
        if (distanceToPlayer <= detectionRadius)
        {
            // Определяем направление к игроку
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            
            // Переворачиваем спрайт в зависимости от направления
            if (direction.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = false;
            }

            // Стреляем, если пришло время
            if (Time.time >= nextFireTime)
            {
                Shoot();
                // Устанавливаем время следующего выстрела
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        
        // Добавляем скорость снаряду
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = directionToPlayer * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("BossAI: Projectile prefab doesn't have Rigidbody2D component!");
        }
    }

    // Отрисовка радиуса обнаружения в редакторе для удобства
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
} 