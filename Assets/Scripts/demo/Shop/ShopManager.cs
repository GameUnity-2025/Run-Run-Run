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
	[Header("Currency UI")]
	public TextMeshProUGUI gemsText;              // Hiển thị số gems hiện có

	private void Start()
    {
		if (gemsText == null)
		{
			var found = GameObject.Find("GemsText");
			if (found != null) gemsText = found.GetComponent<TextMeshProUGUI>();
		}
		RefreshGemsUI();
		LoadShop();
    }

    void LoadShop()
    {
        // Xóa nút cũ trước khi tạo lại
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        string selectedName = PlayerPrefs.GetString("SelectedCharacter", "PlayerDefault");

        // Tạo nút mới cho từng nhân vật
        foreach (PlayerData player in players)
        {
            GameObject btn = Instantiate(playerButtonPrefab, buttonContainer);

            // Gán tên nhân vật lên Text
            var nameText = btn.transform.Find("CharacterName")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = player.playerName;

			// Gán giá nếu có node PriceText
            var priceText = btn.transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
            bool unlocked = IsUnlocked(player);
            bool isCurrent = selectedName == player.playerName;
			if (priceText != null)
			{
				priceText.text = unlocked || player.isDefault ? "Owned" : player.price.ToString();
			}

            // Gắn sự kiện khi nhấn nút "Select" (xử lý cả trường hợp prefab đặt tên "SellectButton")
			Button selectButton = btn.transform.Find("SelectButton")?.GetComponent<Button>();
            if (selectButton == null)
                selectButton = btn.transform.Find("SellectButton")?.GetComponent<Button>();
            if (selectButton == null)
                selectButton = btn.GetComponentInChildren<Button>();

            if (selectButton != null)
            {
                var selectedPlayer = player; // tránh vấn đề capture biến trong vòng lặp
				selectButton.onClick.RemoveAllListeners();
                var btnText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
                if (unlocked || player.isDefault)
				{
                    if (isCurrent)
                    {
                        selectButton.interactable = false;
                        if (btnText != null) btnText.text = "Selected";
                    }
                    else
                    {
                        selectButton.interactable = true;
                        selectButton.onClick.AddListener(() => SelectPlayer(selectedPlayer));
                        if (btnText != null) btnText.text = "Select";
                    }
				}
				else
				{
					selectButton.onClick.AddListener(() => TryBuy(selectedPlayer));
                    if (btnText != null) btnText.text = "Buy";
                    selectButton.interactable = true;
				}
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
        LoadShop();
    }

	private void TryBuy(PlayerData player)
	{
		if (player.isDefault)
		{
			SelectPlayer(player);
			return;
		}

		if (IsUnlocked(player))
		{
			SelectPlayer(player);
			return;
		}

		if (GemsManager.SpendGems(player.price))
		{
			Unlock(player);
			RefreshGemsUI();
			LoadShop();
		}
		else
		{
			UnityEngine.Debug.Log("Not enough gems to buy: " + player.playerName);
		}
	}

	private static string GetUnlockKey(string playerName)
	{
		return "Unlocked_" + playerName;
	}

	private bool IsUnlocked(PlayerData player)
	{
		if (player.isDefault) return true;
		return PlayerPrefs.GetInt(GetUnlockKey(player.playerName), 0) == 1;
	}

	private void Unlock(PlayerData player)
	{
		PlayerPrefs.SetInt(GetUnlockKey(player.playerName), 1);
		PlayerPrefs.Save();
	}

	private void RefreshGemsUI()
	{
		if (gemsText != null)
		{
			gemsText.text = GemsManager.GetGems().ToString();
		}
	}
}
