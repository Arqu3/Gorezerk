using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ControllerHook : MonoBehaviour
{
    //Public vars
    public LayerMask m_LayerMask;
    public float m_SkinWidth = 0.1f;

    //Component vars
    private Rigidbody2D m_Rigidbody;
    private Collider2D m_Collider;

    //Raycasting vars
    private float m_MinimumExtent = 0.0f;
    private float m_PartialExtent = 0.0f;
    private float m_sqrMinimumExtent = 0.0f;
    private Vector2 m_LastPosition = Vector2.zero;

    //Hit vars
    private bool m_Hit = false;
    private GameObject m_IgnorePlayer;
    private GameObject m_IgnoreHead;
    private string m_HitTag1 = "";
    private string m_HitTag2 = "";

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        m_LastPosition = m_Rigidbody.position;
        m_MinimumExtent = Mathf.Min(m_Collider.bounds.extents.x, m_Collider.bounds.extents.y);
        m_PartialExtent = m_MinimumExtent * (1.0f - m_SkinWidth);
        m_sqrMinimumExtent = m_MinimumExtent * m_MinimumExtent;
    }

    void FixedUpdate()
    {
        if ((m_LastPosition - m_Rigidbody.position).sqrMagnitude > m_sqrMinimumExtent)
        {
            Vector2 movementStep = m_Rigidbody.position - m_LastPosition;
            float movementMagnitude = movementStep.magnitude;
            RaycastHit2D hit = Physics2D.Raycast(m_LastPosition, movementStep, movementMagnitude, m_LayerMask);

            if (hit)
            {
                if (hit.collider.gameObject != m_IgnorePlayer && hit.collider.gameObject != m_IgnoreHead)
                {
                    m_Rigidbody.position = hit.point - (movementStep / movementMagnitude) * m_PartialExtent;
                    m_Rigidbody.velocity = Vector2.zero;
                    m_Hit = true;

                    if (hit.collider.gameObject.tag == m_HitTag1 || hit.collider.gameObject.tag == m_HitTag2)
                    {
                        //m_HookClone.GetComponent<Rigidbody2D>().simulated = false;
                        m_Rigidbody.simulated = false;
                        //m_HookClone.transform.SetParent(hit.collider.gameObject.transform);
                        transform.SetParent(hit.collider.gameObject.transform);
                        //Debug.Log("parent set");
                    }
                }
            }
        }

        m_LastPosition = m_Rigidbody.position;
    }

    public bool GetHit()
    {
        return m_Hit;
    }

    public void SetIgnores(GameObject player, GameObject head)
    {
        m_IgnorePlayer = player;
        m_IgnoreHead = head;
        m_HitTag1 = player.gameObject.tag;
        m_HitTag2 = head.gameObject.tag;
    }
}
