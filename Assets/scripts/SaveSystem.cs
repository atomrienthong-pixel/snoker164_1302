using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One ball as it sat on the table when the game was saved.
/// </summary>
[Serializable]
public class BallState
{
    public int color;
    public float x;
    public float y;
    public float z;

    public BallColor Colour => (BallColor)color;
    public Vector3 Position => new Vector3(x, y, z);

    public static BallState From(BallColor colour, Vector3 position)
    {
        return new BallState
        {
            color = (int)colour,
            x = position.x,
            y = position.y,
            z = position.z,
        };
    }
}

/// <summary>
/// The whole frame: both players' scores, whose turn it is, and where every
/// remaining ball was standing.
/// </summary>
[Serializable]
public class SaveData
{
    public int score0;
    public int score1;
    public int turn;
    public bool needRed;
    public string savedAt;
    public List<BallState> balls = new List<BallState>();
}

/// <summary>
/// A one-slot save kept in PlayerPrefs as JSON. It stores the actual table, not
/// just the score, so loading puts every ball back where it was rather than
/// racking a fresh frame.
/// </summary>
public static class SaveSystem
{
    const string HasSaveKey = "save.exists";
    const string DataKey = "save.data";

    /// <summary>True when a saved frame is waiting to be picked up.</summary>
    public static bool HasSave =>
        PlayerPrefs.GetInt(HasSaveKey, 0) == 1 &&
        !string.IsNullOrEmpty(PlayerPrefs.GetString(DataKey, string.Empty));

    /// <summary>
    /// Set just before the game scene loads. GameManger reads it in Start to
    /// decide between racking a new frame and restoring the saved one.
    /// </summary>
    public static bool LoadOnNextStart { get; set; }

    public static void Save(SaveData data)
    {
        if (data == null)
            return;

        data.savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        PlayerPrefs.SetString(DataKey, JsonUtility.ToJson(data));
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"[SaveSystem] Saved: P1 {data.score0} - P2 {data.score1}, " +
                  $"{data.balls.Count} balls left");
    }

    public static SaveData Load()
    {
        if (!HasSave)
            return null;

        string json = PlayerPrefs.GetString(DataKey, string.Empty);

        try
        {
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data != null && data.balls != null)
                return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[SaveSystem] The save could not be read: " + e.Message);
        }

        return null;
    }

    public static void Delete()
    {
        PlayerPrefs.DeleteKey(HasSaveKey);
        PlayerPrefs.DeleteKey(DataKey);
        PlayerPrefs.Save();
    }

    /// <summary>One line for the pause panel, or empty when there is no save.</summary>
    public static string Summary()
    {
        var data = Load();
        if (data == null)
            return string.Empty;

        string score = $"P1 {data.score0} - P2 {data.score1}, {data.balls.Count} balls left";
        return string.IsNullOrEmpty(data.savedAt) ? score : $"{score}  ({data.savedAt})";
    }
}
