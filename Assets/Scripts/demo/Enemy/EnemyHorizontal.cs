using UnityEngine;

public class EnemyHorizontal : BaseEnemyMovement
{
    private bool movingRight = true;

    protected override void Move()
    {
        float left = initialPosition.x - distance;
        float right = initialPosition.x + distance;

        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.position.x >= right)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x <= left)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
