using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ControllerScene : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 3.0f;
    public float m_CountdownSpeed = 2.0f;
    public float m_BarkTime = 1.0f;

    private List<Transform> m_SpawnPoints = new List<Transform>();
    private List<ControllerPlayer> m_Players = new List<ControllerPlayer>();

    //Static vars
    private static bool m_IsPaused = true;
    private static int m_PlayerCount = 0;

    //Round start vars
    private Text m_CountdownText;
    private float m_CountdownTimer = 0.0f;
    private static bool m_IsRoundStart = true;

    //Score vars
    private Text m_ScoreText;
    private string[] m_ScoreStrings;

    //Bark vars
    private static string m_ScoreBark = "";
    private Text m_BarkText;
    private float m_BarkTimer = 0.0f;
    private static bool m_IsBark = false;

    //Pause vars
    GameObject m_PausePanel;

    void Awake()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            m_ScoreStrings = new string[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<ControllerPlayer>())
                    m_Players.Add(players[i].GetComponent<ControllerPlayer>());
            }

            for (int write = 0; write < m_Players.Count; write++)
            {
                for (int sort = 0; sort < m_Players.Count - 1; sort++)
                {
                    if (m_Players[sort].GetPlayerNum() > m_Players[sort + 1].GetPlayerNum())
                    {
                        ControllerPlayer temp = m_Players[sort + 1];
                        m_Players[sort + 1] = m_Players[sort];
                        m_Players[sort] = temp;
                    }
                }
            }

            //for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            //{
            //    Debug.Log(Input.GetJoystickNames()[i]);
            //    if (i < m_Players.Count)
            //    {
            //        m_Players[i].SetKeyboardInput(false);
            //        if (Input.GetJoystickNames()[i].Contains("XBOX") || Input.GetJoystickNames()[i].Contains("xbox") || Input.GetJoystickNames()[i].Contains("Xbox"))
            //            m_Players[i].SetControllerType(ControllerType.Xbox);
            //        else
            //            m_Players[i].SetControllerType(ControllerType.PS);
            //    }
            //}

        }
    }

    void Start()
    {
        m_CountdownText = GameObject.Find("CountdownText").GetComponent<Text>();
        m_CountdownTimer = m_CountdownTime;

        m_BarkText = GameObject.Find("ScoreBarkText").GetComponent<Text>();

        m_ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        if (m_ScoreText)
            m_ScoreText.text = "";

        var spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnpoints.Length > 0)
        {
            for (int i = 0; i < spawnpoints.Length; i++)
            {
                m_SpawnPoints.Add(spawnpoints[i].transform);
            }
        }

        m_PausePanel = GameObject.Find("PausePanel");

        UpdateText();
        SpawnPlayers();
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePaused();

        if (Input.GetKeyDown(KeyCode.Return))
            StartRound();

        CountdownUpdate();
        if (m_PlayerCount <= 1)
        {
            UpdateText();
            StartRound();
        }

        if (!m_IsRoundStart)
        {
            Cursor.visible = m_IsPaused;
            m_PausePanel.SetActive(m_IsPaused);
        }
        else
        {
            Cursor.visible = false;
            m_PausePanel.SetActive(false);
        }

        ScoreBarkUpdate();
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

    void ScoreBarkUpdate()
    {
        if (m_IsBark)
        {
            if (m_BarkText)
                m_BarkText.text = m_ScoreBark + " score!";

            if (m_BarkTimer < m_BarkTime)
                m_BarkTimer += Time.deltaTime;
            else
            {
                m_BarkTimer = 0.0f;
                SetScoreBark("");
            }
        }
        else
            m_BarkText.text = "";
    }

    void UpdateText()
    {
        m_ScoreText.text = "";
        for (int i = 0; i < m_Players.Count; i++)
        {
            m_ScoreStrings[i] = "Player" + (m_Players[i].GetPlayerNum() + 1) + ": " + m_Players[i].GetScore() + "                        ";
            m_ScoreText.text += m_ScoreStrings[i];
        }
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

    public void Quit()
    {
        Application.Quit();
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TogglePaused()
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

    public static void SetScoreBark(string s)
    {
        m_ScoreBark = s;
        m_IsBark = m_ScoreBark != "";
    }
}
