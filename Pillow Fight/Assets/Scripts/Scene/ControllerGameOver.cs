using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ControllerGameOver : MonoBehaviour
{
    //Component vars
    private Text m_Text;

    private bool m_WasDisabled = false;

    void Awake()
    {
        m_Text = GetComponent<Text>();
    }

    void OnDisable()
    {
        m_WasDisabled = true;
    }

    void OnEnable()
    {
        if (m_WasDisabled)
        {
            m_Text.text = "";
            var players = FindObjectOfType<ControllerScene>().GetPlayers();

            for (int write = 0; write < players.Count; write++)
            {
                for (int sort = 0; sort < players.Count - 1; sort++)
                {
                    if (players[sort].GetRounds() < players[sort + 1].GetRounds())
                    {
                        ControllerPlayer temp = players[sort + 1];
                        players[sort + 1] = players[sort];
                        players[sort] = temp;
                    }
                }
            }

            m_Text.text = "<color=#" + ColorToHex(players[0].GetColor()) + ">" + "PLAYER " + (players[0].GetPlayerNum() + 1) + " WINS!" + "</color>" + "\n\n";

            for (int i = 0; i < players.Count; i++)
            {
                string placement = "";

                switch (i)
                {
                    case 0:
                        placement = "1st: ";
                        break;

                    case 1:
                        placement = "2nd: ";
                        break;

                    case 2:
                        placement = "3rd: ";
                        break;

                    case 3:
                        placement = "4th: ";
                        break;
                }

                m_Text.text += "<color=#" + ColorToHex(players[i].GetColor()) + ">" + placement + "Player " + (players[i].GetPlayerNum() + 1) + ", rounds won: " + players[i].GetRounds() 
                   + "\nTotal score earned: " + players[i].GetTotalScore() + "\n</color>";
                if (i < players.Count - 1)
                    m_Text.text += "\n";
            }
        }
    }

    string ColorToHex(Color32 col)
    {
        string hex = col.r.ToString("X2") + col.g.ToString("X2") + col.b.ToString("X2");
        return hex;
    }
}
