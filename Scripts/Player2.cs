using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Transform m_transform;
    CharacterController m_ch;
    float m_movSpeed = 5.0f;
    float m_gravity = 6.0f;
    public float m_life = 5;
    public float m_jumpSpeed = 8.0f;
    private Vector3 m_movDirection = Vector3.zero;


    void Start()
    {
        m_transform = transform;
        m_ch = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_life <= 0)
        {
            return;
        }
        Controll();
    }

    void Controll()
    {
        float zm = -Input.GetAxis("Horizontal") * m_movSpeed * Time.deltaTime;
        float xm = Input.GetAxis("Vertical") * m_movSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_movDirection.y = m_jumpSpeed;
        }
        m_movDirection.y -= m_gravity * Time.deltaTime;
        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, -m_gravity * Time.deltaTime, zm)));
    }
}
