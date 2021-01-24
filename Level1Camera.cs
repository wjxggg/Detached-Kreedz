using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Camera : MonoBehaviour
{
    //1-1 镜头控制
    public GameObject player;
    Vector3 v = new Vector3();
    public Material skybox;
    float lerp = 0;
    float timer = 9;
    Quaternion a;
    void Update()
    {
        lerp += Time.deltaTime;
        GetComponent<Camera>().backgroundColor = Color.Lerp(new Color(0, 0, 0, 0), new Color(15, 15, 15, 0), lerp / 500);
        //skybox.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(20, 20, 20, 0), lerp / 500);
        timer -= Time.deltaTime;
        if (timer > 2)
        {
            transform.position = Vector3.SmoothDamp(transform.position, transform.position + new Vector3(0, 15, -5), ref v, 3.5f);
            transform.LookAt(new Vector3(0, 50, 250));
            a = transform.rotation;
        }
        else if (timer > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + new Vector3(0, 1.65f, 0), ref v, 1f);
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            player.GetComponent<PlayerMove1>().enabled = true;
            player.GetComponent<CrossHair>().enabled = true;
            gameObject.GetComponent<Level1Camera>().enabled = false;
        }
    }
}
