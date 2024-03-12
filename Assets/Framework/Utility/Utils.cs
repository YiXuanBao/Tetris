
using Google.Protobuf;
using SocketGameProtocol;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static int[] lastFrameCounts = new int[3];

    #region Dictionary
    public static bool InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict == null)
        {
            Debug.LogError("InsertOrUpdateKeyInDictionary: <Dictionary dict> is null.");
            return false;
        }
        if (dict.ContainsKey(key))
            dict[key] = value;
        else
            dict.Add(key, value);

        return true;
    }

    public static bool RemoveKeyInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict == null)
        {
            Debug.LogError("RemoveKeyInDictionary: <Dictionary dict> is null.");
            return false;
        }
        if (dict.ContainsKey(key))
        {
            dict.Remove(key);
            return true;
        }
        else
            return false;
    }

    public static TValue GetValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict == null)
        {
            Debug.LogError("GetValueInDictionary: <Dictionary dict> is null.");
            return default(TValue);
        }
        if (dict.ContainsKey(key))
            return dict[key];
        else
        {
            Debug.LogError("GetValueInDictionary: Key is not present.");
            return default(TValue);
        }
    }
    #endregion

    #region Log

    public static void Log(object message, bool detailed = false)
    {
        if (detailed)
            Debug.Log($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[0]}>");
        else
            Debug.Log($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
        lastFrameCounts[0] = Time.frameCount;
    }

    public static void LogWarning(object message, bool detailed = false)
    {
        if (detailed)
            Debug.LogWarning($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[1]}>");
        else
            Debug.LogWarning($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
        lastFrameCounts[1] = Time.frameCount;
    }

    public static void LogError(object message, bool detailed = false)
    {
        if (detailed)
            Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[2]}>");
        else
            Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
        lastFrameCounts[2] = Time.frameCount;
    }

    public static void PrintArray<T>(T[] array)
    {
        foreach (T unit in array)
            Log(unit);
    }

    public static void PrintList<T>(List<T> list)
    {
        foreach (T unit in list)
            Log(unit);
    }

    /// <summary>
    /// Print out each key value pair of the dictionary in the console.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    public static void PrintDict<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        foreach (var key in dict.Keys)
            Log($"Key: {key} --- Value: {dict[key]}");
    }
    #endregion

    public static byte[] MainPackToBytes(MainPack mainPack)
    {
        return mainPack.ToByteArray();
    }
}

