using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save
{
    public static string[] Collectibles = { };

    //�½��浵
    public static void CreateSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Level", 1);
        Debug.Log("�½��浵");
    }

    //ͨ�غ󴢴�ؿ���Ϣ
    public static void SaveGame(int level)
    {
        if (PlayerPrefs.GetInt("Level") < level)
        {
            PlayerPrefs.SetInt("Level", level);
        }
        Debug.Log("����ؿ���Ϣ" + PlayerPrefs.GetInt("Level"));
    }

    //��ùؿ���Ϣ
    public static int GetLevelInfo()
    {
        Debug.Log("�ؿ���Ϣ" + PlayerPrefs.GetInt("Level"));
        return PlayerPrefs.GetInt("Level");
    }

    //�����ռ�Ʒ��Ϣ
    public static void SetCollectible(string item)
    {
        PlayerPrefs.SetInt(item, 1);
        Debug.Log("�����ռ�Ʒ��Ϣ" + item);
    }

    //����ռ�Ʒ��Ϣ
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
