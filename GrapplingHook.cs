using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public bool isFlying = false;
    public Rigidbody rig;
    float timer = 1;
    public Camera playerCamera;
    //public GameObject grapnel;
    const float maxDistance = 50;
    private float distance;
    public LayerMask wallLayer;
    Ray ray;
    RaycastHit hit;
    PlayerMove1.PlayerState state;
    LineRenderer lr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
       
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
                rig.velocity += Vector3.Normalize(hit.point - transform.position)*8;
                distance = hit.distance;
                isFlying = true;
                GetComponent<PlayerMove1>().state = PlayerMove1.PlayerState.Fly;
                //钩爪模型设置
                //grapnel.SetActive(true);
                //grapnel.transform.position = hit.point;
                Quaternion q = Quaternion.identity;
                q.SetLookRotation(hit.point - transform.position);
                //grapnel.transform.rotation = q;
                lr.enabled = true;
            }
        }
        //如果与钩爪距离小于下限，或松开右键
        if (Vector3.Distance(hit.point, transform.position) < 3 || !Input.GetKey(KeyCode.Mouse1))
        {
            isFlying = false;
            //grapnel.SetActive(false);
        }
    }
    void FixedUpdate()
    {
        if (isFlying && Input.GetKey(KeyCode.Mouse1))
        {
            timer += Time.deltaTime;
            Vector3 v3 = -Vector3.Normalize(Vector3.Cross(hit.point - transform.position, transform.right));
            if (Vector3.SignedAngle(v3, transform.forward, transform.right) > 120)
            {
                isFlying = false;
            }
         
            else if(timer>=1.2f)
            {
                v3 = Vector3.Normalize(Vector3.Lerp(v3,  transform.forward, 45 / Vector3.Angle(v3, transform.forward)));
                print(Vector3.SignedAngle(v3, transform.forward, transform.right));
                if (Vector3.SignedAngle(v3, transform.forward, transform.right)<-30)
                {
                    v3 = Vector3.Lerp(Vector3.Normalize(hit.point - transform.position),v3 , Mathf.Abs( Vector3.SignedAngle(v3, transform.forward, transform.right))/120);
                }
                //print(Vector3.SignedAngle(v3, transform.forward, transform.right));
                //
                rig.velocity = Vector3.Lerp(rig.velocity, v3 * Mathf.Lerp(1, 20, timer / 3) + Vector3.Lerp(Vector3.Normalize(hit.point - transform.position), Vector3.Normalize(hit.point - transform.position) * 5, timer / 5), timer / 3);

            }
        }
        else
        {
            lr.enabled = false;
            timer = 1;
        }
    }
    void LateUpdate()
    {
        if (isFlying)
        {
            lr.SetPosition(0, transform.position + Vector3.up);
            lr.SetPosition(1, hit.point);
        }
    }

    //Vector3 RotationMatrix(Vector3 v, float angle)
    //{
    //    var y = v.y;
    //    var z = v.z;
    //    var sin = Mathf.Sin(Mathf.PI * angle / 180);
    //    var cos = Mathf.Cos(Mathf.PI * angle / 180);
    //    var newY = y * cos + z * sin;
    //    var newZ = y * -sin + z * cos;
    //    return new Vector3(0, (float)newY, (float)newZ);
    //}
}
