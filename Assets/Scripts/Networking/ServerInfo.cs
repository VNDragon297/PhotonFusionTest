using UnityEngine;

public static class ServerInfo
{
    public const int MaxCapacity = 8;  // Room hard limit

    public static string LobbyName;

    public static int MaxUsers
    {
        get => PlayerPrefs.GetInt(Constants.LOBBYSIZEPREF, 4);
        set => PlayerPrefs.SetInt(Constants.LOBBYSIZEPREF, Mathf.Clamp(value, 4, MaxCapacity));
    }
}
