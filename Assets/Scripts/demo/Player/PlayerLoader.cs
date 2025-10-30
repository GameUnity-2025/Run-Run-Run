using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public List<PlayerData> allPlayers;

    void Start()
    {
        string selected = PlayerPrefs.GetString("SelectedCharacter", "PlayerDefault");

        PlayerData chosen = allPlayers.Find(c => c.playerName == selected);
        if (chosen != null)
            spriteRenderer.sprite = chosen.characterSprite;
        else
            spriteRenderer.sprite = defaultSprite;
    }
}
