using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image backgroundImage;             // 🪵 khung gỗ
    public Image characterImage;              // 🧍 sprite nhân vật
    public TextMeshProUGUI characterNameText; // 🏷️ tên nhân vật
    public Image borderImage;                 // 🔲 viền khi chọn

    private PlayerData player;
    private ShopManager manager;
    private Button button;

    private void Awake()
    {
        // Đảm bảo có Button để bắt sự kiện
        button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();
    }

    public void Setup(PlayerData data, ShopManager mgr, bool unlocked, bool selected)
    {
        player = data;
        manager = mgr;

        // 1️⃣ Cập nhật hình ảnh nhân vật
        if (characterImage)
            characterImage.sprite = data.characterSprite;

        // 2️⃣ Cập nhật tên hiển thị
        if (characterNameText)
            characterNameText.text = data.playerName;

        // 3️⃣ Cập nhật màu sắc và viền
        UpdateVisual(unlocked, selected);

        // 4️⃣ Gán sự kiện click
        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.ShowDetail(player));
        }

        // ⚠️ KHÔNG reset RectTransform nếu prefab đang nằm trong LayoutGroup
        // LayoutGroup (Grid/Horizontal/Vertical) sẽ tự quản lý vị trí và scale
        var rect = GetComponent<RectTransform>();
        if (rect)
        {
            rect.localScale = Vector3.one;  // Đảm bảo không bị phóng to/thu nhỏ
        }
    }

    private void UpdateVisual(bool unlocked, bool selected)
    {
        // Viền hiển thị trạng thái
        if (borderImage)
        {
            if (selected)
                borderImage.color = Color.green;
            else if (unlocked)
                borderImage.color = Color.white;
            else
                borderImage.color = Color.gray;
        }

        // Làm mờ nhân vật nếu chưa mở khóa
        if (characterImage)
        {
            var imgColor = characterImage.color;
            imgColor.a = unlocked ? 1f : 0.5f;
            characterImage.color = imgColor;
        }

        // Tùy chọn: đổi màu text nếu bị khóa
        if (characterNameText)
        {
            characterNameText.color = unlocked ? Color.black : Color.gray;
        }
    }
}
