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

    private Color m_ToColor;

    void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_BaseColor = m_Renderer.material.color;
        m_ToColor = m_BaseColor;
    }

    void Update()
    {
        m_Renderer.material.color = Color.Lerp(m_Renderer.material.color, m_ToColor, 10.0f * Time.deltaTime);
    }

    public void ChangeColor(Color col)
    {
        StartCoroutine(SetColor(col));
    }

    private IEnumerator SetColor(Color col)
    {
        //m_Renderer.material.color = col;
        m_ToColor = col;
        yield return new WaitForSeconds(m_Time);
        //m_Renderer.material.color = m_BaseColor;
        m_ToColor = m_BaseColor;
    }
}
