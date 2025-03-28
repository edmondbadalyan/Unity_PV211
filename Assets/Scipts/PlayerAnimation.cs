using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private MovePlayer movePlayer;
    private SpriteRenderer spriteRenderer;

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    //private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movePlayer = GetComponent<MovePlayer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        // Получаем текущую скорость по горизонтали
        float horizontalInput = Input.GetAxis("Horizontal");

        // Обновляем направление спрайта
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }

    // Определяем состояния
        bool isJumping = !movePlayer.IsGrounded;
        bool isRunning = Mathf.Abs(horizontalInput) > 0.1f && !isJumping;
        bool isIdle = movePlayer.IsGrounded && !isRunning;

        // Устанавливаем параметры аниматора
        animator.SetBool(IsJumping, isJumping);
        animator.SetBool(IsRunning, isRunning);
        animator.SetBool(IsIdle, isIdle);
    }
}
