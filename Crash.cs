using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crash : MonoBehaviour
{
    //脚本绑在可破坏物体上
    public Object crashingBox;//碎片预制体
    public GameObject collider1;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == collider1)
        {
            Instantiate(crashingBox, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
