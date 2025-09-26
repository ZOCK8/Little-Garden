using UnityEngine;

public static class SaveSystem
{
    public static void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSave", json);
        PlayerPrefs.Save(); // wichtig für WebGL
        Debug.Log("Gespeichert: " + json);
    }

    public static PlayerData Load()
    {
        if (PlayerPrefs.HasKey("PlayerSave"))
        {
            string json = PlayerPrefs.GetString("PlayerSave");
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Geladen: " + json);
            return data;
        }
        else
        {
            Debug.LogWarning("Keine gespeicherten Daten gefunden.");
            return null;
        }
    }
}
