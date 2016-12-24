using UnityEngine;
using System.Collections;

public enum ControllerType
{
    Xbox,
    PS,
    Keyboard
}

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
    public float m_ParryTime = 0.3f;
    public float m_ParryForce = 20.0f;

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

    //Score vars
    private int m_Score = 0;

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
    private Transform m_HookEndpoint;
    private Quaternion m_LocalHookRot;

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
    private const float m_AttackStartDegree = 60.0f;

    //Parry vars
    private bool m_IsParry = false;
    private float m_ParryTimer = 0.0f;

    //Input vars
    private ControllerType m_ControllerType = ControllerType.Xbox;
    private int m_ControllerNum = 0;

    //Input string vars
    private string m_JumpInput;
    private string m_HorizontalInput;
    private string m_AttackInput;
    private string m_GrappleInput;
    private float m_InputValue = 0.0f;

    //Keyboard
    private KeyCode m_JumpInputK;
    private KeyCode m_HorizontalInputK1;
    private KeyCode m_HorizontalInputK2;
    private KeyCode m_AttackInputK;
    private KeyCode m_GrappleInputK;

    void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<Collider2D>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Tag = gameObject.tag;
        m_HookTimer = m_HookCooldown;

        if (transform.FindChild("HookRot"))
            m_HookRotation = transform.FindChild("HookRot");

        if (m_HookRotation.FindChild("GrapplingHook"))
        {
            m_Hook = m_HookRotation.FindChild("GrapplingHook").gameObject;
            m_HookEndpoint = m_Hook.transform.FindChild("Endpoint");
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

        //Assign input variable values from toolbox
        m_ControllerNum = Toolbox.Instance.m_Information[m_PlayerNum].GetControllerNum();
        m_Renderer.color = Toolbox.Instance.m_Colors[m_PlayerNum];
        m_ControllerType = Toolbox.Instance.m_Information[m_PlayerNum].GetCType();

        //Assign input variables
        m_JumpInput = "P" + m_ControllerNum + "Jump";
        m_HorizontalInput = "P" + m_ControllerNum + "Horizontal";
        m_AttackInput = "P" + m_ControllerNum + "RightTrigger";
        m_GrappleInput = "P" + m_ControllerNum + "LeftTrigger";

        if (GetControllerType().Equals(ControllerType.PS))
        {
            m_JumpInput += "PS";
            m_AttackInput += "PS";
            m_GrappleInput += "PS";
            m_InputValue = -1.0f;
        }

        //Bad case, remove later
        if (m_ControllerType.Equals(ControllerType.Keyboard))
        {
            m_JumpInputK = KeyCode.Space;
            m_HorizontalInputK1 = KeyCode.A;
            m_HorizontalInputK2 = KeyCode.D;
            m_AttackInputK = KeyCode.L;
            m_GrappleInputK = KeyCode.P;
        }
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
        if (transform.position.y < -40.0f)
            transform.position = Vector3.zero;

        m_IsInAir = m_Rigidbody.velocity.y != 0;

        if (!WallCheck())
        {
            if (!m_ControllerType.Equals(ControllerType.Keyboard))
                m_Horizontal = Input.GetAxis(m_HorizontalInput);
            else
            {
                if (Input.GetKey(m_HorizontalInputK1))
                    m_Horizontal = -1.0f;
                else if (Input.GetKey(m_HorizontalInputK2))
                    m_Horizontal = 1.0f;
                else
                    m_Horizontal = 0.0f;
            }
        }

        if (!m_IsAirMovement)
        {
            if (!m_IsParry)
            {
                if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                    m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed, m_Rigidbody.velocity.y);
            }
        }
        else
        {
            if (!m_IsParry)
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
        }

        if (!m_ControllerType.Equals(ControllerType.Keyboard))
        {
            if (Input.GetAxis(m_JumpInput) != 0.0f)
            {
                if (GroundCheck() && !m_IsInAir)
                    Jump();
            }
        }
        else
        {
            if (Input.GetKey(m_JumpInputK))
            {
                if (GroundCheck() && !m_IsInAir)
                    Jump();
            }
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

    void Jump()
    {
        m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
    }

    void GrappleUpdate()
    {
        if (m_Hook)
        {
            m_Hook.transform.localScale = new Vector3(Mathf.Clamp(m_Hook.transform.localScale.x, 0.0f, 1000.0f), m_Hook.transform.localScale.y, m_Hook.transform.localScale.z);
            if (!m_IsGrappleCD)
            {
                if (!m_ControllerType.Equals(ControllerType.Keyboard))
                    m_IsGrapple = Input.GetAxis(m_GrappleInput) != m_InputValue;
                else
                    m_IsGrapple = Input.GetKey(m_GrappleInputK);

                if (m_IsGrapple && !m_GrappleHit)
                {
                    if (!m_HasSetGrappleDir && m_Rigidbody.velocity.magnitude > 0.1f)
                    {
                        Vector2 dir = m_Rigidbody.velocity.normalized;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        m_LocalHookRot = Quaternion.AngleAxis(angle, Vector3.forward);
                        m_HookRotation.transform.localRotation = m_LocalHookRot;

                        m_HasSetGrappleDir = true;
                    }

                    m_Hook.transform.localScale += new Vector3(m_HookTravelSpeed, 0, 0) * Time.deltaTime;
                    m_Hook.transform.localPosition = new Vector3(m_Hook.transform.localScale.x / 2.0f, 0, 0);

                    Debug.DrawRay(m_HookEndpoint.position, (m_HookEndpoint.position - transform.position).normalized * 0.3f, Color.red);
                    m_GrappleDir = (m_HookEndpoint.position - transform.position).normalized;

                    RaycastHit2D hit = Physics2D.Raycast(m_HookEndpoint.position, (m_HookEndpoint.position - transform.position).normalized, 1.5f);
                    if (hit)
                    {
                        if (hit.collider.gameObject != this.gameObject)
                        {
                            if (hit.collider.tag != m_Tag)
                            {
                                m_HitPosition = hit.point;
                                m_GrappleHit = true;
                            }
                            else
                            {
                                if (hit.collider.gameObject.GetComponent<ControllerPlayer>())
                                {
                                    hit.collider.gameObject.GetComponent<ControllerPlayer>().Kill();
                                    AddScore(1);
                                    InterruptGrapple();
                                }
                            }
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

                        if (!m_ControllerType.Equals(ControllerType.Keyboard))
                        {
                            if (Input.GetAxis(m_JumpInput) != 0.0f)
                            {
                                InterruptGrapple();
                                Jump();
                            }
                        }
                        else
                        {
                            if (Input.GetKey(m_JumpInputK))
                            {
                                InterruptGrapple();
                                Jump();
                            }
                        }
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
        if (m_IsParry)
        {
            if (m_ParryTimer < m_ParryTime)
                m_ParryTimer += Time.deltaTime;
            else
            {
                m_ParryTimer = 0.0f;
                m_IsParry = false;
            }
        }

        if (m_CanAttack)
        {
            if (!m_ControllerType.Equals(ControllerType.Keyboard))
                m_IsAttacking = Input.GetAxis(m_AttackInput) != m_InputValue;
            else
                m_IsAttacking = Input.GetKey(m_AttackInputK);

            if (m_Rigidbody.velocity.x != 0.0f && !WallCheck())
            {
                if (m_Horizontal > 0)
                    m_AttackDirection = 1.0f;
                else if (m_Horizontal < 0)
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
                m_AttackDegree -= m_AttackStartDegree * 2 / m_AttackTime * Time.deltaTime;
                m_AttackDegree = Mathf.Clamp(m_AttackDegree, -m_AttackStartDegree, m_AttackStartDegree);
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

            if (hit.collider.tag == "HeadCollider")
            {
                if (hit.collider.transform.parent.GetComponent<ControllerPlayer>())
                {
                    hit.collider.transform.parent.GetComponent<ControllerPlayer>().Kill();
                    AddScore(1);
                }
            }
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
                if (hit.collider.tag != "AttackBox")
                {
                    if (!m_ControllerType.Equals(ControllerType.Keyboard))
                    {
                        if (i == 1)
                            m_Horizontal = Mathf.Clamp(Input.GetAxis(m_HorizontalInput), 0.0f, 1.0f);
                        else
                            m_Horizontal = Mathf.Clamp(Input.GetAxis(m_HorizontalInput), -1.0f, 0.0f);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (Input.GetKey(m_HorizontalInputK1))
                                m_Horizontal = -1.0f;
                            else
                                m_Horizontal = 0.0f;
                        }
                        else
                        {
                            if (Input.GetKey(m_HorizontalInputK2))
                                m_Horizontal = 1.0f;
                            else
                                m_Horizontal = 0.0f;
                        }
                    }

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
            }
            Debug.DrawRay(new Vector3(x, transform.position.y + (-m_Collider.bounds.size.y / 2.0f) * 0.8f, 0.0f), Vector3.up * m_Collider.bounds.size.y * 0.8f, col);
        }

        return wallhit;
    }

    void InterruptGrapple()
    {
        m_IsGrappleCD = true;
        m_Rigidbody.velocity *= 0.4f;
        //Debug.Log("Grapple interrupt!");
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

    public int GetPlayerNum()
    {
        return m_PlayerNum;
    }

    public void AddScore(int score)
    {
        if (score > 0)
            ControllerScene.SetScoreBark("Player" + (m_PlayerNum + 1));

        m_Score += score;
    }

    public int GetScore()
    {
        return m_Score;
    }

    public void SetControllerType(ControllerType newType)
    {
        m_ControllerType = newType;
    }

    public ControllerType GetControllerType()
    {
        return m_ControllerType;
    }

    public void InterruptAttack()
    {
        m_CanAttack = false;
        m_AttackTimer = 0.0f;
        m_HitBox.gameObject.SetActive(false);
    }

    public void SetParry(bool state)
    {
        m_IsParry = state;
    }

    public Rigidbody2D GetRigidbody()
    {
        return m_Rigidbody;
    }
}
