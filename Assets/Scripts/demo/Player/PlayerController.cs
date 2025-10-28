using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;

    [Header("Health UI Reference")]

    private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private bool isGrounded = false;
    private Vector3 startPosition;
    private GameManager gameManager;

    private bool canMove = true; // 🔒 Khóa điều khiển khi GameOver
    private bool facingRight = true; // 🔄 Lưu hướng hiện tại của player

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void LateUpdate()
    {
        // Giữ player không bị xoay nghiêng
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        // Nếu game over hoặc player bị khóa -> không xử lý input
        if (!canMove || gameManager.IsGameOver() || gameManager.IsGameWon())
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimation();
            return;
        }

        // Di chuyển trái phải
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // ⚡ Quay đầu theo hướng di chuyển
        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        HandleJump();

        // Rơi khỏi vực
        if (transform.position.y < -10f)
        {
            TriggerGameOver();
        }

        UpdateAnimation();
    }

    // 🔄 Hàm quay đầu player
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Đảo chiều trục X
        transform.localScale = scale;
    }

    // Gọi GameOver một cách an toàn
    private void TriggerGameOver()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        gameManager.GameOver();
    }

    void Respawn()
    {
        

        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }


    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }
}
