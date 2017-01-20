using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ParryHitbox : MonoBehaviour
{

    //Component vars
    ControllerPlayer m_Player;

    public void SetPlayer(ControllerPlayer player)
    {
        if (!m_Player)
            m_Player = player;
    }

    public ControllerPlayer GetPlayer()
    {
        return m_Player;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (m_Player)
        {
            if (col.gameObject.GetComponent<ParryHitbox>())
            {
                Vector2 dir = (m_Player.transform.position - col.gameObject.GetComponent<ParryHitbox>().GetPlayer().transform.position).normalized;
                m_Player.SetParry(true);
                m_Player.InterruptAttack();
                m_Player.GetRigidbody().AddForce(dir * m_Player.m_ParryForce, ForceMode2D.Impulse);
            }
        }
    }
}
