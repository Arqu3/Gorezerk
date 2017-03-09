using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ChangeColorTimer : MonoBehaviour
{
    [Header("Time to change color")]
    [Range(0.0f, 100.0f)]
    public float m_Time = 5.0f;

    //Component vars
    private MeshRenderer m_Renderer;
    private Color m_BaseColor;

    void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_BaseColor = m_Renderer.material.color;
    }

    void Start()
    {
        StartCoroutine(ChangeColor());
    }

    void OnEnable()
    {
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        m_Renderer.material.color = new Color(Random.value, Random.value, Random.value);
        yield return new WaitForSeconds(m_Time);
        //StartCoroutine(ChangeColor());
        m_Renderer.material.color = m_BaseColor;
        enabled = false;
    }
}
