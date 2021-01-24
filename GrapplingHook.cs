using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public bool isFlying = false;
    public Rigidbody rig;
    public Camera playerCamera;
    //public GameObject grapnel;
    const float maxDistance = 50;
    LayerMask wallLayer;
    Ray ray;
    RaycastHit hit;
    PlayerMove1.PlayerState state;
    void Start()
    {
        wallLayer = 1 << LayerMask.NameToLayer("Wall");
    }
    void Update()
    {
        //如果按下右键
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //发射射线检测墙体
            ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
            if (Physics.Raycast(ray, out hit, maxDistance, wallLayer))
            {
                isFlying = true;
                GetComponent<PlayerMove1>().state = PlayerMove1.PlayerState.Fly;
                //钩爪模型设置
                //grapnel.SetActive(true);
                //grapnel.transform.position = hit.point;
                Quaternion q = Quaternion.identity;
                q.SetLookRotation(hit.point - transform.position);
                //grapnel.transform.rotation = q;
            }
        }
        //如果与钩爪距离小于下限，或松开右键
        if (Vector3.Distance(hit.point, transform.position) < 3 || !Input.GetKey(KeyCode.Mouse1))
        {
            isFlying = false;
            //grapnel.SetActive(false);
        }
        //如果正在飞行途中
        else if (isFlying && Input.GetKey(KeyCode.Mouse1))
        {
            rig.velocity = Vector3.Lerp(rig.velocity, Vector3.Normalize(hit.point - transform.position) * 30, 0.015f);
        }
    }
}
