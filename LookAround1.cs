using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround1 : MonoBehaviour
{
    //1-1 Trigger脚本
    public GameObject A2, A3, A4, player, camera1;
    void OnTriggerEnter(Collider other)
    {
        A2.SetActive(true);
        A3.SetActive(true);
        A4.SetActive(true);
        player.GetComponent<PlayerMove1>().enabled = false;
        player.GetComponent<CrossHair>().enabled = false;
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        camera1.GetComponent<Level1Camera>().enabled = true;
        gameObject.SetActive(false);
    }
}
