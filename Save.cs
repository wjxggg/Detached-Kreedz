using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save
{
    public static string[] Collectibles = { };

    //新建存档
    public static void CreateSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Level", 1);
        Debug.Log("新建存档");
    }

    //通关后储存关卡信息
    public static void SaveGame(int level)
    {
        if (PlayerPrefs.GetInt("Level") < level)
        {
            PlayerPrefs.SetInt("Level", level);
        }
        Debug.Log("储存关卡信息" + PlayerPrefs.GetInt("Level"));
    }

    //获得关卡信息
    public static int GetLevelInfo()
    {
        Debug.Log("关卡信息" + PlayerPrefs.GetInt("Level"));
        return PlayerPrefs.GetInt("Level");
    }

    //储存收集品信息
    public static void SetCollectible(string item)
    {
        PlayerPrefs.SetInt(item, 1);
        Debug.Log("储存收集品信息" + item);
    }

    //获得收集品信息
    public static List<string> GetCollectibleInof()
    {
        List<string> whatWeGot = new List<string>();
        foreach (string item in Collectibles)
        {
            if (PlayerPrefs.HasKey(item))
            {
                whatWeGot.Add(item);
                Debug.Log(item);
            }
        }
        return whatWeGot;
    }
}
