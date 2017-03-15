using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInformation : MonoBehaviour
{
    //Public vars

    //Component vars
    private Text m_Text;
    private ControllerScene m_Scene;
    private GameObject m_Panel;

	void Start()
    {
        m_Text = GetComponentInChildren<Text>();
        if (!m_Text)
        {
            Debug.Log("Debug information is missing its text component!");
            enabled = false;
            return;
        }

        m_Scene = FindObjectOfType<ControllerScene>();
        m_Panel = transform.FindChild("Panel").gameObject;

        m_Text.gameObject.SetActive(false);
        m_Panel.SetActive(false);
	}
	
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (m_Text && m_Panel)
            {
                m_Panel.SetActive(!m_Panel.activeSelf);
                m_Text.gameObject.SetActive(!m_Text.gameObject.activeSelf);
            }
        }

        if (m_Text)
        {
            string info = "";

            for (int i = 0; i < m_Scene.GetPlayers().Count; i++)
            {
                info += m_Scene.GetPlayers()[i].GetDebugInformation();
                if (i < m_Scene.GetPlayers().Count - 1)
                    info += "\n";
            }

            m_Text.text = info;

            info = "";
        }
	}
}
