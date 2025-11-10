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

    [Header("Sound Settings")]
    [SerializeField] private float footstepInterval = 0.3f; // Khoảng thời gian giữa các bước chân
    private float footstepTimer = 0f;
    private bool wasMoving = false;
    private AudioSource footstepAudioSource; // AudioSource riêng cho footstep sound

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Tạo AudioSource riêng cho footstep sound
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.loop = false;
        footstepAudioSource.spatialBlend = 0f; // 2D sound
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
            HandleFootstepSound(false); // Dừng âm thanh khi không di chuyển/game over
            UpdateAnimation();
            return;
        }

        // Lấy input từ mobile trước, nếu nhỏ hơn ngưỡng thì dùng Input từ bàn phím
        float move = Mathf.Abs(MobileInput.horizontal) > 0.01f
                     ? MobileInput.horizontal
                     : Input.GetAxis("Horizontal");

        // Gán vận tốc
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Kiểm tra di chuyển để bật âm bước chân
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded;
        HandleFootstepSound(isMoving);

        // Quay đầu
        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        // Xử lý nhảy
        HandleJump();

        // Rơi khỏi vực
        if (transform.position.y < -10f)
        {
            TriggerGameOver();
        }

        // Cập nhật animation
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
        bool jumpPressed = MobileInput.ConsumeJump() || Input.GetButtonDown("Jump");

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            
            // Phát âm thanh nhảy
            PlayJumpSound();
        }
    }

    private void HandleFootstepSound(bool isMoving)
    {
        // Chỉ phát âm thanh khi nhân vật đang di chuyển thực sự
        if (isMoving)
        {
            // Phát âm thanh ngay khi bắt đầu di chuyển
            if (!wasMoving)
            {
                footstepTimer = 0f; // Reset timer khi bắt đầu di chuyển
                PlayFootstepSound(); // Phát âm thanh ngay lập tức
            }
            
            // Tiếp tục phát âm thanh theo interval
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
            wasMoving = true;
        }
        else
        {
            // Dừng đếm thời gian và dừng âm thanh khi không di chuyển
            footstepTimer = 0f;
            wasMoving = false;
            
            // Dừng âm thanh footstep ngay lập tức khi dừng di chuyển
            StopFootstepSound();
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepAudioSource != null && SoundManager.Instance != null && SoundManager.Instance.playerFootstepSound != null)
        {
            // Chỉ phát nếu chưa đang phát hoặc đã phát xong
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = SoundManager.Instance.playerFootstepSound;
                footstepAudioSource.volume = SoundManager.Instance.defaultSFXVolume * SoundManager.Instance.sfxVolume;
                footstepAudioSource.Play();
            }
        }
    }

    private void StopFootstepSound()
    {
        // Dừng âm thanh footstep ngay lập tức
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }
    }

    private void PlayJumpSound()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.playerJumpSound != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.playerJumpSound, SoundManager.Instance.defaultSFXVolume);
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
