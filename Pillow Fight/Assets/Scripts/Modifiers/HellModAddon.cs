using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerHomingMissiles))]
public class HellModAddon : MonoBehaviour
{
    public Color m_ToColor = Color.red;

    private ChangeLightColor m_LightCol;

    void Awake()
    {
        m_LightCol = FindObjectOfType<ChangeLightColor>();
        if (m_LightCol)
        {
            m_LightCol.m_ToColor = m_ToColor;
            m_LightCol.ChangeColor();
        }
    }

    void OnDestroy()
    {
        if (m_LightCol)
        {
            m_LightCol.m_ToColor = Color.white;
            m_LightCol.ChangeColor();
        }
    }
}
