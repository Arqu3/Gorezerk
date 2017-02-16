using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public enum ControllerType
{
    Xbox,
    XboxOne,
    PS,
    Keyboard
}

/// <summary>
/// This is the complete controller for any player, it handles all input that the players recieve when the game is active
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(MeshRenderer))]
public class ControllerPlayer : MonoBehaviour
{
    //Public vars
    [Header("Movement variables")]
    [Range(1f, 100f)]
    public float m_Speed = 10.0f;
    [Range(1f, 20f)]
    public float m_Acceleration = 10.0f;

    [Header("Jump/air variables")]
    [Range(1f, 100f)]
    public float m_JumpForce = 10.0f;
    [Range(0.1f, 1.0f)]
    public float m_FallGraceTime = 0.75f;
    public bool m_IsAirMovement = false;
    [Range(0.1f, 2.0f)]
    public float m_DampXAmount = 0.5f;

    [Header("Hook variables")]
    [Range(1f, 200f)]
    public float m_HookTravelSpeed = 10.0f;
    [Range(1f, 200f)]
    public float m_TravelToHookSpeed = 10.0f;
    [Range(0.1f, 10.0f)]
    public float m_HookBreakDistance = 1.0f;
    [Range(0.1f, 2.0f)]
    public float m_HookCooldown = 0.5f;

    [Header("Attack variables")]
    [Range(0.1f, 2.0f)]
    public float m_AttackTime = 0.3f;
    [Range(0.1f, 2.0f)]
    public float m_AttackCooldown = 0.5f;
    [Range(0.1f, 5.0f)]
    public float m_AttackRange = 1.0f;
    [Range(0.1f, 2.0f)]
    public float m_ParryTime = 0.3f;
    [Range(1f, 100f)]
    public float m_ParryForce = 20.0f;

    [Header("Spawnable prefabs")]
    public GameObject m_HookPrefab;

    [Header("Layermasks")]
    //Layermasks
    public LayerMask m_HookMask;
    public LayerMask m_GroundMask;

    //Component vars
    private Rigidbody2D m_Rigidbody;
    private MeshRenderer m_Renderer;
    private Collider2D m_Collider;

    //Ground vars
    private bool m_IsOnGround = false;
    private bool m_IsInAir = true;
    private float m_FallGraceTimer = 0.0f;
    private bool m_IsJump = false;
    private bool m_CanAirJump = false;

    //General player vars
    private int m_PlayerNum = 1;

    //Movement vars
    private float m_Horizontal = 0.0f;

    //Score vars
    private int m_Score = 0;

    //Grappling hook vars
    private bool m_IsGrapple = false;
    private bool m_GrappleHit = false;
    private bool m_HasSetGrappleDir = false;
    private bool m_HasReachedHook = false;
    private Vector3 m_GrappleDir = Vector3.zero;
    private float m_HookTimer = 0.0f;
    private GameObject m_HookLine;
    private Transform m_PointerRotation;
    private Transform m_Pointer;
    private bool m_CanShootHook = true;
    private GameObject m_HookClone;
    private ControllerHook m_HookController;
    private Vector2 m_Aim = Vector2.zero;
    private bool m_IsHookCD = false;

    //Attack vars
    private bool m_CanAttack = true;
    private bool m_IsAttacking = false;
    private float m_AttackTimer = 0.0f;
    private float m_AttackCDTimer = 0.0f;
    private float m_AttackDegree = 0.0f;
    private float m_AttackDirection = 1.0f;
    private float m_AttackOffset = 0.0f;
    private Transform m_AttackRotation;
    private AttackHitbox m_AttackBox;
    private ParryHitbox m_ParryBox;
    private const float m_AttackStartDegree = 60.0f;

    //Parry vars
    private bool m_IsParry = false;
    private float m_ParryTimer = 0.0f;

    //Head vars
    private GameObject m_Head;

