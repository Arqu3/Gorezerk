using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : Projectile
{
    //Public vars
    public float m_PlayerHitMultiplier = 0.1f;
    public float m_ReflectTime = 0.3f;

    //Speed vars
    private float m_CurSpeed = 0.0f;

    //Target vars
    private Transform m_Target;
    private Vector3 m_Direction = Vector3.zero;
    private float m_ReflectTimer = 0.0f;
    private bool m_IsReflected = false;

	protected override void Start()
    {
        base.Start();
        m_CurSpeed = m_Speed;
        m_ReflectTimer = m_ReflectTime;
	}
	
	void Update()
    {
        if (m_Target)
        {
            if (m_IsReflected)
            {
                m_ReflectTimer += Time.deltaTime;
                if (m_ReflectTimer >= m_ReflectTime)
                {
                    SetTarget(FindNearest());
                    m_ReflectTimer = 0.0f;
                    m_IsReflected = false;
                }
            }

            m_Direction = (m_Target.position + new Vector3(0f, 1f, 0f) - transform.position).normalized;
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Direction * m_CurSpeed, 5 * Time.deltaTime);
        }
        else
        {
            Debug.Log("Homing missile does not have a target!");
            enabled = false;
            return;
        }
    }

    public void Reflect(Vector3 dir)
    {
        m_Rigidbody.AddForce(dir.normalized * m_CurSpeed * 4f, ForceMode2D.Impulse);
        m_CurSpeed += m_Speed * m_PlayerHitMultiplier;
        m_IsReflected = true;
    }

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    Transform FindNearest()
    {
        var players = FindObjectsOfType<ControllerPlayer>();
        Transform min = null;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < players.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < minDist)
            {
                min = players[i].transform;
                minDist = dist;
            }
        }
        return min;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (Vector3.Angle(m_Direction, -col.contacts[0].normal) <= 60)
            m_Rigidbody.AddForce(Vector3.Reflect(m_Direction, col.contacts[0].normal) * m_Speed * 1.5f, ForceMode2D.Impulse);

        if (col.gameObject.GetComponent<ControllerPlayer>())
        {
            col.gameObject.GetComponent<ControllerPlayer>().Kill();
            Destroy(gameObject);
        }

        //This does not work, fix sometime?
        //if (col.gameObject.GetComponent<AttackHitbox>())
        //{
        //    Debug.Log(1);
        //    m_CurSpeed *= 1f + m_PlayerHitMultiplier;
        //}
    }
}
