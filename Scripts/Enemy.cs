using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class Enemy : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    Transform targetPlayerTransform;
    NavMeshAgent nav;
    float movSpeed = 2.5f;
    float m_life = 5.0f;
    Animator animator;

    float sqrAttackDis = 9.0f;
    float sqrRunDis = 10000;
    bool isAlive = true;

    [SerializeField]
    float damage = 0.1f;


    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.speed = movSpeed;
        
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float sqrDistance = float.MaxValue;
        if (targetPlayerTransform == null)
        {
            targetPlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            sqrDistance = (transform.position - targetPlayerTransform.position).sqrMagnitude;
        }
        
        nav.isStopped = false;
        if (!isAlive)
        {
            return;
        }
        else if (m_life <= 0)
        {
            animator.SetBool("death", true);
            animator.SetBool("idle", false);
            animator.SetBool("run", false);
            animator.SetBool("attack", false);
            //TODO: 销毁对象
            isAlive = false;
            Destroy(this.gameObject, 3);
        }
        else if (sqrDistance <= sqrAttackDis)
        {
            animator.SetBool("attack", true);
            animator.SetBool("idle", false);
            animator.SetBool("run", false);
            PhotonView target = targetPlayerTransform.gameObject.GetComponent<PhotonView>();
            target.RPC("TakeDamage", RpcTarget.All, damage, target.ViewID);
        }
        else if (sqrDistance <= sqrRunDis)
        {
            animator.SetBool("run", true);
            animator.SetBool("idle", false);
            animator.SetBool("attack", false);
            nav.SetDestination(targetPlayerTransform.position);
        }
        else
        {
            animator.SetBool("idle", true);
            animator.SetBool("run", false);
            animator.SetBool("attack", false);
            nav.isStopped = true;
        }
    }

    public void TakeDamage(float damage)
    {
        m_life -= damage;
        Debug.Log(m_life);
    }
}
