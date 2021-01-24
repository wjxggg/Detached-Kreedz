using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticBlock : MonoBehaviour
{
    //弹力板
    public float power = 10;
    public GameObject player;
    void Start()
    {
        player = GameObject.Find("/Player");
    }
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rig = other.GetComponent<Rigidbody>();
        rig.velocity = new Vector3(rig.velocity.x, power, rig.velocity.z);
    }
}
