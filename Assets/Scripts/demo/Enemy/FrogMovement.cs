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

    [Header("Sound - Frog Jump")]
    [SerializeField] private AudioClip frogJumpClip;    // Clip nhảy của ếch (gán trong Inspector)

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWaiting;
    private EnemySoundController soundController; // dùng để tắt loop và tái dụng AudioSource
    private AudioSource jumpAudioSource; // nguồn phát jump one-shot

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 4.5f; // điều chỉnh cho cảm giác nhảy thật
        soundController = GetComponent<EnemySoundController>();
        if (soundController != null)
        {
            // tắt phát liên tục cho ếch, chỉ phát khi nhảy
            var scType = typeof(EnemySoundController);
            var field = scType.GetField("continuousLoop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(soundController, false);
            }
        }
        // chuẩn bị AudioSource để phát jump
        jumpAudioSource = GetComponent<AudioSource>();
        if (jumpAudioSource == null)
        {
            jumpAudioSource = gameObject.AddComponent<AudioSource>();
        }
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
        // Phát one-shot clip nhảy nếu đã gán
        if (frogJumpClip != null)
        {
            if (soundController != null)
            {
                // ép 3D thuần và khoảng cách 1..3 cho jump
                var src = GetComponent<AudioSource>();
                if (src != null)
                {
                    src.spatialBlend = 1f;
                    src.rolloffMode = AudioRolloffMode.Linear;
                    src.minDistance = 1f;
                    src.maxDistance = 3f;
                }
                soundController.PlayOneShot3D(frogJumpClip);
            }
            else if (SoundManager.Instance != null)
            {
                SoundManager.Instance.ConfigureEnemy3DSource(jumpAudioSource);
                // override để đảm bảo 3D thuần và tắt ngoài 3 đơn vị
                jumpAudioSource.spatialBlend = 1f;
                jumpAudioSource.minDistance = 1f;
                jumpAudioSource.maxDistance = 3f;
                SoundManager.Instance.PlayEnemyOneShot3D(jumpAudioSource, frogJumpClip, 1f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
