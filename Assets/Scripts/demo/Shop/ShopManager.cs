using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    public PlayerData[] players;                  // Danh sách nhân vật
    public GameObject playerButtonPrefab;         // Prefab nút chọn nhân vật
    public Transform buttonContainer;             // Nơi chứa các nút

    private void Start()
    {
        LoadShop();
    }

    void LoadShop()
    {
        // Xóa nút cũ trước khi tạo lại
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // Tạo nút mới cho từng nhân vật
        foreach (PlayerData player in players)
        {
            GameObject btn = Instantiate(playerButtonPrefab, buttonContainer);

            // Gán tên nhân vật lên Text
            var nameText = btn.transform.Find("CharacterName")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = player.playerName;

            // Gắn sự kiện khi nhấn nút "Select" (xử lý cả trường hợp prefab đặt tên "SellectButton")
            Button selectButton = btn.transform.Find("SelectButton")?.GetComponent<Button>();
            if (selectButton == null)
                selectButton = btn.transform.Find("SellectButton")?.GetComponent<Button>();
            if (selectButton == null)
                selectButton = btn.GetComponentInChildren<Button>();

            if (selectButton != null)
            {
                var selectedPlayer = player; // tránh vấn đề capture biến trong vòng lặp
                selectButton.onClick.AddListener(() => SelectPlayer(selectedPlayer));
            }
            else
            {
                UnityEngine.Debug.LogWarning("ShopManager: Không tìm thấy Button chọn trong prefab.");
            }
        }
    }

    public void SelectPlayer(PlayerData player)
    {
        // Lưu tên nhân vật đã chọn
        PlayerPrefs.SetString("SelectedCharacter", player.playerName);
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("Selected: " + player.playerName);
    }
}
