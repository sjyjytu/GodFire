using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponController : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform firePosition;
    public float bulletSpeed;
    AudioSource audioPlayer;
    [SerializeField]
    AudioClip shotClip;
    public Vector3 aimPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            audioPlayer = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            if (PhotonNetwork.IsConnected)
            {
                //GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
                //GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePosition.position, Quaternion.identity, 0);
                //使用对象池
                GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePosition.position, Quaternion.identity, 0);
                audioPlayer.clip = shotClip;
                audioPlayer.Play();
                //Vector3 direction = Quaternion.Euler(m_camTrans.eulerAngles) * Vector3.forward;
                bullet.GetComponent<Rigidbody>().velocity = bulletSpeed * (aimPoint - firePosition.position).normalized;
            }
        }
    }
}
