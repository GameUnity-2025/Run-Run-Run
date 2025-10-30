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

			// Nút action
			Button selectButton = btn.transform.Find("SelectButton")?.GetComponent<Button>();
			if (selectButton == null)
				selectButton = btn.transform.Find("SellectButton")?.GetComponent<Button>();
			if (selectButton == null)
				selectButton = btn.GetComponentInChildren<Button>();

			if (selectButton != null)
			{
				var selectedPlayer = player;
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
					selectButton.interactable = true;
					selectButton.onClick.AddListener(() => TryBuy(selectedPlayer));
					if (btnText != null) btnText.text = "Buy 10 gems";
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

	// Dùng trong Inspector: truyền vào tên nhân vật tương ứng của button
	public void OnClick_SelectOrBuy(string playerName)
	{
		var player = GetPlayerByName(playerName);
		if (player == null)
		{
			UnityEngine.Debug.LogWarning("ShopManager: Player not found for name: " + playerName);
			return;
		}
		if (IsUnlocked(player) || player.isDefault)
		{
			SelectPlayer(player);
		}
		else
		{
			TryBuy(player);
		}
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

	private PlayerData GetPlayerByName(string playerName)
	{
		foreach (var p in players)
		{
			if (p != null && p.playerName == playerName) return p;
		}
		return null;
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

	// Debug/Utility: Reset unlocks so you can re-test the buying flow
	public void ResetPurchasesKeepGems()
	{
		ResetPurchasesInternal(false);
	}

	public void ResetPurchasesAndGems()
	{
		ResetPurchasesInternal(true);
	}

    //reset tất cả mua và tùy chọn đặt lại gems
    private void ResetPurchasesInternal(bool alsoResetGems)
	{
		// Clear all unlocked flags
		foreach (var p in players)
		{
			if (p == null) continue;
			if (p.isDefault) continue; // default luôn mở
			PlayerPrefs.SetInt(GetUnlockKey(p.playerName), 0);
		}

		// Đặt lại nhân vật mặc định
		PlayerPrefs.SetString("SelectedCharacter", "PlayerDefault");

		if (alsoResetGems)
		{
			PlayerPrefs.SetInt("TotalGems", 0);
		}

		PlayerPrefs.Save();

		// Refresh UI
		RefreshGemsUI();
		LoadShop();
	}

	// Reset một nhân vật duy nhất (dùng cho nút Reset cạnh từng item)
	//public void ResetCharacterPurchase(string playerName)
	//{
	//	var p = GetPlayerByName(playerName);
	//	if (p == null)
	//	{
	//		UnityEngine.Debug.LogWarning("ShopManager: Player not found for reset: " + playerName);
	//		return;
	//	}

	//	if (!p.isDefault)
	//	{
	//		PlayerPrefs.SetInt(GetUnlockKey(p.playerName), 0);
	//	}

	//	// Nếu đang chọn đúng nhân vật này, chuyển về mặc định
	//	string selectedName = PlayerPrefs.GetString("SelectedCharacter", "PlayerDefault");
	//	if (selectedName == p.playerName)
	//	{
	//		PlayerPrefs.SetString("SelectedCharacter", "PlayerDefault");
	//	}

	//	PlayerPrefs.Save();
	//	LoadShop();
	//}

	// Hoàn tác toàn bộ: xóa tất cả mua và hoàn gems đã dùng
	public void ResetAllPurchasesRefundGems()
	{
		int refund = 0;
		foreach (var p in players)
		{
			if (p == null) continue;
			if (p.isDefault) continue;
			if (PlayerPrefs.GetInt(GetUnlockKey(p.playerName), 0) == 1)
			{
				refund += p.price;
				PlayerPrefs.SetInt(GetUnlockKey(p.playerName), 0);
			}
		}

		if (refund > 0) GemsManager.AddGems(refund);

		PlayerPrefs.SetString("SelectedCharacter", "PlayerDefault");
		PlayerPrefs.Save();

		RefreshGemsUI();
		LoadShop();
	}
}
