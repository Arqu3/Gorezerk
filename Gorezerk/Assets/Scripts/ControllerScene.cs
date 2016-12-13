using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControllerScene : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 3.0f;
    public float m_CountdownSpeed = 2.0f;

    public List<Transform> m_SpawnPoints;
    public List<ControllerPlayer> m_Players;

    //Static vars
    private static bool m_IsPaused = true;
    private static int m_PlayerCount;

    //Round start vars
    private Text m_CountdownText;
    private float m_CountdownTimer = 0.0f;
    private static bool m_IsRoundStart = true;

	void Start()
    {
        m_CountdownText = GameObject.Find("CountdownText").GetComponent<Text>();
        m_CountdownTimer = m_CountdownTime;

        var spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnpoints.Length > 0)
        {
            for (int i = 0; i < spawnpoints.Length; i++)
            {
                m_SpawnPoints.Add(spawnpoints[i].transform);
            }
        }

        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<ControllerPlayer>())
                    m_Players.Add(players[i].GetComponent<ControllerPlayer>());
            }
            SpawnPlayers();
        }
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePaused();

        if (Input.GetKeyDown(KeyCode.Space))
            StartRound();

        CountdownUpdate();
        if (m_PlayerCount <= 1)
            StartRound();
	}

    void CountdownUpdate()
    {
        if (m_IsRoundStart)
        {
            m_CountdownTimer -= Time.deltaTime * m_CountdownSpeed;
            if (m_CountdownTimer > 1)
                m_CountdownText.text = m_CountdownTimer.ToString("F0");
            else
                m_CountdownText.text = "GO!";
            if (m_CountdownTimer <= 0.0f)
            {
                m_CountdownTimer = m_CountdownTime;
                SetPaused(false);
                m_IsRoundStart = false;
            }
        }
        else
            m_CountdownText.text = "";
    }

    void TextUpdate()
    {

    }

    void SpawnPlayers()
    {
        List<Transform> tempSpawn = new List<Transform>();
        m_PlayerCount = m_Players.Count;

        for (int i = 0; i < m_SpawnPoints.Count; i++)
        {
            tempSpawn.Add(m_SpawnPoints[i]);
        }

        for (int i = 0; i < m_Players.Count; i++)
        {
            m_Players[i].gameObject.SetActive(true);
            m_Players[i].ResetValues();
            int random = Random.Range(0, tempSpawn.Count);
            m_Players[i].transform.position = tempSpawn[random].position;
            tempSpawn.RemoveAt(random);
        }
    }

    void StartRound()
    {
        m_IsRoundStart = true;
        m_IsPaused = true;
        SpawnPlayers();
        Time.timeScale = 1.0f;
    }

    void TogglePaused()
    {
        m_IsPaused = !m_IsPaused;

        if (GetPaused())
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    void SetPaused(bool state)
    {
        m_IsPaused = state;

        if (GetPaused())
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public static bool GetPaused()
    {
        return m_IsPaused;
    }

    public static bool GetRoundStart()
    {
        return m_IsRoundStart;
    }

    public static void ReducePlayerCount()
    {
        m_PlayerCount--;
    }
}
