using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ParryHitbox : MonoBehaviour
{
    [Header("On parry spawn")]
    public GameObject m_ParticlePrefab;

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
                //m_Player.InterruptAttack();
                m_Player.GetRigidbody().AddForce(dir * m_Player.m_ParryForce, ForceMode2D.Impulse);

                if (m_ParticlePrefab)
                    Instantiate(m_ParticlePrefab, transform.position, Quaternion.identity);
            }
            else if (col.gameObject.GetComponent<HomingMissile>())
                col.gameObject.GetComponent<HomingMissile>().Reflect(col.gameObject.transform.position - m_Player.transform.position);
        }
    }
}
