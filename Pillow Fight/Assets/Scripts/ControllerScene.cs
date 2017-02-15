using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is made in such a way that no more than one instance of it should be present at any time,
/// This class handles all game-state logic (such as pausing etc) and how/when to spawn players in a scene (think god-class-ish)
/// </summary>
public class ControllerScene : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 3.0f;
    public float m_CountdownSpeed = 2.0f;
    public float m_BarkTime = 1.0f;
    public GameObject m_PlayerPrefab;

    private List<Transform> m_SpawnPoints = new List<Transform>();
    public List<ControllerPlayer> m_Players = new List<ControllerPlayer>();

    //Static vars
    private static bool m_IsPaused = true;
    private static int m_PlayerCount = 0;
    private static bool m_UpdateText = false;

    //Round start vars
    private Text m_CountdownText;
    private float m_CountdownTimer = 0.0f;
    private static bool m_IsRoundStart = true;
    private int m_RestartNum = 0;

    //Score vars
    private Text m_ScoreText;
    private string[] m_ScoreStrings;

    //Bark vars
    private static string m_ScoreBark = "";
    private Text m_BarkText;
    private float m_BarkTimer = 0.0f;
    private static bool m_IsBark = false;

    //Modifier vars
    public List<Modifier> m_Modifiers = new List<Modifier>();

    //Panel vars
    GameObject m_PausePanel;
    GameObject m_ModifierPanel;

    void Awake()
    {
        if (m_PlayerPrefab)
        {
            for (int i = 0; i < Toolbox.Instance.m_PlayerCount; i++)
            {
                GameObject clone = (GameObject)Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
                if (clone.GetComponent<ControllerPlayer>())
                    clone.GetComponent<ControllerPlayer>().m_PlayerNum = i;
                else
                    Debug.Log("Player clone " + i + " is missing a controllerplayer script!");
            }
        }
        else
            Debug.Log(gameObject.name + " does not have a player prefab to instantiate!");

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

            if (m_Players.Count > 1)
                m_RestartNum = 1;
            else
                m_RestartNum = 0;
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
        m_ModifierPanel = GameObject.Find("ModifierPanel");

        if (m_ModifierPanel)
            m_ModifierPanel.SetActive(false);

        //Get modifiers
        var mods = GetComponentsInChildren<Modifier>();
        for (int i = 0; i < mods.Length; i++)
        {
            m_Modifiers.Add(mods[i]);
        }

        UpdateText();
        StartRound();
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_IsRoundStart)
            TogglePaused();

        if (Input.GetKeyDown(KeyCode.Return))
            StartRound();

        CountdownUpdate();

        if (m_PlayerCount <= m_RestartNum)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i].gameObject.activeSelf)
                {
                    m_Players[i].AddScore(1);
                    break;
                }
            }
            UpdateText();
            StartRound();
        }

        ScoreBarkUpdate();

        if (m_UpdateText)
        {
            UpdateText();
            m_UpdateText = false;
        }
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

                //Call all modifier starting functions
                for (int i = 0; i < m_Modifiers.Count; i++)
                {
                    m_Modifiers[i].OnRoundStart();
                }
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
            m_ScoreStrings[i] = "Player " + (m_Players[i].GetPlayerNum() + 1) + ": " + m_Players[i].GetScore();
            m_ScoreText.text += m_ScoreStrings[i];
            if (i < m_Players.Count - 1)
                m_ScoreText.text += "    ";
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
        //Call all modifier end functions
        for (int i = 0; i < m_Modifiers.Count; i++)
        {
            m_Modifiers[i].OnRoundEnd();
        }

        m_PausePanel.SetActive(false);
        Cursor.visible = false;

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
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void TogglePaused()
    {
        m_IsPaused = !m_IsPaused;

        if (GetPaused())
            Time.timeScale = 0.0f;
        else
        {
            Time.timeScale = 1.0f;
            if (m_ModifierPanel.activeSelf)
                m_ModifierPanel.SetActive(m_IsPaused);
        }

        Cursor.visible = m_IsPaused;
        m_PausePanel.SetActive(m_IsPaused);
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

    public static int GetPlayerCount()
    {
        return m_PlayerCount;
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

    public static void ToggleUpdateText()
    {
        m_UpdateText = true;
    }

    public void AddModifier(Modifier mod)
    {
        m_Modifiers.Add(mod);
    }

    public void RemoveModifier(Modifier mod)
    {
        m_Modifiers.Remove(mod);
    }

    public List<Transform> GetSpawnPoints()
    {
        return m_SpawnPoints;
    }
}
