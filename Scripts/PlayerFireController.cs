using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerFireController : MonoBehaviourPunCallbacks
{
    public WeaponController weapon;
    public float fireInterval;
    float fireTimer;

    public static GameObject playerInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            fireTimer = fireInterval;
            playerInstance = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            fireTimer += Time.deltaTime;

            #region 射击
            if (weapon != null)
            {
                if (Input.GetMouseButton(0) && fireTimer >= fireInterval)
                {
                    #region 获取瞄准方向
                    Ray ray = new Ray(Camera.main.gameObject.transform.position, Camera.main.gameObject.transform.forward);
                    Debug.DrawRay(Camera.main.gameObject.transform.position, Camera.main.gameObject.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        weapon.aimPoint = hit.point;
                    }
                    else
                    {
                        weapon.aimPoint = Camera.main.gameObject.transform.position + Camera.main.gameObject.transform.forward * 100;
                    }
                    #endregion

                    weapon.Fire();
                    fireTimer = 0;
                }
            }
            #endregion
        }
    }
}
