using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBlock : MonoBehaviour
{
    GameObject timeBlock;
    const float maxTime = 0.4f;
    float timer = 0;
    Material material1;
    void Start()
    {
        timeBlock = transform.parent.gameObject;
        material1 = timeBlock.GetComponent<MeshRenderer>().material;
    }
    void OnTriggerStay(Collider other)
    {
        timer += Time.deltaTime;
        material1.color = Color32.Lerp(new Color32(255, 128, 0, 255), new Color32(255, 0, 0, 255), timer / 0.5f);
        if (timer > maxTime)
        {
            timeBlock.GetComponent<MeshCollider>().isTrigger = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        timer = 0;
        material1.color = new Color32(255, 128, 0, 255);
        timeBlock.GetComponent<MeshCollider>().isTrigger = false;
    }
}
