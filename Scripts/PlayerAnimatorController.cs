using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAnimatorController : MonoBehaviourPunCallbacks
{

    #region Required Components
    //玩家地动画控制机
    public Animator playerAnim;
    #endregion

    #region Private Field
    /// <summary>
    /// 玩家键盘输入的向量形式
    /// </summary>
    Vector2 userInputVect;
    float h;
    float v;
    #endregion

    #region Serialize Field
    [Tooltip("玩家的移动动画平滑过渡")]
    [SerializeField]
    float smooth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            #region 获取玩家移动方向向量（键盘）并写入动画机
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
            //向量化一，并且Lerp
            if (Input.GetKey(KeyCode.LeftShift))
            {
                userInputVect = Vector2.Lerp(userInputVect, new Vector2(h, v).normalized * 2, Time.deltaTime * smooth);
            }
            else
            {
                userInputVect = Vector2.Lerp(userInputVect, new Vector2(h, v).normalized, Time.deltaTime * smooth);
            }
            //写入动画机
            playerAnim.SetFloat("MovX", userInputVect.x);
            playerAnim.SetFloat("MovY", userInputVect.y);
            #endregion
        }
    }
}