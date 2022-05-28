using UnityEngine;

public static class PlayerInfo
{
    // To be change when switch to Azure Playfab
    public static string displayName {
        get => PlayerPrefs.GetString(Constants.DISPLAYNAMEPREF, string.Empty);
        set => PlayerPrefs.SetString(Constants.DISPLAYNAMEPREF, value);
    }
}
