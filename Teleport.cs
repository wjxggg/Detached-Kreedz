using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    //脚本绑在死亡区域上
    public GameObject player;
    void Start()
    {
        player = GameObject.Find("/Player");
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<PlayerMove1>().moveToTeleport = true;
        }
    }
}
