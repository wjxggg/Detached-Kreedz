using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    //脚本绑在检测点 Trigger上
    public GameObject teleport;//设置相应的传送点
    public GameObject player;
    void Start()
    {
        player = GameObject.Find("/Player");
    }
    void OnTriggerEnter(Collider other)
    {
        player.GetComponent<PlayerMove1>().teleport = teleport;
    }
}
