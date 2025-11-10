using UnityEngine;

public class SpringPad : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float bounceForce = 20f; // Lực bật lên
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (AudioManager.Instance != null)  
                {
                    AudioManager.Instance.Play_Bounce(); 
                }
                // Bật Player lên
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                // Phát animation bounce
                if (animator != null)
                {
                    animator.SetTrigger("Bounce");
                }
            }
        }
    }
}
