using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleavingCollider : MonoBehaviour
{
    GameObject camera1;
    void Start()
    {
        camera1 = GameObject.Find("PlayerCamera");
    }
    void Update()
    {
        transform.rotation = camera1.transform.rotation;
    }
}
