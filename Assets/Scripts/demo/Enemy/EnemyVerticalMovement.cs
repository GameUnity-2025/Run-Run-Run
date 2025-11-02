using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyVerticalMovement : BaseEnemyMovement
{
    private bool movingUp = true;

    protected override void Move()
    {
        float top = initialPosition.y + distance;
        float bottom = initialPosition.y - distance;

        if (movingUp)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (transform.position.y >= top) movingUp = false;
        }
        else
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y <= bottom) movingUp = true;
        }
    }
}
