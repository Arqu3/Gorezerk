using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateConstant : MonoBehaviour
{
    //Public vars
    public Vector3 m_RotationAxis = Vector3.zero;
    public float m_RotationSpeed = 10.0f;

    void Update()
    {
        transform.Rotate(m_RotationAxis * m_RotationSpeed * Time.deltaTime);
    }
}
