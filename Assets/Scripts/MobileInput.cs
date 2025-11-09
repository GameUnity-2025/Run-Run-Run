using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static float horizontal;   // -1, 0, 1
    private static bool jump;

    public void MoveLeft(bool isPressed) { horizontal = isPressed ? -1f : 0f; }
    public void MoveRight(bool isPressed) { horizontal = isPressed ? 1f : 0f; }
    public void Jump(bool isPressed) { if (isPressed) jump = true; }

    // Gọi hàm này để lấy và reset cờ nhảy (tránh lặp)
    public static bool ConsumeJump()
    {
        if (!jump) return false;
        jump = false;
        return true;
    }
}
