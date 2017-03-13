using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Trap : MonoBehaviour
{

    //Component vars
    private Rigidbody2D m_Rigidbody;
    private Collider2D m_Collider;

	void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        RaycastHit2D hit;
        Vector3 pos = transform.position;
        pos.y -= m_Collider.bounds.extents.y * 1.1f;
        hit = Physics2D.Raycast(pos, -transform.up, 5.0f);
        if (hit)
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + m_Collider.bounds.extents.y);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!m_Rigidbody.isKinematic)
            m_Rigidbody.isKinematic = true;

        m_Rigidbody.velocity = Vector2.zero;

        ControllerPlayer player = col.gameObject.GetComponent<ControllerPlayer>();
        if (player)
            player.Kill();
    }
}
