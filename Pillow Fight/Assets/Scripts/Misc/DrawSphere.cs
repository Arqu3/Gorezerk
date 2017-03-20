using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawSphere : MonoBehaviour
{
    public float m_Radius = 2.0f;
    public Color m_Color = Color.yellow;

    void OnDrawGizmos()
    {
        Gizmos.color = m_Color;
        Gizmos.DrawSphere(transform.position, m_Radius);
    }
}
