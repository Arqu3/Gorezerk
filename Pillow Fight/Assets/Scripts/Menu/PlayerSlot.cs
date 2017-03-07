using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using XInputDotNetPure;

public class PlayerSlot : MonoBehaviour
{
    //Public vars
    public int m_PlayerNum = 0;

    //Text vars
    private Text m_NumText;
    private Text m_ReadyText;
    private Text m_SlotText;
    private Image m_Image;
    private bool m_IsOpen = true;
    private bool m_IsReady = false;

    //Controller vars
    private bool m_IsKeyboardInput = false;
    private int m_ControllerNum = -1;
    private bool m_IsAxisInput = false;
    private bool m_IsAxisInUse = false;
    private bool m_IsButtonInput = false;
    private bool m_IsButtonInUse = true;
    private PlayerIndex m_PlayerIndex;

    //Color vars
    private int m_ColorCounter = 0;

    //Component vars
    private ControllerMenu m_Menu;

    void Awake()
    {
        m_Menu = FindObjectOfType<ControllerMenu>();

        m_NumText = transform.FindChild("PlayerText").GetComponent<Text>();
        m_ReadyText = transform.FindChild("ReadyText").GetComponent<Text>();
        if (m_ReadyText)
            m_ReadyText.gameObject.SetActive(false);

        m_SlotText = transform.FindChild("SlotText").GetComponent<Text>();
        m_Image = transform.FindChild("ColorImage").GetComponent<Image>();
        if (m_Image)
            m_Image.gameObject.SetActive(false);
    }
	
	void Update ()
    {
        ObjectUpdate();

        InputUpdate();
    }

    void ObjectUpdate()
    {
        if (m_ReadyText)
            m_ReadyText.gameObject.SetActive(m_IsReady);

        if (m_SlotText)
            m_SlotText.gameObject.SetActive(m_IsOpen);

        if (m_Image)
            m_Image.gameObject.SetActive(!m_IsOpen);

        if (m_NumText)
            m_NumText.text = "Player" + (m_PlayerNum + 1);
    }

    void InputUpdate()
    {
        //Check controller index
        if (m_ControllerNum != -1)
        {
            //Make sure input isn't keyboard-based
            if (!m_IsKeyboardInput)
            {
                if (!m_IsReady)
                {
                    //Register input
                    m_IsAxisInput = Input.GetAxis("P" + m_ControllerNum + "Horizontal") != 0.0f;
                    if (m_IsAxisInput)
                    {
                        //Getkeydown functionality for axis-input
                        if (!m_IsAxisInUse)
                        {
                            //if (!m_Menu.m_Colors.Contains(m_Image.color))
                            //    m_Menu.m_Colors.Add(m_Image.color);

                            //Switch between colors
                            float horizontal = Input.GetAxisRaw("P" + m_ControllerNum + "Horizontal");
                            if (horizontal > 0)
                            {
                                m_ColorCounter++;
                                if (m_ColorCounter >= m_Menu.m_Colors.Count)
                                    m_ColorCounter = 0;
                            }
                            else if (horizontal < 0)
                            {
                                m_ColorCounter--;
                                if (m_ColorCounter < 0 || m_ColorCounter >= m_Menu.m_Colors.Count)
                                    m_ColorCounter = m_Menu.m_Colors.Count - 1;
                            }

                            m_Image.color = m_Menu.m_Colors[m_ColorCounter];
                            Toolbox.Instance.m_Colors[m_PlayerNum] = m_Menu.m_Colors[m_ColorCounter];

                            //m_Menu.m_Colors.Remove(m_Image.color);

                            m_IsAxisInUse = true;
                        }
                    }

                    if (!m_IsAxisInput)
                        m_IsAxisInUse = false;
                }

                //Ready-button check
                m_IsButtonInput = Input.GetAxis("P" + m_ControllerNum + "Jump") != 0.0f || Input.GetAxis("P" + m_ControllerNum + "JumpPS") != 0.0f;
                if (m_IsButtonInput)
                {
                    if (!m_IsButtonInUse)
                    {
                        ToggleReady();
                        m_IsButtonInUse = true;
                    }
                }
                if (!m_IsButtonInput)
                    m_IsButtonInUse = false;
            }
        }
        else
        {
            if (m_IsKeyboardInput)
            {
                //Switch between colors (only when not ready)
                if (!m_IsReady)
                {
                    bool input = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
                    if (input)
                    {
                        //if (!m_Menu.m_Colors.Contains(m_Image.color))
                        //    m_Menu.m_Colors.Add(m_Image.color);

                        if (Input.GetKeyDown(KeyCode.D))
                        {
                            m_ColorCounter++;
                            if (m_ColorCounter >= m_Menu.m_Colors.Count)
                                m_ColorCounter = 0;
                        }
                        else if (Input.GetKeyDown(KeyCode.A))
                        {
                            m_ColorCounter--;
                            if (m_ColorCounter < 0 || m_ColorCounter >= m_Menu.m_Colors.Count)
                                m_ColorCounter = m_Menu.m_Colors.Count - 1;
                        }

                        m_Image.color = m_Menu.m_Colors[m_ColorCounter];
                        Toolbox.Instance.m_Colors[m_PlayerNum] = m_Menu.m_Colors[m_ColorCounter];

                        //m_Menu.m_Colors.Remove(m_Image.color);
                    }
                }

                //Toggle ready
                if (Input.GetKeyDown(KeyCode.Space))
                    ToggleReady();
            }
        }
    }

    public void SetControllerNum(int num)
    {
        m_ControllerNum = num;
    }
    public int GetControllerNum()
    {
        return m_ControllerNum;
    }

    public void SetPlayerIndex(PlayerIndex index)
    {
        m_PlayerIndex = index;
    }
    public PlayerIndex GetPlayerIndex()
    {
        return m_PlayerIndex;
    }

    public void SetOpen(bool state)
    {
        m_IsOpen = state;
    }
    public bool GetOpen()
    {
        return m_IsOpen;
    }

    public void SetReady(bool state)
    {
        m_IsReady = state;
    }
    public void ToggleReady()
    {
        m_IsReady = !m_IsReady;
        if (m_IsReady)
            Toolbox.Instance.m_PlayerCount++;
        else
            Toolbox.Instance.m_PlayerCount--;
    }
    public bool GetReady()
    {
        return m_IsReady;
    }

    public void SetKeyboard(bool state)
    {
        m_IsKeyboardInput = state;
    }
    public bool GetKeyboard()
    {
        return m_IsKeyboardInput;
    }
}
