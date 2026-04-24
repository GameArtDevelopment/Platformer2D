using UnityEngine;

public static class SaveManager
{
    public const string CurrentLevelKey = "currentLevel";
    public const string LivesKey = "lives";
    public const string FruitKey = "fruit";

    private const string SaveFileName = "game.es3";

    public static bool HasKey(string key)
    {
        return ES3.KeyExists(key, SaveFileName);
    }

    public static void SaveInt(string key, int value)
    {
        ES3.Save(key, value, SaveFileName);
    }

    public static int LoadInt(string key, int defaultValue = 0)
    {
        return ES3.Load<int>(key, SaveFileName, defaultValue);
    }

    public static void SaveString(string key, string value)
    {
        ES3.Save(key, value, SaveFileName);
    }

    public static string LoadString(string key, string defaultValue = "")
    {
        return ES3.Load<string>(key, SaveFileName, defaultValue);
    }

    public static void DeleteKey(string key)
    {
        ES3.DeleteKey(key, SaveFileName);
    }

    public static void DeleteAll()
    {
        ES3.DeleteFile(SaveFileName);
    }
}
