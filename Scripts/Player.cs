using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviourPunCallbacks
{

    //把运动相关的参数，独立出来
    [System.Serializable]
    public class MoveSetting
    {
        public float ForwarSpeed = 5f;
        public float BackSpeed = 3f;
        public float HorizonSpeed = 4f;

        public float RunValue = 2f;
        public float JumpForce = 200f;
    }
    //把视角相关的独立出来
    [System.Serializable]
    public class MouseLook
    {
        public float XSensitive = 2f;
        public float YSensitive = 2f;
    }

    [SerializeField] MoveSetting moveSet;
    [SerializeField] MouseLook CameraSet;

    //当前速度
    private float currentSpeed;
    //一段跳
    private bool m_jump;
    //二段跳
    private bool m_jump2;

    //第一人称，胶囊碰撞
    private CapsuleCollider m_capsule;
    //第一人称，刚体
    private Rigidbody m_rigidbody;

    private Camera m_camera;
    //相机的Transform（减少Update中transform的调用）
    private Transform m_camTrans;
    //主角的Transform
    private Transform m_chaTrans;

    ////摄像机欧拉角
    //private Vector3 m_camRotate;
    ////主角的欧拉角
    //private Vector3 m_chaRotate;

    //摄像机旋转四元数
    private Quaternion m_camQutation;
    //主角的旋转四元数
    private Quaternion m_chaQutation;

    //爬坡的速度曲线
    public AnimationCurve SlopCurve;

    //是否在地面上
    private bool m_isOnGround;
    //地面法线向量
    private Vector3 curGroundNormal;

    // Use this for initialization
    void Start()
    {
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_capsule = GetComponent<CapsuleCollider>();
            m_rigidbody = GetComponent<Rigidbody>();

            m_camera = Camera.main;

            m_camera.gameObject.transform.parent = gameObject.transform;
            m_camera.transform.localPosition = new Vector3(0.5f, 0.8f, 0);

            m_camTrans = m_camera.transform;
            m_chaTrans = transform;

            //初始化参数
            m_camQutation = m_camTrans.rotation;
            m_chaQutation = m_chaTrans.rotation;
            // 人物下落速度快一些
            Physics.gravity = new Vector3(0, -10, 0);  // gravity= -35 其他的默认
        }
    }

    private bool lockView = false;
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //视角转动
            if (!lockView)
            {
                RotateView();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                lockView = !lockView;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_jump = true;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    //物理的运动，需要放到FixedUpdate中，固定帧率0.02秒，可在Edit.time中修改
    void FixedUpdate()
    {
        DoMove();
    }

    //视图的旋转
    void RotateView()
    {
        float xRot = Input.GetAxis("Mouse Y") * CameraSet.YSensitive;
        float yRot = Input.GetAxis("Mouse X") * CameraSet.XSensitive;

        //欧拉角使用
        //{
        //    m_camRotate += new Vector3(-xRot, 0f, 0f);
        //需要LocalEulerAnglers，否则摄像机和胶囊体会同时对相机旋转起作用
        //    m_camTrans.localEulerAngles = m_camRotate;
        //    m_camRotate += new Vector3(0f, yRot, 0f);
        //    m_chaTrans.localEulerAngles = m_chaRotate;
        //}

        //四元数使用
        {
            //m_camQutation *= Quaternion.Euler(-xRot, 0f, 0f);
            m_camQutation *= Quaternion.Euler(-xRot, 0, 0f);
            //限制旋转角度在【-90，90】内
            m_camQutation = ClampRotation(m_camQutation);
            m_camTrans.localRotation = m_camQutation;
            m_chaQutation *= Quaternion.Euler(0f, yRot, 0f);
            m_chaTrans.localRotation = m_chaQutation;
        }
    }

    void DoMove()
    {
        //检测地面
        CheckGround();
        //检测前后左右输入
        Vector2 input = GetInput();
        //更新当前速度，根据移动方向
        CaculateSpeed(input);

        //判断是否有移动的速度，没有就不给刚体施加力
        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon)/* && m_isOnGround*/)
        {
            //计算方向力
            Vector3 desireMove = m_camTrans.forward * input.y + m_camTrans.right * input.x;
            
            //力在地面投影的向量的（单位向量）
            desireMove = Vector3.ProjectOnPlane(desireMove, curGroundNormal).normalized;
            desireMove.x = desireMove.x * currentSpeed;
            desireMove.y = 0;
            desireMove.z = desireMove.z * currentSpeed;

            //当前速度不能大于规定速度（Magnitude方法，需要开平方根，使用sqr节省运算）
            if (m_rigidbody.velocity.sqrMagnitude < currentSpeed * currentSpeed)
            {
                //给刚体施加（坡度计算后）的力
                m_rigidbody.AddForce(desireMove * SlopeValue(), ForceMode.Impulse);
            }
        }


        if (m_isOnGround)
        {
            m_rigidbody.drag = 5f;
            jumpTime = 0;
            //一段跳
            if (m_jump)
            {
                JumpUp();
            }
        }
        else
        {
            if (m_jump)
            {
                jumpTime++;
                //二段跳
                if (jumpTime < 2)
                {
                    JumpUp();
                }
            }
        }

        m_jump = false;
    }
    int jumpTime = 0;

    //跳跃方法
    void JumpUp()
    {
        m_rigidbody.drag = 0f;
        //把刚体的上下方向的速度先归零
        m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0f, m_rigidbody.velocity.z);
        m_rigidbody.AddForce(new Vector3(0, moveSet.JumpForce, 0), ForceMode.Impulse);
    }

    //检测方向键输入
    Vector2 GetInput()
    {
        Vector2 input = new Vector2
            (
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            );
        //float horValue = Input.GetAxis("Horizontal");
        //float verValue = Input.GetAxis("Vertical");
        return input;
    }

    //计算 速度
    void CaculateSpeed(Vector2 _input)
    {
        currentSpeed = moveSet.ForwarSpeed;

        //横向移动
        if (Mathf.Abs(_input.x) > float.Epsilon)
        {
            currentSpeed = moveSet.HorizonSpeed;
        }
        //前进后退
        else if (_input.y < 0)
        {
            currentSpeed = moveSet.BackSpeed;
        }
        //Shift跑加速
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= moveSet.RunValue;
        }
        //Ctrl下蹲减速
    }

    //爬坡参数
    float SlopeValue()
    {
        float angle = Vector3.Angle(curGroundNormal, Vector3.up);
        float value = SlopCurve.Evaluate(angle);
        return value;
    }

    //检测地面
    void CheckGround()
    {
        RaycastHit hit;

        //球形碰撞检测（第9个方法）
        if (Physics.SphereCast(m_capsule.transform.position, m_capsule.radius, Vector3.down, out hit, ((m_capsule.height / 2 - m_capsule.radius) + 0.01f)))
        {
            //获取碰撞位置的发现向量
            curGroundNormal = hit.normal;
            m_isOnGround = true;
        }
        else
        {
            curGroundNormal = Vector3.up;
            m_isOnGround = false;
        }
    }

    void CheckBuffer()
    {
        RaycastHit hit;
        float speed = m_rigidbody.velocity.y;
        if (speed < 0)
        {
            if (Physics.SphereCast(m_capsule.transform.position, m_capsule.radius, Vector3.down, out hit, ((m_capsule.height / 2 - m_capsule.radius) + 1f)))
            {
                speed *= 0.5f;
                m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, speed, m_rigidbody.velocity.z);
            }
        }
    }

    //四元数俯角，仰角限制
    Quaternion ClampRotation(Quaternion q)
    {
        //四元数的xyzw，分别除以同一个数，只改变模，不改变旋转
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1;

        /*给定一个欧拉旋转(X, Y, Z)（即分别绕x轴、y轴和z轴旋转X、Y、Z度），则对应的四元数为
x = sin(Y/2)sin(Z/2)cos(X/2)+cos(Y/2)cos(Z/2)sin(X/2)
y = sin(Y/2)cos(Z/2)cos(X/2)+cos(Y/2)sin(Z/2)sin(X/2)
z = cos(Y/2)sin(Z/2)cos(X/2)-sin(Y/2)cos(Z/2)sin(X/2)
w = cos(Y/2)cos(Z/2)cos(X/2)-sin(Y/2)sin(Z/2)sin(X/2)
         */

        //得到推导公式[欧拉角x=2*Aan(q.x)]
        float angle = 2 * Mathf.Rad2Deg * Mathf.Atan(q.x);
        //限制速度
        angle = Mathf.Clamp(angle, -90f, 90f);
        //反推出q的新x的值
        q.x = Mathf.Tan(Mathf.Deg2Rad * (angle / 2));

        return q;
    }

    void setPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
