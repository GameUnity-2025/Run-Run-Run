using UnityEngine;

public static class GemsManager
{
    private const string GemsKey = "TotalGems";

    public static int GetGems()
    {
        return PlayerPrefs.GetInt(GemsKey, 0);
    }

    public static void AddGems(int amount)
    {
        if (amount <= 0) return;
        int current = GetGems();
        PlayerPrefs.SetInt(GemsKey, current + amount);
        PlayerPrefs.Save();
    }

    public static bool SpendGems(int amount)
    {
        if (amount <= 0) return true;
        int current = GetGems();
        if (current < amount) return false;
        PlayerPrefs.SetInt(GemsKey, current - amount);
        PlayerPrefs.Save();
        return true;
    }
}