using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerHomingMissiles))]
public class HellModAddon : MonoBehaviour
{
    private ChangeLightColor m_LightCol;

    void Awake()
    {
        m_LightCol = FindObjectOfType<ChangeLightColor>();
        if (m_LightCol)
        {
            m_LightCol.m_ToColor = Color.red;
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
