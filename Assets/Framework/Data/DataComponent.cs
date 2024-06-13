using UnityEngine;

public class DataComponent : GameFrameworkComponent
{
    public string UserName;

    public void SetData(string key, string value)
    {
        PlayerPrefs.SetString(UserName + key, value);
    }

    public void SetData(string key, int value)
    {
        PlayerPrefs.SetInt(UserName + key, value);
    }

    public void SetData(string key, float value)
    {
        PlayerPrefs.SetFloat(UserName + key, value);
    }

    public string GetData(string key)
    {
        return PlayerPrefs.GetString(UserName + key, string.Empty);
    }

    /// <summary>
    /// 返回int.MaxValue表示不存在此key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int GetIntData(string key)
    {
        return PlayerPrefs.GetInt(UserName + key, int.MaxValue);
    }

    /// <summary>
    /// 返回float.NaN表示不存在此key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float GetFloatData(string key)
    {
        return PlayerPrefs.GetFloat(UserName + key, float.NaN);
    }
}