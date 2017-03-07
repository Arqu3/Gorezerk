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
    public int m_ScoreToWinRound = 10;
    public int m_RoundsToWin = 5;
    public GameObject m_PlayerPrefab;

    //Lists
    private List<Transform> m_SpawnPoints = new List<Transform>();
    private List<ControllerPlayer> m_Players = new List<ControllerPlayer>();

    //Static vars
    private static bool m_IsPaused = true;
    private static int m_PlayerCount = 0;
    private static bool m_UpdateText = true;
    private static int m_RoundNum = 0;

    //Round start vars
    private Text m_CountdownText;
    private float m_CountdownTimer = 0.0f;
    private static bool m_IsRoundStart = true;
    private int m_RestartNum = 0;

    //Score vars
    private Text m_ScoreText;
    private bool m_FirstRoundWon = false;

    //Bark vars
    private static string m_ScoreBark = "";
    private static Color m_BarkColor = Color.red;
    private Text m_BarkText;
    private float m_BarkTimer = 0.0f;
    private static bool m_IsBark = false;

    //Panel vars
    private GameObject m_PausePanel;
    private GameObject m_ModifierPanel;
    private GameObject m_GameOverPanel;

    //Game over vars
    private static bool m_GameOver = false;
    private static bool m_ResetScores = false;

    //Component
    private MusicManager m_MusicManager;
    private ControllerModifiers m_ModController;

    void Awake()
    {
        //Static variables needs to reset manually because they are a part of the class, not the object
        m_GameOver = false;
        m_ResetScores = false;
        m_IsBark = false;
        m_BarkColor = Color.red;
        m_ScoreBark = "";
        m_IsPaused = true;
        m_PlayerCount = 0;
        m_UpdateText = true;
        m_RoundNum = 0;

        m_MusicManager = FindObjectOfType<MusicManager>();
        m_MusicManager.MatchMusic();

        m_ModController = GetComponent<ControllerModifiers>();

        if (m_PlayerPrefab)
        {
            for (int i = 0; i < Toolbox.Instance.m_PlayerCount; i++)
            {
                GameObject clone = (GameObject)Instantiate(m_PlayerPrefab, Vector3.zero, Quaternion.identity);
                if (clone.GetComponent<ControllerPlayer>())
                    clone.GetComponent<ControllerPlayer>().SetPlayerNum(i);
                else
                    Debug.Log("Player clone " + i + " is missing a controllerplayer script!");
            }
        }
        else
            Debug.Log(gameObject.name + " does not have a player prefab to instantiate!");

        var players = FindObjectsOfType<ControllerPlayer>();
        if (players.Length > 0)
        {
            for (int i = 0; i < players.Length; i++)
            {
                m_Players.Add(players[i]);
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
        m_GameOverPanel = GameObject.Find("GameOverPanel");

        if (m_ModifierPanel)
            m_ModifierPanel.SetActive(false);

        if (m_GameOverPanel)
            m_GameOverPanel.SetActive(false);

        StartRound();
    }

    void Update()
    {
        if (!m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !m_IsRoundStart)
                TogglePaused();

            if (m_IsPaused && Input.GetKeyDown(KeyCode.F11))
                m_ModifierPanel.SetActive(!m_ModifierPanel.activeSelf);

            if (Input.GetKeyDown(KeyCode.Return))
                StartRound();

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

            CountdownUpdate();
            ScoreBarkUpdate();
        }
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
                for (int i = 0; i < m_ModController.GetActiveMods().Count; i++)
                {
                    m_ModController.GetActiveMods()[i].OnRoundStart();
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
            {
                if (m_BarkText.color != m_BarkColor)
                    m_BarkText.color = m_BarkColor;

                m_BarkText.text = m_ScoreBark + " score!";
            }

            if (m_BarkTimer < m_BarkTime)
                m_BarkTimer += Time.deltaTime;
            else
            {
                m_BarkTimer = 0.0f;
                SetScoreBark("", Color.red);
            }
        }
        else
            m_BarkText.text = "";
    }

    void UpdateText()
    {
        if (m_ResetScores)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                m_Players[i].SetScore(0);
            }
            m_FirstRoundWon = true;
            m_ResetScores = false;
        }

        m_ScoreText.text = "";
        for (int i = 0; i < m_Players.Count; i++)
        {
            m_ScoreText.text += "<color=#" + ColorToHex(m_Players[i].GetColor()) + ">" + "Player " + (m_Players[i].GetPlayerNum() + 1) + ": " + m_Players[i].GetScore() + "</color>";
            if (i < m_Players.Count - 1)
                m_ScoreText.text += "    ";
        }
        m_ScoreText.text += "\n";
        for (int i = 0; i < m_Players.Count; i++)
        {
            m_ScoreText.text += "<color=#" + ColorToHex(m_Players[i].GetColor()) + ">" + "Rounds: " + m_Players[i].GetRounds() + "</color>";
            if (i < m_Players.Count - 1)
                m_ScoreText.text += "    ";
        }

        if (m_GameOver)
        {
            m_UpdateText = true;
            Time.timeScale = 0.0f;
            m_IsPaused = true;
            Cursor.visible = true;
            m_GameOverPanel.SetActive(true);
        }
    }

    string ColorToHex(Color32 col)
    {
        string hex = col.r.ToString("X2") + col.g.ToString("X2") + col.b.ToString("X2");
        return hex;
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
        for (int i = 0; i < m_ModController.GetActiveMods().Count; i++)
        {
            m_ModController.GetActiveMods()[i].OnRoundEnd();
        }

        m_RoundNum++;

        if (m_ModController && m_FirstRoundWon && m_RoundNum % m_ModController.m_RoundToChange == 0)
        {
            m_CountdownTimer = m_CountdownTime * 2.0f;
            m_ModController.ChangeMods();
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
        m_MusicManager.UnPauseMusic();
        m_MusicManager.MenuMusic();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
    public void ReloadScene()
    {
        Time.timeScale = 1.0f;
        Toolbox.Instance.m_CanMove = true;
        Toolbox.Instance.m_MovementSpeed = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePaused()
    {
        m_IsPaused = !m_IsPaused;

        if (GetPaused())
        {
            m_MusicManager.PauseMusic();
            Time.timeScale = 0.0f;
        }
        else
        {
            m_MusicManager.UnPauseMusic();
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

    public static void SetScoreBark(string s, Color textCol)
    {
        m_BarkColor = textCol;
        m_ScoreBark = s;
        m_IsBark = m_ScoreBark != "";
    }

    public static void ToggleUpdateText()
    {
        m_UpdateText = true;
    }

    public static int GetRoundNum()
    {
        return m_RoundNum;
    }

    public static void GameOver()
    {
        m_GameOver = true;
    }

    public static void ResetScores()
    {
        m_ResetScores = true;
    }

    public void AddModifier(Modifier mod)
    {
        m_ModController.AddMod(mod);
        
    }

    public void RemoveModifier(Modifier mod)
    {
        m_ModController.AddMod(mod);
    }

    public List<Transform> GetSpawnPoints()
    {
        return m_SpawnPoints;
    }

    public List<ControllerPlayer> GetPlayers()
    {
        return m_Players;
    }
}
