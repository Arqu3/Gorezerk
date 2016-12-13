﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    public int m_PlayerNum = 1;
    public float m_Speed = 10.0f;
    public float m_JumpForce = 10.0f;
    public bool m_IsAirMovement = false;
    public float m_DampXAmount = 0.5f;
    public float m_HookTravelSpeed = 10.0f;
    public float m_TravelToHookSpeed = 10.0f;
    public float m_HookBreakDistance = 1.0f;
    public float m_HookCooldown = 0.5f;
    public float m_AttackTime = 0.3f;
    public float m_AttackCooldown = 0.5f;
    public float m_AttackRange = 1.0f;
    public float m_AttackStartDegree = 60.0f;

    //Component vars
    private Rigidbody2D m_Rigidbody;
    private SpriteRenderer m_Renderer;
    private Collider2D m_Collider;

    //Ground vars
    private bool m_IsOnGround = false;
    private bool m_IsInAir = true;

    //Air vars
    private float m_AirTime = 0.0f;

    //Movement vars
    private float m_Horizontal = 0.0f;

    //Raycast vars
    private string m_Tag;

    //Grappling hook vars
    private bool m_IsGrapple = false;
    private bool m_GrappleHit = false;
    private bool m_HasSetGrappleDir = false;
    private Vector3 m_GrappleDir = Vector3.zero;
    private bool m_IsGrappleCD = false;
    private float m_HookTimer = 0.0f;
    private Vector3 m_HitPosition = Vector3.zero;
    private GameObject m_Hook;
    private Transform m_HookRotation;

    //Attack vars
    private bool m_CanAttack = true;
    private bool m_IsAttacking = false;
    private float m_AttackTimer = 0.0f;
    private float m_AttackCDTimer = 0.0f;
    private float m_AttackDegree = 0.0f;
    private float m_AttackDirection = 1.0f;
    private float m_AttackOffset = 0.0f;
    private Transform m_AttackRotation;
    private AttackHitbox m_HitBox;

    //Input string vars
    private string m_JumpInput;
    private string m_HorizontalInput;
    private string m_AttackInput;
    private string m_GrappleInput;

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

        if (transform.FindChild("AttackRot"))
        {
            m_AttackRotation = transform.FindChild("AttackRot");
            m_AttackOffset = m_AttackRotation.FindChild("StartPoint").localPosition.x;
            m_HitBox = m_AttackRotation.FindChild("StartPoint").GetComponentInChildren<AttackHitbox>();
            if (m_HitBox)
            {
                m_HitBox.SetPlayer(this);
                m_HitBox.transform.localScale = new Vector3(m_AttackRange, m_HitBox.transform.localScale.y, m_HitBox.transform.localScale.z);
                m_HitBox.gameObject.SetActive(false);
            }
        }
        else
            Debug.Log(gameObject.name + " could not find attack!");

        //Assign input variables
        m_JumpInput = "P" + m_PlayerNum + "Jump";
        m_HorizontalInput = "P" + m_PlayerNum + "Horizontal";
        m_AttackInput = "P" + m_PlayerNum + "RightTrigger";
        m_GrappleInput = "P" + m_PlayerNum + "LeftTrigger";
    }

    void Update()
    {
        if (ControllerScene.GetRoundStart())
            m_Rigidbody.isKinematic = true;
        else
            m_Rigidbody.isKinematic = false;

        if (!ControllerScene.GetPaused())
        {
            MovementUpdate();
            GrappleUpdate();
            AttackUpdate();
        }
        else
            m_Rigidbody.gravityScale = 0.0f;

        Debug.DrawRay(transform.position, m_Rigidbody.velocity.normalized * 1.5f, Color.blue);
    }

    void MovementUpdate()
    {
        if (transform.position.y < -10.0f)
            transform.position = Vector3.zero;

        m_IsInAir = m_Rigidbody.velocity.y != 0;

        if (!WallCheck())
            m_Horizontal = Input.GetAxis(m_HorizontalInput);

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

        if (Input.GetAxis(m_JumpInput) != 0.0f)
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
                m_IsGrapple = Input.GetAxis(m_GrappleInput) != 0;

                if (m_IsGrapple && !m_GrappleHit)
                {
                    if (!m_HasSetGrappleDir)
                    {
                        Vector2 dir = m_Rigidbody.velocity.normalized;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        m_HookRotation.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        m_HasSetGrappleDir = true;
                    }

                    m_Hook.transform.localScale += new Vector3(m_HookTravelSpeed, 0, 0) * Time.deltaTime;
                    m_Hook.transform.localPosition = new Vector3(m_Hook.transform.localScale.x / 2.0f, 0, 0);

                    Debug.DrawRay(m_Hook.transform.FindChild("Endpoint").position, (m_Hook.transform.FindChild("Endpoint").position - transform.position).normalized * 0.3f, Color.red);
                    m_GrappleDir = (m_Hook.transform.FindChild("Endpoint").position - transform.position).normalized;

                    RaycastHit2D hit = Physics2D.Raycast(m_Hook.transform.FindChild("Endpoint").position, (m_Hook.transform.FindChild("Endpoint").position - transform.position).normalized, 0.3f);
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
                        InterruptGrapple();
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

    void AttackUpdate()
    {
        if (m_CanAttack)
        {
            m_IsAttacking = Input.GetAxis(m_AttackInput) != 0.0f;

            if (m_Rigidbody.velocity.x != 0.0f)
            {
                if (m_Rigidbody.velocity.x > 0)
                    m_AttackDirection = 1.0f;
                else
                    m_AttackDirection = -1.0f;
            }

            if (m_IsAttacking)
            {
                m_AttackDegree = m_AttackStartDegree;
                m_AttackRotation.FindChild("StartPoint").localPosition = new Vector3(m_AttackOffset * m_AttackDirection, 0f, 0f);
                m_AttackRotation.localEulerAngles = new Vector3(0, 0, m_AttackDegree * m_AttackDirection);
                m_HitBox.gameObject.SetActive(true);
                m_HitBox.transform.localPosition = new Vector3(m_HitBox.transform.localScale.x / 2.0f * m_AttackDirection, 0f, 0f);
                m_AttackTimer = 0.0f;
                m_CanAttack = false;
            }
        }
        else
        {
            if (m_AttackCDTimer < m_AttackCooldown)
                m_AttackCDTimer += Time.deltaTime;
            else
            {
                m_AttackCDTimer = 0.0f;
                m_CanAttack = true;
            }

            if (m_AttackTimer < m_AttackTime)
            {
                m_AttackTimer += Time.deltaTime;
                m_AttackDegree -= 1.0f / m_AttackTime;
                m_AttackRotation.localEulerAngles = new Vector3(0, 0, m_AttackDegree * m_AttackDirection);


                //Debug.DrawRay(m_AttackRotation.FindChild("StartPoint").position, (m_AttackRotation.FindChild("StartPoint").position - transform.position).normalized * m_AttackRange, Color.red);
                //Quaternion rot = Quaternion.AngleAxis(m_AttackDegree * m_AttackDirection, Vector3.forward);
                //Debug.DrawRay(transform.position, rot * transform.right * m_AttackDirection * m_AttackRange, Color.red);
            }
            else
                m_HitBox.gameObject.SetActive(false);
        }
    }

    bool GroundCheck()
    {
        Color col = Color.green;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(m_Collider.bounds.size.x / 4, m_Collider.bounds.size.y / 2 * 1.2f, 0), Vector2.right, 0.5f);
        if (hit)
        {
            m_IsOnGround = true;
            col = Color.red;
        }
        else
            m_IsOnGround = false;

        Debug.DrawRay(transform.position - new Vector3(m_Collider.bounds.size.x / 4.0f, m_Collider.bounds.size.y / 2 * 1.2f, 0), Vector2.right * 0.5f, col);

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
            switch (i)
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
                if (i == 1)
                    m_Horizontal = Mathf.Clamp(Input.GetAxis(m_HorizontalInput), 0.0f, 1.0f);
                else
                    m_Horizontal = Mathf.Clamp(Input.GetAxis(m_HorizontalInput), -1.0f, 0.0f);

                if (m_GrappleHit)
                {
                    if (i == 1 && m_GrappleDir.x < 0)
                        InterruptGrapple();
                    else if (i == 0 && m_GrappleDir.x > 0)
                        InterruptGrapple();
                }

                col = Color.red;
                wallhit = true;
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

    public void ResetValues()
    {
        m_Rigidbody.velocity = Vector2.zero;
        //Grappling hook vars
        m_IsGrapple = false;
        m_GrappleHit = false;
        m_HasSetGrappleDir = false;
        m_GrappleDir = Vector3.zero;
        m_IsGrappleCD = false;
        m_HookTimer = 0.0f;
        m_HitPosition = Vector3.zero;
        m_Hook.transform.localPosition = Vector3.zero;
        m_Hook.transform.localScale = new Vector3(0, m_Hook.transform.localScale.y, m_Hook.transform.localScale.z);

        //Attack vars
        m_CanAttack = true;
        m_IsAttacking = false;
        m_AttackTimer = 0.0f;
        m_AttackCDTimer = 0.0f;
        m_AttackDegree = 0.0f;
        m_AttackDirection = 1.0f;
        m_AttackOffset = 0.0f;
        m_HitBox.gameObject.SetActive(false);
    }

    public void Kill()
    {
        ControllerScene.ReducePlayerCount();
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (m_GrappleHit)
        {
            InterruptGrapple();
        }
    }
}
