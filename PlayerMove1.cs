using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove1 : MonoBehaviour
{
    [SerializeField]
    Rigidbody rig; //玩家自身的物理模块
    [SerializeField]
    CapsuleCollider collider1;

    LayerMask groundLayer; //地面层

    public enum PlayerState
    {
        Idle,
        Run,
        Duck,
        Slide,
        AirSlide,
        Fly,
    }
    public PlayerState state = PlayerState.Idle;
    public GameObject playerCamera;//获得玩家摄像机
    public float sensitivityMouse = 1f;//鼠标速度
    public bool isGrounded = false;//判断是否在地上
    public bool checkIsGrouned = true;//用于控制跳跃后一小段时间不检测地面
    public float GTimer = 0.3f;//上面的Timer
    public bool isHoldingSpace = false;//判断是否按了空格
    public float maxHoldingSpaceTIme = 0.2f;//空格持续时间
    private float holdingSpaceTimer = 0;//空格Timer
    public float maxSlidingTime = 0.7f;//滑铲不失速持续时间
    private float slidingTimer = 0;//滑铲Timer
    public float slideAddPower = 8;//滑铲加速度
    public float duckSpeed = 4;//蹲下速度
    public float Speed = 9;//地上速度
    public float jumpHeight = 5.5f;//跳跃高度
    public float jumpAddPower = 3;//起跳加速度
    public float airSpeed = 3;//空中加速度
    public float fallGravityMultiplier = 1.6f;//下落时重力倍率
    private float h, v;//玩家方向输入
    private float radius = 0.34f;//碰撞箱半径
    private float lastTickAngle;//上一帧的方向
    private float difBtAngles;//方向的差
    public bool moveToTeleport = false;//Trigger触发传送回检查点
    public GameObject teleport;

    Vector3 vel = new Vector3(0,0,0);//不知道有什么用

    void Start()
    {
        Cursor.visible = false;
        rig = GetComponent<Rigidbody>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        collider1 = GetComponent<CapsuleCollider>();
    }
    void Update()
    {
        h = Input.GetAxis("Horizontal"); //获取玩家在水平方向上的输入
        v = Input.GetAxis("Vertical");//获取玩家在垂直方向上的输入
        MouseControl();//鼠标控制视角
        CheckSpace();//检测空格
        CheckIsGrounded();//检测地面
        CheckHead();//检测头顶是否有墙
        DuckOrSlide();//蹲下和滑铲
        if (Input.GetKey(KeyCode.R) || moveToTeleport)//按R或玩家触发死亡区域Trigger
        {
            Teleport();//传送回检查点
        }
    }
    void FixedUpdate()
    {
        difBtAngles = Mathf.Abs(transform.eulerAngles.y - lastTickAngle) < 300 ? transform.eulerAngles.y - lastTickAngle : transform.eulerAngles.y - lastTickAngle > 0 ? transform.eulerAngles.y - lastTickAngle - 360 : transform.eulerAngles.y - lastTickAngle + 360;//计算这一帧与上一帧屏幕角度的差
        GravityCheck();//重力控制，跳跃手感调整
        Move();//地上，蹲下，空中移动
        Jump();//跳跃
        lastTickAngle = transform.eulerAngles.y;
    }

    //地面检测
    void CheckIsGrounded()
    {
        //如果checkIsGrouned
        if (checkIsGrouned)
        {
            var pointBottom = transform.position + new Vector3(0, 0.25f, 0);
            var pointTop = transform.position + new Vector3(0, 0.4f, 0);
            //创建一个碰撞箱，检测下方是否碰到Ground层，实现地面检测
            Collider[] collider = Physics.OverlapCapsule(pointBottom, pointTop, radius * 0.9f, groundLayer.value);
            if (collider.Length != 0)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
                //设置state
                if (state != PlayerState.AirSlide) state = PlayerState.Fly;
            }
        }
        //否则
        else
        {
            //这一段用于控制跳跃后一段时间不检测地面，避免许多Bug
            isGrounded = false;
            GTimer -= Time.deltaTime;
            //设置state
            if (state != PlayerState.AirSlide)
            {
                state = PlayerState.Fly;
            }
            //计时器结束，方可继续检测地面
            if (GTimer < 0)
            {
                GTimer = 0.3f;
                checkIsGrouned = true;
            }
        }
    }

    //检测蹲下时碰头
    void CheckHead()
    {
        if ((state == PlayerState.Duck || state == PlayerState.Slide || state == PlayerState.Idle || state == PlayerState.Fly) && !Input.GetKey(KeyCode.LeftControl))
        {
            var pointBottom = transform.position + new Vector3(0, 0.4f, 0);
            var pointTop = transform.position + new Vector3(0, 1.4f, 0);
            //创建碰撞箱检测头上
            Collider[] collider = Physics.OverlapCapsule(pointBottom, pointTop, radius * 0.9f, groundLayer.value);
            if (collider.Length != 0)//如果有墙
            {
                state = state == PlayerState.Slide ? PlayerState.Slide : PlayerState.Duck;
            }
            else
            {
                state = state == PlayerState.Slide ? PlayerState.Slide : PlayerState.Idle;
            }
        }
    }

    //地上，空中移动
    void Move()
    {
        //地上移动
        if (isGrounded && state != PlayerState.Duck && state != PlayerState.Slide && state != PlayerState.AirSlide)
        {
            rig.velocity = transform.TransformVector(h * Speed, rig.velocity.y, v * Speed);
            //设置state
            if (Mathf.Abs(rig.velocity.x) + Mathf.Abs(rig.velocity.z) > 3)
            {
                state = PlayerState.Run;
            }
            else
            {
                state = PlayerState.Idle;
            }
        }
        //蹲下移动
        else if (state == PlayerState.Duck)
        {
            rig.velocity = transform.TransformVector(h * duckSpeed, rig.velocity.y, v * duckSpeed);
            //设置摄像机和碰撞箱
            playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 0.75f, ref vel, 0.1f);
            collider1.height = 0.8f;
            collider1.center = Vector3.up * collider1.height / 2;
        }
        //空中移动
        else if (!isGrounded && v < 0)
        {
            //空中按S减速
            if (transform.TransformVector(rig.velocity).z > -5)
            {
                rig.AddRelativeForce(0, 0, -Speed);
            }
            if (transform.TransformVector(rig.velocity).z > 5)
            {
                rig.velocity = Vector3.Lerp(rig.velocity, new Vector3(0, rig.velocity.y, 0), 0.07f);
            }
        }
        else if (!isGrounded && v > 0)
        {
            //给玩家向量增加角度，实现空中旋转
            rig.velocity = new Vector3(RotationMatrix(new Vector3(rig.velocity.x, 0, rig.velocity.z), difBtAngles).x, rig.velocity.y, RotationMatrix(new Vector3(rig.velocity.x, 0, rig.velocity.z), difBtAngles).z);
            //如果速度小于上限
            if (transform.InverseTransformVector(rig.velocity).z < 30)
            {
                //增加airSpeed * (difBtAngles 与 2的最小值)
                rig.AddRelativeForce(0, 0, airSpeed * Mathf.Min(Mathf.Abs(difBtAngles), 2));
            }
            if (transform.InverseTransformVector(rig.velocity).z < 5)
            {
                //增加airSpeed * (difBtAngles 与 2的最小值)
                rig.AddRelativeForce(0, 0, airSpeed);
            }
        }
    }

    //跳跃
    void Jump()
    {
        if (isHoldingSpace && isGrounded)
        {
            //普通跳跃
            if (state == PlayerState.Run || state == PlayerState.Idle || state == PlayerState.Duck)
            {
                //高度增加0.5
                rig.velocity = new Vector3(rig.velocity.x, jumpHeight + 0.5f, rig.velocity.z);
                //如果速度小于上限
                if (transform.InverseTransformDirection(rig.velocity).z < 10)
                {
                    var z = transform.InverseTransformVector(rig.velocity).z;
                    var addZ = z == 0 ? 0 : z > 4 ? jumpAddPower : 0;
                    //速度增加addZ * 目前速度 / Speed
                    //这样可以让玩家更好控制速度
                    rig.velocity += transform.TransformVector(0, 0, addZ * (transform.InverseTransformVector(rig.velocity).z / Speed));
                }
            }
            //滑铲跳跃
            else if (state == PlayerState.Slide)
            {
                //高度不增加
                rig.velocity = new Vector3(rig.velocity.x, jumpHeight, rig.velocity.z);
                //同上
                if (transform.InverseTransformDirection(rig.velocity).z < 16)
                {
                    var z = transform.InverseTransformVector(rig.velocity).z;
                    var addZ = z == 0 ? 0 : z > 0 ? jumpAddPower : 0;
                    rig.velocity += transform.TransformVector(0, 0, addZ);
                }
            }
            isHoldingSpace = false;
            isGrounded = false;
            //起跳后一段时间不检测地面的设置
            checkIsGrouned = false;
        }
    }

    //检测空格，假如落地前0.5秒按下空格，落地后也可跳跃
    void CheckSpace()
    {
        //如果Timer结束，则重置
        if (holdingSpaceTimer < 0)
        {
            holdingSpaceTimer = maxHoldingSpaceTIme;
            isHoldingSpace = false;
            return;
        }
        else
        {
            //如果按下空格，则Timer开始计时
            if (Input.GetKeyDown(KeyCode.Space) && !isHoldingSpace)
            {
                isHoldingSpace = true;
                holdingSpaceTimer = maxHoldingSpaceTIme;
                return;
            }
            //如果正在计时
            else if (isHoldingSpace)
            {
                holdingSpaceTimer -= Time.deltaTime;
                return;
            }
        }
    }

    //控制重力
    void GravityCheck()
    {
        //加快下落速度，不然跳跃手感很飘
        if (rig.velocity.y < 4 && rig.velocity.y >= -25) rig.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
        //下落速度限制
        else if (rig.velocity.y < -25) rig.velocity = new Vector3(rig.velocity.x, -25, rig.velocity.z);
    }

    //鼠标控制转向
    void MouseControl()
    {
        //鼠标X坐标控制模型Y轴
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityMouse, 0, Space.World);
        //鼠标Y坐标控制摄像机X轴
        playerCamera.transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivityMouse, 0, 0);
    }

    //蹲下或滑铲
    void DuckOrSlide()
    {
        //判断如果前进速度大于5，或者AirSlide状态碰到地面,则开始滑铲
        if ((Input.GetKeyDown(KeyCode.LeftControl) && transform.InverseTransformDirection(rig.velocity).z > 5 && isGrounded && state != PlayerState.Slide) || (state == PlayerState.AirSlide && isGrounded))
        {
            state = PlayerState.Slide;
            slidingTimer = maxSlidingTime;
            //摄像机设置
            playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 0.55f, ref vel, 0.1f);
            //碰撞箱设置
            collider1.height = 0.6f;
            collider1.center = Vector3.up * collider1.height / 2;
            //如果速度小于上限
            if (Mathf.Abs(transform.InverseTransformDirection(rig.velocity).z) < 14)
            {
                //X轴速度设为0，增加slideAddPower
                rig.velocity = transform.TransformVector(0, rig.velocity.y, transform.InverseTransformVector(rig.velocity).z + slideAddPower);
            }
            else
            {
                //X轴速度设为0
                rig.velocity = transform.TransformVector(0, rig.velocity.y, transform.InverseTransformVector(rig.velocity).z);
            }
            return;
        }
        //空中滑铲不加速
        else if (Input.GetKeyDown(KeyCode.LeftControl) && Mathf.Abs(transform.InverseTransformDirection(rig.velocity).z) > 5 && state != PlayerState.Slide && state != PlayerState.Duck && !isGrounded)
        {
            state = PlayerState.AirSlide;
            //摄像机设置
            playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 0.55f, ref vel, 0.1f);
            //碰撞箱设置
            collider1.height = 0.6f;
            collider1.center = Vector3.up * collider1.height / 2;
            //X轴速度设为0
            rig.velocity = transform.TransformVector(0, rig.velocity.y, transform.InverseTransformVector(rig.velocity).z);
        }
        //判断滑铲状态
        else if (state == PlayerState.Slide)
        {
            playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 0.55f, ref vel, 0.1f);
            slidingTimer -= Time.deltaTime;
            //如果离开地面，则退出滑铲状态
            if (!isGrounded)
            {
                state = PlayerState.Idle;
                return;
            }
            //如果松开Ctrl
            else if (!Input.GetKey(KeyCode.LeftControl))
            {
                state = PlayerState.Idle;
                return;
            }
            //如果滑铲速度小于3，则退出滑铲状态
            else if (Mathf.Abs(transform.InverseTransformDirection(rig.velocity).z) < 3)
            {
                state = PlayerState.Duck;
                return;
            }
            //如果Timer结束
            else if (slidingTimer < 0)
            {
                //滑铲速度减慢
                rig.velocity = transform.TransformVector(new Vector3(0, rig.velocity.y, Mathf.Lerp(0, transform.InverseTransformVector(rig.velocity).z, 0.05f)));
                return;
            }
            return;
        }
        //判断空中滑铲状态
        else if (state == PlayerState.AirSlide)
        {
            playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 0.55f, ref vel, 0.1f);
            //如果松开Ctrl
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                state = PlayerState.Idle;
                return;
            }
            return;
        }
        //如果速度不够滑铲，并且在地上，则蹲下
        else if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            state = PlayerState.Duck;
            return;
        }
        //重置Timer
        else if(state != PlayerState.Duck)
        {
            ResetSlide();
        }
    }
    void ResetSlide()
    {
        slidingTimer = maxSlidingTime;
        playerCamera.transform.position = Vector3.SmoothDamp(playerCamera.transform.position, transform.position + Vector3.up * 1.65f, ref vel, 0.1f);
        collider1.height = 1.7f;
        collider1.center = Vector3.up * collider1.height / 2;
        return;
    }

    //改变向量的方向
    Vector3 RotationMatrix(Vector3 v, float angle)
    {
        var x = v.x;
        var z = v.z;
        var sin = Mathf.Sin(Mathf.PI * angle / 180);
        var cos = Mathf.Cos(Mathf.PI * angle / 180);
        var newX = x * cos + z * sin;
        var newZ = x * -sin + z * cos;
        return new Vector3((float)newX, 0, (float)newZ);
    }

    //传送回起点
    public void Teleport()
    {
        rig.velocity = new Vector3(0, 0, 0);
        transform.position = teleport.transform.position;
        transform.eulerAngles = teleport.transform.eulerAngles;
        moveToTeleport = false;
    }
}
