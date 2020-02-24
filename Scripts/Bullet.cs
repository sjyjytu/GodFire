using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float damage = 2.0f;
    public float lifetime = 5.0f;
    float life = 0;
    Vector3 flyDirection;
    public GameObject hitPeopleEffect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            life += Time.deltaTime;
            if (life > lifetime)
            {
                DestroyBullet();
                return;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            //只有开枪的人会执行这里的内容，可以看到飙血啥的
            if (collision.collider.tag == "Enemy")
            {
                collision.collider.GetComponent<Enemy>().TakeDamage(damage);
                GameObject effect = GameObject.Instantiate(hitPeopleEffect, transform.position, Quaternion.identity);
                Destroy(effect, 3);
            }
            if (collision.collider.tag == "Player")
            {
                //TODO: 特效没用Photon
                //collision.collider.GetComponent<PlayerHealth>().TakeDamage(damage);
                PhotonView target = collision.collider.GetComponent<PhotonView>();
                target.RPC("TakeDamage", RpcTarget.All, damage, target.ViewID);
                GameObject effect = GameObject.Instantiate(hitPeopleEffect, transform.position, Quaternion.identity);
                Destroy(effect, 3);
            }
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        life = 0;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        PhotonNetwork.Destroy(this.gameObject);
    }
}
