using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVertical : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;      // Lực nhảy lên
    [SerializeField] private float pauseAtBottom = 1f;  // Thời gian dừng khi chạm đất
    [SerializeField] private float horizontalForce = 3f; // Lực nhảy ngang

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;     // Điểm kiểm tra chạm đất
    [SerializeField] private float groundRadius = 0.2f; // Bán kính kiểm tra
    [SerializeField] private LayerMask groundLayer;     // Layer mặt đất

    [Header("Animation")]
    [SerializeField] private Animator animator;         // Animator cho cóc

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWaiting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4.5f; // điều chỉnh cho cảm giác nhảy thật
    }

    private void Update()
    {
        // kiểm tra chạm đất
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // vừa chạm đất
        if (isGrounded && !wasGrounded && !isWaiting)
        {
            animator.SetBool("isJumping", false);
            StartCoroutine(JumpRoutine());
        }

        // đang ở trên không
        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        
        // Âm thanh được điều khiển bởi EnemySoundController component (nếu có)
        // Không cần làm gì ở đây
    }

    private IEnumerator JumpRoutine()
    {
        isWaiting = true;

        // đứng yên 1s khi chạm đất
        yield return new WaitForSeconds(pauseAtBottom);

        // chỉ nhảy khi vẫn đang chạm đất
        if (isGrounded)
        {
            Jump();
        }

        isWaiting = false;
    }

    private void Jump()
    {
        // Đảo hướng nhảy mỗi lần (qua lại trái - phải)
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Nhảy theo cả hướng ngang và dọc
        Vector2 jumpDirection = new Vector2(transform.localScale.x > 0 ? 1 : -1, 1).normalized;
        rb.linearVelocity = Vector2.zero; // reset vận tốc cũ để nhảy ổn định
        rb.AddForce(jumpDirection * new Vector2(horizontalForce, jumpForce), ForceMode2D.Impulse);

        animator.SetBool("isJumping", true);

        // Âm thanh được điều khiển bởi EnemySoundController component
        // Không cần phát ở đây nữa - EnemySoundController sẽ tự động phát khi player gần
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
