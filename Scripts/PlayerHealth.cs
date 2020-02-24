using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health;
    public float startHealth;
    public string username;
    [SerializeField]
    Text usernameText;
    [SerializeField]
    Slider hpSlider;
    

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        if (photonView.IsMine)
        {
            username = PlayerPrefs.GetString("username");
            Debug.Log("load username: " + username);
            usernameText.text = username;
            PlayerPrefs.DeleteKey("username");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void TakeDamage(float damage, int targetID)
    {
        if (photonView.IsMine)
        {
            health -= damage;
            hpSlider.value = health;
            Debug.Log("你中子弹了，血量：" + health);
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(username);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            username = (string)stream.ReceiveNext();
            usernameText.text = username;
        }
    }

    void Die()
    {
        Debug.Log("你挂了！");
        this.gameObject.SetActive(false);
        health = 100;
        hpSlider.value = health;
        //重生
        System.Random rd = new System.Random();
        int i = rd.Next(0, SpawnPoints.positions.Length - 1);
        Debug.Log("现在位置："+transform.position);
        Debug.Log("重生地点："+i+", 位置："+ SpawnPoints.positions[i].position);
        transform.position = SpawnPoints.positions[i].position;
        this.gameObject.SetActive(true);
    }
}
