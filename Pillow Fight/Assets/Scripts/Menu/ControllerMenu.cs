using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

[System.Serializable]
public struct PlayerInformation
{
    public int m_ControllerNum;
    public ControllerType m_ControllerType;
    public PlayerIndex m_PlayerIndex;
    public PlayerInformation(ControllerType controllertype, int controllernum, PlayerIndex index)
    {
        m_ControllerNum = controllernum;
        m_ControllerType = controllertype;
        m_PlayerIndex = index;

        if (m_ControllerType.Equals(ControllerType.Keyboard))
            m_ControllerNum = -1;
    }

    public int GetControllerNum()
    {
        return m_ControllerNum;
    }
    public void SetControllerNum(int num)
    {
        m_ControllerNum = num;
    }
    public void SetPlayerIndex(PlayerIndex index)
    {
        m_PlayerIndex = index;
    }

    public void SetControllerType(ControllerType newtype)
    {
        m_ControllerType = newtype;
    }
    public ControllerType GetCType()
    {
        return m_ControllerType;
    }
    public PlayerIndex GetPlayerIndex()
    {
        return m_PlayerIndex;
    }
}

[System.Serializable]
public struct SelectInformation
{
    public bool m_Selected;
    public Color m_Color;
    public GameObject m_Character;

    public SelectInformation(bool selected, GameObject character, Color col)
    {
        m_Character = character;
        m_Color = col;
        m_Selected = selected;
    }

    public bool GetSelected()
    {
        return m_Selected;
    }

    public GameObject GetCharacter()
    {
        return m_Character;
    }

    public Color GetColor()
    {
        return m_Color;
    }
}

[System.Serializable]
public struct CharacterInformation
{
    public GameObject m_Character;
    public Color m_Color;

    public CharacterInformation(GameObject character, Color col)
    {
        m_Character = character;
        m_Color = col;
    }

    public GameObject GetCharacter()
    {
        return m_Character;
    }

    public Color GetColor()
    {
        return m_Color;
    }
}

public enum MenuState
{
    Main,
    CharacterSelect
}

/// <summary>
/// This is the complete controller for the main-menu of the game, it handles menu-state logic, player lobby logic etc
/// </summary>
public class ControllerMenu : MonoBehaviour
{
    //Public vars
    [Header("Spawnable prefabs")]
    public GameObject m_SfxPrefab;
    public GameObject m_MusicPrefab;

    [Header("Countdown")]
    public float m_CountdownTime = 10.0f;

    [Header("Current menu state (read only)")]
    public MenuState m_State = MenuState.Main;

    [Header("Selectable characters")]
    public List<CharacterInformation> m_Characters = new List<CharacterInformation>();

    //Player/selection vars
    [HideInInspector]
    public List<SelectInformation> m_SelectInformation = new List<SelectInformation>();

    private List<PlayerIndex> m_Controllers = new List<PlayerIndex>();

    //Player slot vars
    private List<PlayerSlot> m_PlayerSlots = new List<PlayerSlot>();
    private List<bool> m_ControllerSlots = new List<bool>();
    private int m_SlotIndex = 0;
    private bool m_HasSetKeyboard = false;

    //Ready vars
    private bool m_IsCountDown = false;
    private float m_CountdownTimer = 0.0f;
    private Text m_CountdownText;

    //Music variables
    private MusicManager m_MusicManager;
    private SFXManager m_SfxManager;

