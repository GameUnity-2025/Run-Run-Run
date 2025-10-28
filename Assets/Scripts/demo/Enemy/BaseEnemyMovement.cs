using UnityEngine;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    [Header("Common Movement Settings")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float distance = 3f;

    protected Vector3 initialPosition;

    protected virtual void Start()
    {
        initialPosition = transform.position;
    }

    protected virtual void Update()
    {
        Move(); // gọi hàm abstract mà class con sẽ cài đặt
    }

    protected abstract void Move(); // hàm abstract (chưa định nghĩa)
}