using UnityEngine;
using UnityEngine.UI;
using TMPro; // nếu bạn dùng TextMeshPro

public class ShopManager : MonoBehaviour
{
    public PlayerData[] players; // danh sách nhân vật
    public GameObject playerButtonPrefab; // prefab nút mua/chọn
    public Transform buttonContainer; // nơi chứa các nút

    private void Start()
    {
        LoadShop();
    }

    void LoadShop()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        foreach (PlayerData player in players)
        {
            GameObject btn = Instantiate(playerButtonPrefab, buttonContainer);
            btn.transform.Find("CharacterName").GetComponent<TextMeshProUGUI>().text = player.playerName;

            Button buyButton = btn.transform.Find("BuyButton").GetComponent<Button>();
            Button selectButton = btn.transform.Find("SelectButton").GetComponent<Button>();

            buyButton.gameObject.SetActive(!player.isPurchased);
            selectButton.gameObject.SetActive(player.isPurchased);

            buyButton.onClick.AddListener(() => BuyPlayer(player));
            selectButton.onClick.AddListener(() => SelectPlayer(player));
        }
    }

    void BuyPlayer(PlayerData player)
    {
        // ở đây bạn có thể trừ tiền, hoặc đơn giản set isPurchased = true
        player.isPurchased = true;
        PlayerPrefs.SetInt(player.playerName + "_purchased", 1);
        LoadShop();
    }

    void SelectPlayer(PlayerData player)
    {
        PlayerPrefs.SetString("SelectedCharacter", player.playerName);
        Debug.Log("Selected: " + player.playerName);
    }
}