    void Start()
    {
        for (int i = 0; i < m_Characters.Count; i++)
        {
            m_SelectInformation.Add(new SelectInformation(false, m_Characters[i].GetCharacter(), m_Characters[i].GetColor()));
        }

        //Reset toolbox variables whenever main menu is loaded
        Toolbox.Instance.ClearInformation();

        var musics = FindObjectsOfType<MusicManager>();
        if (musics.Length == 0)
        {
            Debug.Log("Could not find any active music managers, creating one");
            m_MusicManager = Instantiate(m_MusicPrefab, Vector3.zero, Quaternion.identity).GetComponent<MusicManager>();
        }
        else if (!m_MusicManager)
            m_MusicManager = FindObjectOfType<MusicManager>();

        m_MusicManager.MenuMusic();

        var sfx = FindObjectsOfType<SFXManager>();
        if (sfx.Length == 0)
        {
            Debug.Log("Could not find any active sfx managers, creating one");
            m_SfxManager = Instantiate(m_SfxPrefab, Vector3.zero, Quaternion.identity).GetComponent<SFXManager>();
        }
        else if (!m_SfxManager)
            m_SfxManager = FindObjectOfType<SFXManager>();

        //Set countdown vars
        m_CountdownTimer = m_CountdownTime;
        m_CountdownText = GameObject.Find("CountdownText").GetComponent<Text>();
        if (!m_CountdownText)
            Debug.Log("Could not find countdowntext!");

        //Find playerslots
        var slots = GameObject.FindGameObjectsWithTag("Player");
        if (slots.Length > 0)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].GetComponent<PlayerSlot>())
                    m_PlayerSlots.Add(slots[i].GetComponent<PlayerSlot>());
                else
                    Debug.Log("Player slot: " + i + " does not have a playerslot script!");
            }

            //Sort playerslots
            for (int write = 0; write < m_PlayerSlots.Count; write++)
            {
                for (int sort = 0; sort < m_PlayerSlots.Count - 1; sort++)
                {
                    if (m_PlayerSlots[sort].m_PlayerNum > m_PlayerSlots[sort + 1].m_PlayerNum)
                    {
                        PlayerSlot temp = m_PlayerSlots[sort];
                        m_PlayerSlots[sort] = m_PlayerSlots[sort + 1];
                        m_PlayerSlots[sort + 1] = temp;
                    }
                }
            }
        }
        else
            Debug.Log("Couldn't find any playerslots!");

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            m_ControllerSlots.Add(false);
        }

        for (int i = 0; i < 4; i++)
        {
            PlayerIndex test = (PlayerIndex)i;
            GamePadState state = GamePad.GetState(test);
            if (state.IsConnected)
                m_Controllers.Add(test);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Exit();

        if (m_State.Equals(MenuState.CharacterSelect))
        {
            InputUpdate();
            CountdownUpdate();
        }
    }

    void InputUpdate()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (m_ControllerSlots[i] != true)
            {
                bool open = false;
                for (int j = 0; j < m_PlayerSlots.Count; j++)
                {
                    if (m_PlayerSlots[j].GetOpen())
                    {
                        open = true;
                        m_SlotIndex = j;
                        break;
                    }
                }
                if (open)
                {
                    bool xbox = Input.GetAxis("P" + i + "Jump") != 0.0f;
                    bool PS = Input.GetAxis("P" + i + "JumpPS") != 0.0f;
                    PlayerIndex addIndex = PlayerIndex.One;
                    for (int j = 0; j < m_Controllers.Count; j++)
                    {
                        if (GamePad.GetState(m_Controllers[j]).Buttons.A == ButtonState.Pressed)
                        {
                            addIndex = m_Controllers[j];
                            break;
                        }
                    }
                    if (xbox || PS)
                    {
                        Toolbox.Instance.m_Colors.Add(Color.white);
                        Toolbox.Instance.m_Characters.Add(null);
                        ControllerType type = ControllerType.Xbox;
                        m_PlayerSlots[m_SlotIndex].SetOpen(false);
                        m_PlayerSlots[m_SlotIndex].SetControllerNum(i);
                        m_ControllerSlots[i] = true;

                        if (xbox)
                        {
                            if (Input.GetJoystickNames()[i].Contains("One"))
                                type = ControllerType.XboxOne;
                            else
                                type = ControllerType.Xbox;
                        }
                        else if (PS)
                            type = ControllerType.PS;

                        Toolbox.Instance.m_Information.Add(new PlayerInformation(type, i, addIndex));
                        break;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && !m_HasSetKeyboard)
        {
            bool open = false;
            for (int j = 0; j < m_PlayerSlots.Count; j++)
            {
                if (m_PlayerSlots[j].GetOpen())
                {
                    open = true;
                    m_SlotIndex = j;
                    break;
                }
            }
            if (open)
            {
                m_HasSetKeyboard = true;
                Toolbox.Instance.m_Information.Add(new PlayerInformation(ControllerType.Keyboard, -1, PlayerIndex.One));
                Toolbox.Instance.m_Colors.Add(Color.white);
                Toolbox.Instance.m_Characters.Add(null);
                m_PlayerSlots[m_SlotIndex].SetOpen(false);
                m_PlayerSlots[m_SlotIndex].SetControllerNum(-1);
                m_PlayerSlots[m_SlotIndex].SetKeyboard(true);
            }
        }
    }

    void CountdownUpdate()
    {
        if (m_CountdownText)
        {
            m_IsCountDown = ReadyCheck();

            if (m_IsCountDown)
            {
                if (m_CountdownTimer > 1.0f)
                    m_CountdownTimer -= Time.deltaTime;
                else
                    SceneManager.LoadScene(1);

                m_CountdownText.text = m_CountdownTimer.ToString("F0");
            }
            else
            {
                m_CountdownTimer = m_CountdownTime;
                m_CountdownText.text = "";
            }
        }
    }

    bool ReadyCheck()
    {
        bool ready = true;
        bool exists = false;
        //int num = 0;

        for (int i = 0; i < m_PlayerSlots.Count; i++)
        {
            if (!m_PlayerSlots[i].GetOpen())
            {
                exists = true;
                //num++;
                if (!m_PlayerSlots[i].GetReady())
                {
                    ready = false;
                    break;
                }
            }
            else
            {
                if (i == m_PlayerSlots.Count - 1 && (!exists))// || num <= 1))
                    ready = false;
            }
        }

        return ready;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OnPlayButton()
    {
        m_State = MenuState.CharacterSelect;
    }
}
