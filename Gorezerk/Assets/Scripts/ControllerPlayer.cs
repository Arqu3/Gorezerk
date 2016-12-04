using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    public float m_Speed = 10.0f;
    public float m_JumpForce = 10.0f;
    public bool m_IsAirMovement = false;
    public float m_DampXAmount = 0.5f;

    //Component vars
    Rigidbody2D m_Rigidbody;

    //Ground vars
    bool m_IsOnGround = false;
    public bool m_IsInAir = true;

    //Air vars
    float m_AirTime = 0.0f;

	void Start ()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        MovementUpdate();
        Debug.DrawRay(transform.position, Vector2.down * 1.0f, Color.red);
	}

    void MovementUpdate()
    {
        if (transform.position.y < -10.0f)
            transform.position = Vector3.zero;

        m_IsOnGround = Physics2D.Raycast(transform.position, Vector2.down, 1.0f);
        m_IsInAir = m_Rigidbody.velocity.y != 0;

        if (!m_IsAirMovement)
        {
            if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                m_Rigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * m_Speed, m_Rigidbody.velocity.y);
        }
        else
        {
            if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                m_Rigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * m_Speed, m_Rigidbody.velocity.y);
            else
            {
                if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
                    m_Rigidbody.velocity = new Vector2(Input.GetAxis("Horizontal") * m_Speed * m_DampXAmount, m_Rigidbody.velocity.y);
            }
        }

        if (Input.GetAxis("Fire1") != 0.0f)
        {
            if (m_IsOnGround && !m_IsInAir)
                m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        }

        if (m_IsOnGround)
        {
            m_Rigidbody.gravityScale = 0.0f;
            m_AirTime = 0.0f;
        }
        else
        {
            m_Rigidbody.gravityScale = 1.0f;
            m_AirTime += Time.deltaTime;
        }
    }

    //bool CheckValidX()
    //{
    //    Vector2 dir = Vector2.zero;
    //    for (int i = 0; i < 6; i++)
    //    {
    //        switch(i)
    //        {
    //            case 1:
    //                dir = new Vector2()
    //                break;

    //            case 2:
    //                break;

    //            case 3:
    //                break;

    //            case 4:
    //                break;

    //            case 5:
    //                break;

    //            case 6:
    //                break;
    //        }

    //        if (Physics2D.Raycast(transform.position, dir, 1.0f))
    //            return false;
    //    }

    //    return true;
    //}
}
