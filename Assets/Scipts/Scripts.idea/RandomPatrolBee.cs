using UnityEngine;
using System.Collections.Generic;

public class RandomPatrolBee : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField, Range(0f, 10f)] private float moveSpeed = 2f;
    [SerializeField, Range(0f, 5f)] private float minWaitTime = 1f;
    [SerializeField, Range(0f, 5f)] private float maxWaitTime = 3f;
    [SerializeField, Range(0.01f, 1f)] private float minDistanceToPoint = 0.1f;
    
    [Header("Patrol Points")]
    [SerializeField] private List<Transform> patrolPoints;
    
    private int currentPointIndex;
    private float waitTimer;
    private bool isWaiting;
    private Vector2 targetPosition;
    private SpriteRenderer spriteRenderer;
    private Vector2 direction;
    private Vector2 currentPosition;

    private void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points assigned to RandomPatrolBee!");
            enabled = false;
            return;
        }

        // Начинаем с случайной точки
        currentPointIndex = Random.Range(0, patrolPoints.Count);
        targetPosition = patrolPoints[currentPointIndex].position;
    }

    private void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewTarget();
            }
        }
        else
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        currentPosition = transform.position;
        direction = targetPosition - currentPosition;
        direction.Normalize();
        
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x > 0;
        }

        transform.position = Vector2.MoveTowards(
            currentPosition, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(currentPosition, targetPosition) < minDistanceToPoint)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
    }

    private void SetNewTarget()
    {
        if (patrolPoints.Count <= 1) return;

        var availablePoints = new List<Transform>(patrolPoints);
        availablePoints.RemoveAt(currentPointIndex);
        currentPointIndex = patrolPoints.IndexOf(availablePoints[Random.Range(0, availablePoints.Count)]);
        targetPosition = patrolPoints[currentPointIndex].position;
    }

    // Визуализация точек патрулирования в редакторе
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }

    private void OnValidate()
    {
        if (maxWaitTime < minWaitTime)
            maxWaitTime = minWaitTime;
    }
}
