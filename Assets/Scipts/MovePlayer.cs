using System;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private GameObject levelObject;
    private float jumpCooldown  = 0.5f;
    private float jumpTimer;
    private bool IsGrounded;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
            rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.linearVelocity.y);

            if (Input.GetButtonDown("Jump")) 
            {
                if (IsGrounded) 
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    jumpTimer = 0f; // Сбрасываем таймер
                }
                else if (jumpTimer < jumpCooldown) // Второй прыжок, если в пределах времени
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    jumpTimer = jumpCooldown; // Завершаем возможность прыжка
                }
            }
            //if (!IsGrounded)
            //{
                //jumpTimer += Time.deltaTime;
            //}
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == levelObject)
        {
            IsGrounded = true;
            
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject == levelObject)
        {
            IsGrounded = false;
        }
    }

    
}