    //Input vars
    private ControllerType m_ControllerType = ControllerType.Xbox;
    private int m_ControllerNum = 0;

    //Input string vars
    private string m_JumpInput;
    private string m_HorizontalInput;
    private string m_VerticalInput;
    private string m_AttackInput;
    private string m_GrappleInput;
    PlayerIndex m_ControllerIndex;
    GamePadState m_GamePadState;

    //Keyboard
    private KeyCode m_JumpInputK;
    private KeyCode m_HorizontalInputK1;
    private KeyCode m_HorizontalInputK2;
    private KeyCode m_VerticalInputK1;
    private KeyCode m_VerticalInputK2;
    private KeyCode m_AttackInputK;
    private KeyCode m_GrappleInputK;

    void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Collider = GetComponent<Collider2D>();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_HookTimer = m_HookCooldown;

        if (transform.FindChild("HeadCollider"))
            m_Head = transform.FindChild("HeadCollider").gameObject;

        if (transform.FindChild("PointerRot"))
        {
            m_PointerRotation = transform.FindChild("PointerRot");
            if (m_PointerRotation.FindChild("Pointer"))
                m_Pointer = m_PointerRotation.FindChild("Pointer");
        }

        if (transform.FindChild("GrapplingHook"))
        {
            m_HookLine = transform.FindChild("GrapplingHook").gameObject;
            m_HookLine.transform.localScale = new Vector3(m_HookLine.transform.localScale.x, 0, m_HookLine.transform.localScale.z);
        }

        if (transform.FindChild("AttackRot"))
        {
            m_AttackRotation = transform.FindChild("AttackRot");
            m_AttackOffset = m_AttackRotation.FindChild("StartPoint").localPosition.x;
            m_AttackBox = m_AttackRotation.FindChild("StartPoint").GetComponentInChildren<AttackHitbox>();
            if (m_AttackBox)
            {
                m_AttackBox.SetPlayer(this);
                m_AttackBox.transform.localScale = new Vector3(m_AttackRange, m_AttackBox.transform.localScale.y, m_AttackBox.transform.localScale.z);
                m_AttackBox.gameObject.SetActive(false);
            }
        }
        else
            Debug.Log(gameObject.name + " could not find attack!");

