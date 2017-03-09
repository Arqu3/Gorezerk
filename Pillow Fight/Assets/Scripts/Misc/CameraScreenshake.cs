using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScreenshake : MonoBehaviour
{
    [Header("General screenshake settings")]
    public float m_ShakeInterval = 0.1f;
    public float m_ShakeAmount = 10.0f;
    public float m_ShakeTime = 1.0f;

    //Position vars
    private Vector3 m_StartPos;
    private bool m_IsShake = false;

    private float m_ShakeTimer = 0.0f;
    private float m_ShakeIntervalTimer = 0.0f;

    void Awake()
    {
        m_StartPos = transform.position;
    }

    public void StartShake()
    {
        m_IsShake = true;
    }

    void Update()
    {
        if (m_IsShake)
        {
            m_ShakeTimer += Time.deltaTime;
            m_ShakeIntervalTimer += Time.deltaTime;

            if (m_ShakeIntervalTimer >= m_ShakeInterval)
            {
                Vector3 pos = transform.position;
                pos.x += Random.Range(-m_ShakeAmount, m_ShakeAmount);
                pos.y += Random.Range(-m_ShakeAmount, m_ShakeAmount);

                transform.position = pos;

                m_ShakeIntervalTimer = 0.0f;
            }

            if (m_ShakeTimer >= m_ShakeTime)
            {
                m_ShakeTimer = 0.0f;
                m_ShakeIntervalTimer = 0.0f;
                transform.position = m_StartPos;
                m_IsShake = false;
            }
        }
    }
}
