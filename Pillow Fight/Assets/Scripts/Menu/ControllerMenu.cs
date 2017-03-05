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

/// <summary>
/// This is the complete controller for the main-menu of the game, it handles menu-state logic, player lobby logic etc
/// </summary>
public class ControllerMenu : MonoBehaviour
{
    //Public vars
    public float m_CountdownTime = 10.0f;
    public List<Color> m_Colors;

    private static List<Color> m_StaticColors = new List<Color>();

    public List<PlayerIndex> m_Controllers = new List<PlayerIndex>();

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

	void Start()
    {
        //Reset toolbox variables whenever main menu is loaded
        Toolbox.Instance.ClearInformation();

        m_MusicManager = FindObjectOfType<MusicManager>();
        m_MusicManager.MenuMusic();

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
        for (int i = 0; i < m_Colors.Count; i++)
        {
            m_StaticColors.Add(m_Colors[i]);
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
            Application.Quit();

        InputUpdate();
        CountdownUpdate();
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
                        Toolbox.Instance.m_Colors.Add(Color.white);
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
                m_PlayerSlots[m_SlotIndex].SetOpen(false);
                m_PlayerSlots[m_SlotIndex].SetControllerNum(-1);
                m_PlayerSlots[m_SlotIndex].SetKeyboard(true);
                Toolbox.Instance.m_Information.Add(new PlayerInformation(ControllerType.Keyboard, -1, PlayerIndex.One));
                Toolbox.Instance.m_Colors.Add(Color.white);
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
                {
                    m_MusicManager.MatchMusic();
                    SceneManager.LoadScene(1);
                }

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

    public static void RemoveColor(Color col)
    {
        if (m_StaticColors.Contains(col))
        {
            //Debug.Log("Removed color: " + col);
            m_StaticColors.Remove(col);
        }
    }

    public static void InsertColor(int index, Color col)
    {
        if (!m_StaticColors.Contains(col))
        {
            //Debug.Log("Inserted color at: " + index);
            m_StaticColors.Insert(index, col);
        }
    }

    public static Color GetColor(int index)
    {
        return m_StaticColors[index];
    }

    public static int GetColorAmount()
    {
        return m_StaticColors.Count;
    }
}
