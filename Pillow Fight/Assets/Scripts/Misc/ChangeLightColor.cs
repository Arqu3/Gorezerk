using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ChangeLightColor : MonoBehaviour
{
    public Color m_ToColor = Color.white;
    [Range(0.1f, 10000.0f)]
    public float m_ChangeTime = 3.0f;

    //Component vars
    private Light m_Light;

    //Change vars
    private bool m_ChangeColor = false;
    private float m_ChangeTimer = 0.0f;

    void Awake()
    {
        m_Light = GetComponent<Light>();
    }

    void Update()
    {
        if (m_ChangeColor)
        {
            m_ChangeTimer += Time.deltaTime;

            m_Light.color = Color.Lerp(m_Light.color, m_ToColor, m_ChangeTime * Time.deltaTime);

            if (m_ChangeTimer >= m_ChangeTime * 2.0f)
            {
                m_ChangeTimer = 0.0f;
                m_ChangeColor = false;
            }
        }
    }

    public void ChangeColor()
    {
        m_ChangeColor = true;
        m_ChangeTimer = 0.0f;
    }
}
