using UnityEngine;

public class BeePatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed;
    
    private Transform currentPoint;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPoint = pointB;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPoint.position, moveSpeed * Time.deltaTime);


        if (Vector3.Distance(transform.position,currentPoint.position) < 0.1f)
        {
            currentPoint = currentPoint == pointA ? pointB : pointA;
            if (currentPoint == pointB)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