        if (GetComponentInChildren<ParryHitbox>())
        {
            m_ParryBox = GetComponentInChildren<ParryHitbox>();
            m_ParryBox.SetPlayer(this);
            m_ParryBox.gameObject.SetActive(false);
        }
        else
            Debug.Log(gameObject.name + "could not find parry!");
    }

    void Start()
    {
        //Assign input variable values from toolbox
        m_ControllerNum = Toolbox.Instance.m_Information[m_PlayerNum].GetControllerNum();
        m_ControllerIndex = Toolbox.Instance.m_Information[m_PlayerNum].GetPlayerIndex();
        m_Renderer.material.color = Toolbox.Instance.m_Colors[m_PlayerNum];
        if (m_Head)
        {
            if (m_Head.GetComponent<MeshRenderer>())
                m_Head.GetComponent<MeshRenderer>().material.color = Toolbox.Instance.m_Colors[m_PlayerNum];
        }
        m_ControllerType = Toolbox.Instance.m_Information[m_PlayerNum].GetCType();

        //Setup xinput gamepad
        //PlayerIndex testIndex = (PlayerIndex)m_ControllerNum;
        //GamePadState testState = GamePad.GetState(testIndex);
        //if (testState.IsConnected)
        //{
        //    m_ControllerIndex = testIndex;
        //    Debug.Log(m_ControllerIndex + " " + m_ControllerNum);
        //}

        //Assign input variables
        m_JumpInput = "P" + m_ControllerNum + "Jump";
        m_HorizontalInput = "P" + m_ControllerNum + "Horizontal";
        m_VerticalInput = "P" + m_ControllerNum + "Vertical";
        m_AttackInput = "P" + m_ControllerNum + "RightTrigger";
        m_GrappleInput = "P" + m_ControllerNum + "LeftTrigger";

        if (GetControllerType().Equals(ControllerType.PS))
        {
            m_JumpInput += "PS";
            m_AttackInput += "PS";
            m_GrappleInput += "PS";
        }
        else if (GetControllerType().Equals(ControllerType.XboxOne))
        {
            m_AttackInput = "P" + m_ControllerNum + "LeftTriggerXOne";
            m_GrappleInput += "XOne";
        }

        //Bad case, remove later
        if (m_ControllerType.Equals(ControllerType.Keyboard))
        {
            m_JumpInputK = KeyCode.Space;
            m_HorizontalInputK1 = KeyCode.A;
            m_HorizontalInputK2 = KeyCode.D;
            m_VerticalInputK1 = KeyCode.S;
            m_VerticalInputK2 = KeyCode.W;
            m_AttackInputK = KeyCode.L;
            m_GrappleInputK = KeyCode.P;
        }
    }

    void Update()
    {
        if (ControllerScene.GetRoundStart())
        {
            m_Rigidbody.simulated = false;
            m_Rigidbody.isKinematic = true;
        }
        else
        {
            m_Rigidbody.simulated = true;
            m_Rigidbody.isKinematic = false;
        }

        if (!ControllerScene.GetPaused())
        {
            MovementUpdate();
            GrappleUpdate();
            AttackUpdate();
        }
        else
            m_Rigidbody.gravityScale = 0.0f;
    }

    void MovementUpdate()
    {
        if (transform.position.y < -40.0f)
            transform.position = Vector3.zero;

        m_IsInAir = Mathf.Round(m_Rigidbody.velocity.y) != 0;
        m_IsOnGround = GroundCheck();

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
                    m_Rigidbody.velocity = new Vector2(m_Horizontal * m_Speed * Toolbox.Instance.m_MovementSpeed, m_Rigidbody.velocity.y);
            }
        }
        else
        {
            if (!m_IsParry)
            {
                float totalSpeed = Mathf.Lerp(m_Rigidbody.velocity.x, m_Horizontal * m_Speed * Toolbox.Instance.m_MovementSpeed, m_Acceleration * Time.deltaTime);
                float totalAirSpeed = Mathf.Lerp(m_Rigidbody.velocity.x, m_Horizontal * m_Speed * Toolbox.Instance.m_MovementSpeed * m_DampXAmount, m_Acceleration * Time.deltaTime);

                if (Mathf.Approximately(m_Rigidbody.velocity.y, 0.0f))
                    m_Rigidbody.velocity = new Vector2(totalSpeed, m_Rigidbody.velocity.y);
                else
                {
                    if (!m_IsOnGround)
                    {
                        if (m_Horizontal > 0.5f || m_Horizontal < -0.5f)
                            m_Rigidbody.velocity = new Vector2(totalAirSpeed, m_Rigidbody.velocity.y);
                    }
                    else
                        m_Rigidbody.velocity = new Vector2(totalSpeed, m_Rigidbody.velocity.y);
                }
            }
        }

        if (!m_IsOnGround && !m_IsJump)
        {
            m_CanAirJump = true;
            m_FallGraceTimer += Time.deltaTime;
            if (m_FallGraceTimer >= m_FallGraceTime)
            {
                m_FallGraceTimer = 0.0f;
                m_CanAirJump = false;
                m_IsJump = true;
            }
        }

        if (!m_ControllerType.Equals(ControllerType.Keyboard))
        {
            if (Input.GetAxis(m_JumpInput) != 0.0f)
            {
                if ((m_IsOnGround && !m_IsInAir) || m_CanAirJump)
                    Jump();
            }
        }
        else
        {
            if (Input.GetKey(m_JumpInputK))
            {
                if ((m_IsOnGround && !m_IsInAir) || m_CanAirJump)
                    Jump();
            }
        }

        if (!m_GrappleHit)
        {
            if (m_IsOnGround)
                m_Rigidbody.gravityScale = 0.0f;
            else
                m_Rigidbody.gravityScale = 1.0f;
        }
    }

    void Jump()
    {
        m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
        m_IsJump = true;
        m_FallGraceTimer = 0.0f;
        m_CanAirJump = false;
    }

    void GrappleUpdate()
    {
        if (m_HookPrefab)
        {
            //Set aim
            if (!m_ControllerType.Equals(ControllerType.Keyboard))
            {
                //Controller
                if (Mathf.Abs(Input.GetAxis(m_HorizontalInput)) > 0.1f)
                    m_Aim.x = Input.GetAxis(m_HorizontalInput);

                if (Mathf.Abs(Input.GetAxis(m_VerticalInput)) > 0.1f)
                    m_Aim.y = Input.GetAxis(m_VerticalInput);
            }
            else
            {
                //Keyboard
                //X
                if (Input.GetKey(m_HorizontalInputK1) || Input.GetKey(m_HorizontalInputK2))
                {
                    if (Input.GetKey(m_HorizontalInputK1))
                        m_Aim.x = -1.0f;
                    else if (Input.GetKey(m_HorizontalInputK2))
                        m_Aim.x = 1.0f;
                }
                else if (Input.GetKey(m_VerticalInputK1) || Input.GetKey(m_VerticalInputK2))
                    m_Aim.x = 0.0f;

                //Y
                if (Input.GetKey(m_VerticalInputK1) || Input.GetKey(m_VerticalInputK2))
                {
                    if (Input.GetKey(m_VerticalInputK1))
                        m_Aim.y = -1.0f;
                    else if (Input.GetKey(m_VerticalInputK2))
                        m_Aim.y = 1.0f;
                }
                else if (Input.GetKey(m_HorizontalInputK1) || Input.GetKey(m_HorizontalInputK2))
                    m_Aim.y = 0.0f;
            }

            if (m_IsHookCD)
            {
                m_HookTimer -= Time.deltaTime;
                if (m_HookTimer <= 0.0f)
                {
                    m_IsHookCD = false;
                    m_HookTimer = m_HookCooldown;
                }
            }

            //Clamp scale to never be negative
            m_HookLine.transform.localScale = new Vector3(Mathf.Clamp(m_HookLine.transform.localScale.x, 0.0f, 1000.0f), m_HookLine.transform.localScale.y, m_HookLine.transform.localScale.z);
            //Set line between player and hook if active, else set to 0
            if (m_HookClone)
            {
                Vector3 dir = m_HookClone.transform.position - transform.position;
                Vector3 mid = (dir) / 2.0f + transform.position;
                m_HookLine.transform.position = mid;
                m_HookLine.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                Vector3 scale = m_HookLine.transform.localScale;
                scale.y = dir.magnitude;
                m_HookLine.transform.localScale = scale;
            }
            else
            {
                m_HookLine.transform.localPosition = Vector3.zero;
                m_HookLine.transform.localScale = new Vector3(m_HookLine.transform.localScale.x, 0, m_HookLine.transform.localScale.z);
            }

            //Always rotate pointer
            if (m_Rigidbody.velocity.magnitude > 0.1f)
            {
                Vector2 dir = new Vector2(m_Aim.x, m_Aim.y).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
                //Rotate in steps of 45 degrees
                m_PointerRotation.localRotation = Quaternion.Slerp(m_PointerRotation.localRotation, Quaternion.Euler(targetRot.eulerAngles.x, targetRot.eulerAngles.y, 
                    Mathf.Round(targetRot.eulerAngles.z / 45f) * 45f), 100f * Time.deltaTime);
            }

            //Get input
            if (!m_ControllerType.Equals(ControllerType.Keyboard))
                m_IsGrapple = CheckLeftTrigger();
            else
                m_IsGrapple = Input.GetKey(m_GrappleInputK);

            //If input & nothing has been hit
            if (m_IsGrapple && !m_GrappleHit)
            {
                //Set direction only once
                if (!m_HasSetGrappleDir)
                    m_HasSetGrappleDir = true;

                //Set initial grapple-direction
                m_GrappleDir = m_Pointer.position - transform.position;
                m_GrappleDir.z = 0.0f;

                //Spawn hook prefab
                if (m_HookPrefab && m_CanShootHook && !m_IsHookCD)
                {
                    m_HookClone = (GameObject)Instantiate(m_HookPrefab, transform.position, Quaternion.identity);
                    if (m_HookClone.GetComponent<Rigidbody2D>())
                        m_HookClone.GetComponent<Rigidbody2D>().velocity = m_GrappleDir.normalized * m_HookTravelSpeed;

                    if (m_HookClone.GetComponent<ControllerHook>())
                    {
                        m_HookController = m_HookClone.GetComponent<ControllerHook>();
                        m_HookController.SetIgnores(gameObject, m_Head);
                    }

                    m_CanShootHook = false;
                }

                if (m_HookClone && m_HookController)
                {
                    //Currently using circlecast, gives slightly better results than regular raycasting
                    //RaycastHit2D hit = Physics2D.CircleCast(m_HookClone.transform.position, 0.5f, Vector2.right * 0);
                    //RaycastHit2D hit = m_HookController.GetRayHit();
                    //If something is hit
                    //if (hit)
                    //{
                    //    //Check hit isn't self & not a headcollider
                    //    if (hit.collider.gameObject != gameObject && hit.collider.gameObject != m_Head)
                    //    {
                    //        //m_HookClone.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    //        m_GrappleHit = true;

                    //        //If hook hit player, set hooks parent to player
                    //        if (hit.collider.gameObject.tag == m_Tag || hit.collider.gameObject.tag == m_Head.gameObject.tag)
                    //        {
                    //            m_HookClone.GetComponent<Rigidbody2D>().simulated = false;
                    //            m_HookClone.transform.SetParent(hit.collider.gameObject.transform);
                    //            //Debug.Log("parent set");
                    //        }
                    //    }
                    //}

                    bool hit = m_HookController.GetHit();
                    if (hit)
                        m_GrappleHit = true;
                }
            }
            //Interrupt the hook if button is released and hook hasn't hit anything
            else if (!m_GrappleHit && m_HasSetGrappleDir)
                InterruptGrappleNoP();
            //If something was hit
            else if (m_GrappleHit)
            {
                //Travel to hook
                if (Vector3.Distance(transform.position, m_HookClone.transform.position) > m_HookBreakDistance && !m_HasReachedHook)
                {
                    m_Rigidbody.gravityScale = 0.0f;
                    m_Rigidbody.velocity = (m_HookClone.transform.position - transform.position).normalized * m_TravelToHookSpeed;

                    //Interrupt if button is released
                    if (!m_IsGrapple)
                        InterruptGrapple();

                    //Mid-hook jump
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
                //Grapple-point is reached
                else
                {
                    //Stay on grapple point
                    if (!m_HasReachedHook)
                        m_Rigidbody.velocity = Vector2.zero;
                    m_HasReachedHook = true;

                    //Release if too far away from hit point or released grapple-button
                    if (Vector3.Distance(transform.position, m_HookClone.transform.position) > m_HookBreakDistance)
                        InterruptGrapple();
                    else if (!m_IsGrapple)
                        InterruptGrapple();

                    //Release if jump
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
            }
            //Base-state, button isn't used etc etc
            else
                m_HasSetGrappleDir = false;
            //What happens when grapple is on cd
            //else
            //{
            //    m_HasSetGrappleDir = false;
            //    m_GrappleHit = false;

            //    if (m_HookTimer >= 0.0f)
            //        m_HookTimer -= Time.deltaTime;
            //    else
            //    {
            //        m_IsGrappleCD = false;
            //        m_HookTimer = m_HookCooldown;
            //    }
            //}
        }
    }

    void AttackUpdate()
    {
        //Parry timer
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
            //Get attack input
            if (!m_ControllerType.Equals(ControllerType.Keyboard))
                m_IsAttacking = CheckRightTrigger();
            else
                m_IsAttacking = Input.GetKey(m_AttackInputK);

            //Set attack direction
            if (m_Aim.x > 0)
                m_AttackDirection = 1.0f;
            else if (m_Aim.x < 0)
                m_AttackDirection = -1.0f;

            //Attack rotation
            if (m_IsAttacking)
            {
                m_AttackDegree = m_AttackStartDegree;
                m_AttackRotation.FindChild("StartPoint").localPosition = new Vector3(m_AttackOffset * m_AttackDirection, 0f, 0f);
                m_AttackRotation.localEulerAngles = new Vector3(0, 0, m_AttackDegree * m_AttackDirection);
                m_AttackBox.gameObject.SetActive(true);
                m_AttackBox.transform.localPosition = new Vector3(m_AttackBox.transform.localScale.x / 2.0f * m_AttackDirection, 0f, 0f);
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
            }
            else
                m_AttackBox.gameObject.SetActive(false);
        }
        if (m_AttackBox.gameObject.activeSelf)
        {
            if (!m_ParryBox.gameObject.activeSelf)
            {
                m_ParryBox.gameObject.SetActive(true);
                m_ParryBox.transform.localPosition = new Vector3(m_AttackDirection, 0, 0);
            }
        }
        else
        {
            if (m_ParryBox.gameObject.activeSelf)
                m_ParryBox.gameObject.SetActive(false);
        }
    }

    bool GroundCheck()
    {
        Color col = Color.green;
        bool onGround = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(m_Collider.bounds.size.x / 4, m_Collider.bounds.size.y / 2 * 1.2f, 0), Vector2.right, 0.5f, m_GroundMask);
        if (hit)
        {
            onGround = true;

            if (m_Rigidbody.velocity.y <= 0.0f)
                m_IsJump = false;

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
            onGround = false;

        Debug.DrawRay(transform.position - new Vector3(m_Collider.bounds.size.x / 4.0f, m_Collider.bounds.size.y / 2 * 1.2f, 0), Vector2.right * 0.5f, col);

        return onGround;
    }

    bool WallCheck()
    {
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
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, transform.position.y + (-m_Collider.bounds.size.y / 2.0f) * 0.8f), Vector2.up, m_Collider.bounds.size.y * 0.8f, m_GroundMask);
            if (hit)
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

                //if (m_GrappleHit)
                //{
                //    if (i == 1 && m_GrappleDir.x < 0)
                //        InterruptGrapple();
                //    else if (i == 0 && m_GrappleDir.x > 0)
                //        InterruptGrapple();
                //}

                col = Color.red;
                wallhit = true;
            }
            Debug.DrawRay(new Vector3(x, transform.position.y + (-m_Collider.bounds.size.y / 2.0f) * 0.8f, 0.0f), Vector3.up * m_Collider.bounds.size.y * 0.8f, col);
        }

        return wallhit;
    }

    /// <summary>
    /// Regular grapple interrupt, this sets grapple on cooldown and multiplies the players velocity
    /// </summary>
    void InterruptGrapple()
    {
        InterruptGrappleNoP();
        m_Rigidbody.velocity *= 0.4f;
    }

    /// <summary>
    /// No-physics interrupt, sets grapple on cooldown but does not change the players velocity
    /// </summary>
    void InterruptGrappleNoP()
    {
        m_HasReachedHook = false;
        m_HasSetGrappleDir = false;
        m_GrappleHit = false;
        m_IsHookCD = true;
        m_Rigidbody.gravityScale = 1.0f;
        if (m_HookClone)
            Destroy(m_HookClone);
        m_CanShootHook = true;
    }

    public void ResetValues()
    {
        m_Rigidbody.velocity = Vector2.zero;
        //Grappling hook vars
        m_IsGrapple = false;
        m_GrappleHit = false;
        m_IsHookCD = false;
        m_HasSetGrappleDir = false;
        m_GrappleDir = Vector3.zero;
        m_HasReachedHook = false;
        m_HookTimer = m_HookCooldown;
        m_HookLine.transform.localPosition = Vector3.zero;
        m_HookLine.transform.localScale = new Vector3(m_HookLine.transform.localScale.x, 0, m_HookLine.transform.localScale.z);
        if (m_HookClone)
            Destroy(m_HookClone);
        m_CanShootHook = true;
        m_Aim = Vector2.zero;
        m_FallGraceTimer = 0.0f;
        m_IsJump = false;
        m_IsOnGround = false;
        m_CanAirJump = false;

        //Attack vars
        m_CanAttack = true;
        m_IsAttacking = false;
        m_AttackTimer = 0.0f;
        m_AttackCDTimer = 0.0f;
        m_AttackDegree = 0.0f;
        m_AttackDirection = 1.0f;
        m_AttackOffset = 0.0f;
        m_AttackBox.gameObject.SetActive(false);
    }

    public void Kill()
    {
        if (m_HookClone)
            Destroy(m_HookClone);
        ControllerScene.ReducePlayerCount();
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (m_HookClone)
        {
            if (m_GrappleHit && Vector3.Distance(transform.position, m_HookClone.transform.position) > m_HookBreakDistance)
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
        {
            ControllerScene.SetScoreBark("Player" + (m_PlayerNum + 1), m_Renderer.material.color);
            ControllerScene.ToggleUpdateText();
        }

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

    public void SetParry(bool state)
    {
        m_IsParry = state;
        //Reset attack
        if (m_IsParry)
        {
            m_AttackTimer = 0.0f;
            m_AttackBox.gameObject.SetActive(false);
            m_CanAttack = false;
        }
    }

    public Rigidbody2D GetRigidbody()
    {
        return m_Rigidbody;
    }

    bool CheckLeftTrigger()
    {
        bool state = false;
        switch (m_ControllerType)
        {
            case ControllerType.Xbox:
                //state = Mathf.Round(Input.GetAxis(m_GrappleInput)) == 1.0f;
                m_GamePadState = GamePad.GetState(m_ControllerIndex);
                state = Mathf.Round(m_GamePadState.Triggers.Left) == 1.0f;
                break;
            case ControllerType.XboxOne:
                state = Mathf.Round(Input.GetAxis(m_GrappleInput)) == 1.0f; 
                break;
            case ControllerType.PS:
                state = Input.GetAxis(m_GrappleInput) != -1.0f;
                break;
        }
        return state;
    }

    bool CheckRightTrigger()
    {
        bool state = false;
        switch (m_ControllerType)
        {
            case ControllerType.Xbox:
                //state = Mathf.Round(Input.GetAxis(m_AttackInput)) == 1.0f;
                m_GamePadState = GamePad.GetState(m_ControllerIndex);
                state = Mathf.Round(m_GamePadState.Triggers.Right) == 1.0f;
                break;
            case ControllerType.XboxOne:
                state = Mathf.Round(Input.GetAxis(m_AttackInput)) == 1.0f;
                break;
            case ControllerType.PS:
                state = Input.GetAxis(m_AttackInput) != -1.0f;
                break;
        }
        return state;
    }

    public string GetDebugInformation()
    {
        string Controllers = m_ControllerType.ToString() + " " + m_ControllerNum.ToString() + " " + m_AttackInput + " " + m_GrappleInput + " " + CheckLeftTrigger() + " " + CheckRightTrigger();

        return Controllers;
    }

    public void SetPlayerNum(int num)
    {
        m_PlayerNum = num;
    }
}
