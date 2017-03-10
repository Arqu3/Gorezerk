using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStopMoving : Modifier
{
    public string m_ModName = "";
    [Header("Standing still timer")]
    [Range(0.0f, 10.0f)]
    public float m_StillTime = 1.0f;
    public int m_ID = 0;

    //Timer vars
    private List<float> m_Timers = new List<float>();

    //Player vars
    private List<ControllerPlayer> m_Players = new List<ControllerPlayer>();

    protected override void Start()
    {
        ControllerScene scene = GetComponentInParent<ControllerScene>();
        if (scene)
        {
            for (int i = 0; i < scene.GetPlayers().Count; i++)
            {
                m_Players.Add(scene.GetPlayers()[i]);
                m_Timers.Add(0.0f);
            }
        }
    }

    void Update()
    {
        if (!ControllerScene.GetRoundStart())
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i].gameObject.activeSelf)
                {
                    if (Mathf.Round(m_Players[i].GetRigidbody().velocity.magnitude) == 0)
                    {
                        m_Timers[i] += Time.deltaTime;
                        if (m_Timers[i] >= m_StillTime)
                            m_Players[i].Kill();
                    }
                    else
                        m_Timers[i] = 0.0f;
                }
            }
        }
    }

    public override void OnRoundEnd()
    {
        for (int i = 0; i < m_Timers.Count; i++)
        {
            m_Timers[i] = 0.0f;
        }
    }

    public override void OnRoundStart()
    {
    }

    protected override void OnDestroy()
    {
    }

    public override string GetName()
    {
        return m_ModName;
    }

    public override int GetID()
    {
        return m_ID;
    }
}
