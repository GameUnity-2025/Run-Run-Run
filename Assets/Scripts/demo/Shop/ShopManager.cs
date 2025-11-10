using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    public PlayerData[] players;
    public GameObject playerButtonPrefab;
    public Transform buttonContainer;

    [Header("Currency UI")]
    public TextMeshProUGUI gemsText;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Image detailCharacterImage;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailPriceText;
    public Image detailGemIcon;
    public Button detailBuyButton;
    public Button detailSelectButton;
    public TextMeshProUGUI buyButtonText;
    public TextMeshProUGUI selectButtonText;

    private string selectedName;
    private PlayerData currentViewingPlayer;

    private void Start()
    {
        // Gán gems text nếu chưa có
        if (gemsText == null)
        {
            var found = GameObject.Find("GemsText");
            if (found != null) gemsText = found.GetComponent<TextMeshProUGUI>();
        }

        // Gán nhân vật mặc định
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("SelectedCharacter", "")))
        {
            var def = GetDefaultPlayer();
            if (def != null)
                PlayerPrefs.SetString("SelectedCharacter", def.playerName);
        }

        selectedName = PlayerPrefs.GetString("SelectedCharacter", "");

        if (detailPanel)
            detailPanel.SetActive(false);

        if (detailBuyButton)
            detailBuyButton.onClick.AddListener(OnDetailBuyClicked);

        if (detailSelectButton)
            detailSelectButton.onClick.AddListener(OnDetailSelectClicked);

        RefreshGemsUI();
        LoadShop();
    }

    // 🔹 Tạo danh sách PlayerButton
    public void LoadShop()
    {
        if (!buttonContainer || !playerButtonPrefab)
        {
            Debug.LogError("ShopManager: buttonContainer hoặc playerButtonPrefab chưa gán!");
            return;
        }

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (var player in players)
        {
            if (player == null) continue;

            // ✅ Giữ nguyên layout gốc của prefab
            var obj = Instantiate(playerButtonPrefab, buttonContainer, false);

            var ui = obj.GetComponent<ShopItemUI>();
            bool unlocked = IsUnlocked(player);
            bool selected = selectedName == player.playerName;

            if (ui != null)
                ui.Setup(player, this, unlocked, selected);
        }
    }

    // 🔹 Hiển thị chi tiết nhân vật
    public void ShowDetail(PlayerData player)
    {
        if (player == null) return;
        currentViewingPlayer = player;

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play_Button();

        if (!detailPanel) return;
        detailPanel.SetActive(true);

        if (detailCharacterImage)
            detailCharacterImage.sprite = player.characterSprite;

        if (detailNameText)
            detailNameText.text = player.playerName;

        bool unlocked = IsUnlocked(player);
        bool selected = selectedName == player.playerName;

        // Hiển thị giá và icon gem
        if (detailPriceText)
        {
            if (unlocked || player.isDefault)
            {
                detailPriceText.gameObject.SetActive(false);
                if (detailGemIcon) detailGemIcon.gameObject.SetActive(false);
            }
            else
            {
                detailPriceText.gameObject.SetActive(true);
                detailPriceText.text = player.price.ToString();
                if (detailGemIcon) detailGemIcon.gameObject.SetActive(true);
            }
        }

        UpdateDetailButtons(unlocked, selected);
    }

    // 🔹 Cập nhật nút mua và chọn
    private void UpdateDetailButtons(bool unlocked, bool selected)
    {
        if (detailBuyButton && buyButtonText)
        {
            if (unlocked || currentViewingPlayer.isDefault)
            {
                detailBuyButton.gameObject.SetActive(true);
                detailBuyButton.interactable = false;
                buyButtonText.text = "Owned";
            }
            else
            {
                detailBuyButton.gameObject.SetActive(true);
                int gems = GemsManager.GetGems();
                detailBuyButton.interactable = gems >= currentViewingPlayer.price;
                buyButtonText.text = detailBuyButton.interactable ? "Purchase" : "Not Enough Gems";
            }
        }

        if (detailSelectButton && selectButtonText)
        {
            if (unlocked || currentViewingPlayer.isDefault)
            {
                detailSelectButton.gameObject.SetActive(true);
                if (selected)
                {
                    detailSelectButton.interactable = false;
                    selectButtonText.text = "Selected";
                }
                else
                {
                    detailSelectButton.interactable = true;
                    selectButtonText.text = "Select";
                }
            }
            else
            {
                detailSelectButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnDetailBuyClicked()
    {
        if (currentViewingPlayer == null) return;

        if (GemsManager.SpendGems(currentViewingPlayer.price))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.Play_Purchase();

            Unlock(currentViewingPlayer);
            RefreshGemsUI();
            LoadShop();
            ShowDetail(currentViewingPlayer);

            if (buyButtonText)
                buyButtonText.text = "Owned";
            if (detailBuyButton)
                detailBuyButton.interactable = false;

            Debug.Log($"✅ Purchased: {currentViewingPlayer.playerName}");
        }
        else
        {
            Debug.Log($"❌ Not enough gems to buy {currentViewingPlayer.playerName}");
        }
    }

    private void OnDetailSelectClicked()
    {
        if (currentViewingPlayer == null) return;
        SelectPlayer(currentViewingPlayer);
        ShowDetail(currentViewingPlayer);
    }

    public void SelectPlayer(PlayerData player)
    {
        if (player == null) return;

        selectedName = player.playerName;
        PlayerPrefs.SetString("SelectedCharacter", selectedName);
        PlayerPrefs.Save();
        LoadShop();
        Debug.Log($"Selected character: {selectedName}");
    }

    private static string GetUnlockKey(string name) => $"Unlocked_{name}";

    private bool IsUnlocked(PlayerData player)
    {
        if (player == null) return false;
        return player.isDefault || PlayerPrefs.GetInt(GetUnlockKey(player.playerName), 0) == 1;
    }

    private void Unlock(PlayerData player)
    {
        PlayerPrefs.SetInt(GetUnlockKey(player.playerName), 1);
        PlayerPrefs.Save();
    }

    private void RefreshGemsUI()
    {
        if (gemsText)
            gemsText.text = GemsManager.GetGems().ToString();
    }

    private PlayerData GetDefaultPlayer()
    {
        foreach (var p in players)
            if (p != null && p.isDefault)
                return p;
        return players.Length > 0 ? players[0] : null;
    }

    public void ResetAllPurchases()
    {
        foreach (var p in players)
        {
            if (p == null || p.isDefault) continue;
            PlayerPrefs.DeleteKey(GetUnlockKey(p.playerName));
        }
        var def = GetDefaultPlayer();
        PlayerPrefs.SetString("SelectedCharacter", def != null ? def.playerName : "");
        PlayerPrefs.Save();
        selectedName = PlayerPrefs.GetString("SelectedCharacter", "");
        LoadShop();

        if (detailPanel)
            detailPanel.SetActive(false);
    }

    public GameObject GetSelectedCharacterPrefab()
    {
        var p = System.Array.Find(players, x => x.playerName == selectedName);
        return p != null ? p.characterPrefab : null;
    }

    public void CloseDetailPanel()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play_MenuBack();

        if (detailPanel)
            detailPanel.SetActive(false);
    }
}
