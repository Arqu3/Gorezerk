using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    public float m_Speed = 10.0f;
    public float m_JumpForce = 10.0f;
    public bool m_IsAirMovement = false;
    public float m_DampXAmount = 0.5f;
    public float m_HookTravelSpeed = 10.0f;
    public float m_TravelToHookSpeed = 10.0f;
    public float m_HookBreakDistance = 1.0f;
    public float m_HookCooldown = 0.5f;

    //Component vars
    Rigidbody2D m_Rigidbody;
    SpriteRenderer m_Renderer;
    Collider2D m_Collider;

    //Ground vars
    bool m_IsOnGround = false;
    public bool m_IsInAir = true;

    //Air vars
    float m_AirTime = 0.0f;

    //Movement vars
    float m_Horizontal = 0.0f;

    //Raycast vars
    string m_Tag;

    //Grappling hook vars
    bool m_IsGrapple = false;
    bool m_GrappleHit = false;
    bool m_HasSetGrappleDir = false;
    bool m_IsGrappleCD = false;
    float m_HookTimer = 0.0f;
    Vector3 m_HitPosition = Vector3.zero;
    GameObject m_Hook;
    Transform m_HookRotation;

    void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<Collider2D>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Tag = gameObject.tag;
        m_HookTimer = m_HookCooldown;
        //Debug.Log(Input.GetJoystickNames()[0]);

        if (transform.FindChild("HookRot"))
            m_HookRotation = transform.FindChild("HookRot");

        if (m_HookRotation.FindChild("GrapplingHook"))
        {
            m_Hook = m_HookRotation.FindChild("GrapplingHook").gameObject;
            m_Hook.transform.localScale = new Vector3(0, m_Hook.transform.localScale.y, m_Hook.transform.localScale.z);
        }
        else
            Debug.Log(gameObject.name + " could not find hook!");
    }
	
	void Update ()
    {
        MovementUpdate();
        GrappleUpdate();
        WallCheck();

        Debug.DrawRay(transform.position, m_Rigidbody.velocity.normalized * 1.5f);
	}

    void MovementUpdate()
    {
        if (transform.position.y < -10.0f)
            transform.position = Vector3.zero;

        m_IsInAir = m_Rigidbody.velocity.y != 0;

        m_Horizontal = Input.GetAxis("Horizontal");

        if (!m_IsAirMovement)
        {
            if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed, m_Rigidbody.velocity.y);
        }
        else
        {
            if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed, m_Rigidbody.velocity.y);
            else
            {
                if (!GroundCheck())
                {
                    if (m_Horizontal > 0.5f || m_Horizontal < -0.5f)
                        m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed * m_DampXAmount, m_Rigidbody.velocity.y);
                }
                else
                    m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed, m_Rigidbody.velocity.y);
            }
        }

        if (Input.GetAxis("Fire1") != 0.0f)
        {
            if (GroundCheck() && !m_IsInAir)
                m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        }

        if (GroundCheck())
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

    void GrappleUpdate()
    {
        if (m_Hook)
        {
            if (!m_IsGrappleCD)
            {
                m_IsGrapple = Input.GetAxis("LeftTrigger") != 0;

                if (m_IsGrapple && !m_GrappleHit)
                {
                    if (!m_HasSetGrappleDir)
                    {
                        Vector2 dir = m_Rigidbody.velocity;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        m_HookRotation.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        m_HasSetGrappleDir = true;
                    }

                    m_Hook.transform.localScale += new Vector3(m_HookTravelSpeed, 0, 0) * Time.deltaTime;
                    m_Hook.transform.localPosition = new Vector3(m_Hook.transform.localScale.x / 2.0f, 0, 0);

                    Debug.DrawRay(m_Hook.transform.Find("Endpoint").position, (m_Hook.transform.FindChild("Endpoint").position - transform.position).normalized * 0.3f, Color.red);

                    RaycastHit2D hit = Physics2D.Raycast(m_Hook.transform.Find("Endpoint").position, (m_Hook.transform.FindChild("Endpoint").position - transform.position).normalized, 0.3f);
                    if (hit)
                    {
                        if (hit.collider.tag != m_Tag)
                        {
                            m_HitPosition = hit.point;
                            m_GrappleHit = true;
                        }
                    }
                }
                else if (m_GrappleHit)
                {
                    if (Vector3.Distance(transform.position, m_HitPosition) > m_HookBreakDistance)
                    {
                        m_Rigidbody.gravityScale = 0.0f;
                        m_Hook.transform.localScale -= new Vector3(m_TravelToHookSpeed, 0, 0) * Time.deltaTime;
                        m_Hook.transform.localPosition = new Vector3(m_Hook.transform.localScale.x / 2.0f, 0, 0);
                        m_Rigidbody.velocity = (m_HitPosition - transform.position).normalized * m_TravelToHookSpeed;

                        if (!m_IsGrapple)
                            InterruptGrapple();
                    }
                    else
                    {
                        m_Rigidbody.gravityScale = 1.0f;
                        m_IsGrappleCD = true;
                    }
                }
                else
                {
                    m_Hook.transform.localPosition = Vector3.zero;
                    m_Hook.transform.localScale = new Vector3(0, m_Hook.transform.localScale.y, m_Hook.transform.localScale.z);
                    m_HasSetGrappleDir = false;
                }
            }
            else
            {
                m_Hook.transform.localPosition = Vector3.zero;
                m_Hook.transform.localScale = new Vector3(0, m_Hook.transform.localScale.y, m_Hook.transform.localScale.z);
                m_HasSetGrappleDir = false;
                m_GrappleHit = false;

                if (m_HookTimer >= 0.0f)
                    m_HookTimer -= Time.deltaTime;
                else
                {
                    m_IsGrappleCD = false;
                    m_HookTimer = m_HookCooldown;
                }
            }
        }
    }

    bool GroundCheck()
    {
        Debug.DrawRay(transform.position - new Vector3(m_Collider.bounds.size.x / 4.0f, m_Collider.bounds.size.y / 2 * 1.1f, 0), Vector2.right * 0.5f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(m_Collider.bounds.size.x / 4, m_Collider.bounds.size.y / 2 * 1.1f, 0), Vector2.right, 0.5f);
        if (hit)
        {
            if (hit.collider.tag != m_Tag)
                m_IsOnGround = true;
            else
                m_IsOnGround = false;
        }
        else
            m_IsOnGround = false;

        return m_IsOnGround;
    }

    bool WallCheck()
    {
        //Vector3 pos = new Vector3(transform.position.x + m_Collider.bounds.size.x / 2, transform.position.y + ((-m_Collider.bounds.size.y / 2.0f) * 0.8f), 0);
        //Debug.DrawRay(pos, Vector3.up * m_Collider.bounds.size.y * 0.8f);

        float x = 0.0f;
        bool wallhit = false;

        for (int i = 0; i < 2; i++)
        {
            switch(i)
            {
                case 0:
                    x = transform.position.x + m_Collider.bounds.size.x / 2.0f * 1.2f;
                    break;

                case 1:
                    x = transform.position.x - m_Collider.bounds.size.x / 2.0f * 1.2f;
                    break;
            }
            Color col = Color.green;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, transform.position.y + (-m_Collider.bounds.size.y / 2.0f) * 0.8f), Vector2.up, m_Collider.bounds.size.y * 0.8f);
            if (hit)
            {
                if (hit.collider.tag == "Wall")
                {
                    col = Color.red;
                    wallhit = true;
                }
            }
            Debug.DrawRay(new Vector3(x, transform.position.y + (-m_Collider.bounds.size.y / 2.0f) * 0.8f, 0.0f), Vector3.up * m_Collider.bounds.size.y * 0.8f, col);
        }

        return wallhit;
    }

    void InterruptGrapple()
    {
        m_IsGrappleCD = true;
        m_Rigidbody.velocity *= 0.4f;
        Debug.Log("Grapple interrupt!");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (m_GrappleHit)
        {
            InterruptGrapple();
        }
    }

    //void OnCollisionStay2D(Collision2D col)
    //{
    //    if (m_GrappleHit)
    //    {
    //        Debug.Log("Grapple interrupt!");
    //        m_IsGrappleCD = true;
    //    }
    //}
}
