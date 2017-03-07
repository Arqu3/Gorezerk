using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class RedButton : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 5.0f;

    //Component vars
    private ControllerPlayer m_SafePlayer;
    private Text m_CountdownText;

    //Timer vars
    private float m_CountdownTimer = 0.0f;
    private bool m_IsCountdown = false;
    private Transform m_LastSpawn;

	void Start()
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
	
	void Update()
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
            SelectSpawnPosition();

            m_CountdownTimer = m_CountdownTime;
            m_SafePlayer = col.gameObject.GetComponent<ParryHitbox>().GetPlayer();
            GetComponent<MeshRenderer>().material.color = m_SafePlayer.gameObject.GetComponent<MeshRenderer>().material.color;
            m_IsCountdown = true;
        }
        else if (col.gameObject.GetComponent<AttackHitbox>())
        {
            SelectSpawnPosition();

            m_CountdownTimer = m_CountdownTime;
            m_SafePlayer = col.gameObject.GetComponent<AttackHitbox>().GetPlayer();
            GetComponent<MeshRenderer>().material.color = m_SafePlayer.gameObject.GetComponent<MeshRenderer>().material.color;
            m_IsCountdown = true;
        }
    }

    void SelectSpawnPosition()
    {
        List<Transform> temp = new List<Transform>();

        var scene = FindObjectOfType<ControllerScene>();
        if (scene)
        {
            for (int i = 0; i < scene.GetSpawnPoints().Count; i++)
            {
                if (scene.GetSpawnPoints()[i] != m_LastSpawn)
                {
                    temp.Add(scene.GetSpawnPoints()[i]);
                }
            }
        }

        int random = Random.Range(0, temp.Count);
        transform.position = temp[random].position;

        m_LastSpawn = temp[random];
    }
}
