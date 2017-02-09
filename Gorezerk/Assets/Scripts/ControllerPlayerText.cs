using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerPlayerText : MonoBehaviour
{

    //Component vars
    private ControllerPlayer m_Player;
    private Text m_Text;

    //Rotation vars
    private Transform m_LookAt;

	void Start()
    {
        m_Player = transform.parent.GetComponent<ControllerPlayer>();
        if (!m_Player)
        {
            Debug.Log("Text handler in player local canvas could not find player!");
            enabled = false;
            return;
        }

        m_LookAt = Camera.main.transform;
        m_Text = GetComponentInChildren<Text>();
	}
	
	void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - m_LookAt.position);
        TextUpdate();	
	}

    void TextUpdate()
    {
        if (m_Text)
        {
            m_Text.text = m_Player.GetCurrentHookCharges().ToString();
        }
    }
}
