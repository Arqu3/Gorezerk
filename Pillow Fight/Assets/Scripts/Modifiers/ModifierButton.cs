using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModifierButton : MonoBehaviour
{
    //Public vars
    public GameObject m_Modifier;
    public Color m_EnabledColor = Color.green;
    public Color m_DisabledColor = Color.red;

    //Onclick vars
    private bool m_Active = false;
    private GameObject m_Instance = null;

    //Component vars
    ControllerScene m_Controller;
    Image m_Image;

    void Awake()
    {
        if (!m_Modifier)
        {
            Debug.Log("Modifier button script needs an associated modifier!");
            enabled = false;
            return;
        }
        if (!m_Modifier.GetComponent<Modifier>())
        {
            Debug.Log("Modifier button prefab does not contain a modifier script!");
            enabled = false;
            return;
        }

        m_Controller = FindObjectOfType<ControllerScene>();
        m_Image = GetComponent<Image>();
        m_Image.color = m_DisabledColor;
    }

    public void OnClick()
    {
        m_Active = !m_Active;

        if (m_Active)
        {
            m_Instance = (GameObject)Instantiate(m_Modifier, Vector3.zero, Quaternion.identity);
            m_Instance.transform.SetParent(m_Controller.transform);
            m_Controller.AddModifier(m_Instance.GetComponent<Modifier>());
            m_Image.color = m_EnabledColor;
        }
        else
        {
            if (m_Instance)
            {
                m_Controller.RemoveModifier(m_Instance.GetComponent<Modifier>());
                Destroy(m_Instance);
                m_Image.color = m_DisabledColor;
            }
        }
    }
}
