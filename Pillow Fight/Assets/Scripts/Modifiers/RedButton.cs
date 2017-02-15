using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class RedButton : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 5.0f;

    public ControllerPlayer m_SafePlayer;
    public Text m_CountdownText;

    //Timer vars
    private float m_CountdownTimer = 0.0f;
    private bool m_IsCountdown = false;

	void Start ()
    {
        m_CountdownText = GetComponentInChildren<Text>();
        if (!m_CountdownText)
        {
            Debug.Log("Redbutton is missing its text component!");
            enabled = false;
            return;
        }

        m_CountdownText.text = "";
        m_CountdownTimer = m_CountdownTime;
        GetComponent<MeshRenderer>().material.color = Color.red;
	}
	
	void Update ()
    {
		if (m_IsCountdown)
        {
            m_CountdownTimer -= Time.deltaTime;
            m_CountdownText.text = m_CountdownTimer.ToString("F1");
            if (m_CountdownTimer <= 0.0f)
            {
                m_CountdownTimer = m_CountdownTime;
                m_CountdownText.text = "";
                m_IsCountdown = false;
                m_SafePlayer.AddScore(1);

                var players = FindObjectsOfType<ControllerPlayer>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] == m_SafePlayer)
                        continue;

                    players[i].Kill();
                }
                m_SafePlayer = null;
            }
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<ParryHitbox>())
        {
            ControllerScene scene = FindObjectOfType<ControllerScene>();
            int random = Random.Range(0, scene.GetSpawnPoints().Count);
            transform.position = scene.GetSpawnPoints()[random].position;

            m_CountdownTimer = m_CountdownTime;
            m_SafePlayer = col.gameObject.GetComponent<ParryHitbox>().GetPlayer();
            GetComponent<MeshRenderer>().material.color = m_SafePlayer.gameObject.GetComponent<MeshRenderer>().material.color;
            m_IsCountdown = true;
        }
    }
}
