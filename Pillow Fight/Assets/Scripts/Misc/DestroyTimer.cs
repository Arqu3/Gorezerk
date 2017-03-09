using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [Header("Destroy after time in seconds")]
    [Range(0.0f, 100.0f)]
    public float m_DestroyTime = 0.5f;

    void Awake()
    {
        Destroy(gameObject, m_DestroyTime);
    }
}
