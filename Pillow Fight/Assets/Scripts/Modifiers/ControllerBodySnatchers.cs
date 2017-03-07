using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBodySnatchers : Modifier
{
    //Public vars
    public string m_ModName = "";
    public float m_CountdownTime = 5.0f;

    //Timer vars
    private float m_CountdownTimer = 0.0f;

    //Player vars
    private List<ControllerPlayer> m_Players = new List<ControllerPlayer>();

    protected override void Start()
    {
        var players = FindObjectsOfType<ControllerPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            m_Players.Add(players[i]);
        }
        m_CountdownTimer = m_CountdownTime;
    }

    void Update()
    {
        m_CountdownTimer -= Time.deltaTime;
        if (m_CountdownTimer <= 0.0f)
        {
            SwapPlayers();
            m_CountdownTimer = m_CountdownTime;
        }
    }

    void SwapPlayers()
    {
        List<Transform> tempSpawn = new List<Transform>();

        for (int i = 0; i < m_Players.Count; i++)
        {
            if (m_Players[i].gameObject.activeSelf)
                tempSpawn.Add(m_Players[i].transform);
        }

        //for (int i = 0; i < tempSpawn.Count; i++)
        //{
        //    if (tempSpawn.Count <= 1)
        //        break;

        //    int random = Random.Range(0, tempSpawn.Count);
        //    Transform first = tempSpawn[random];
        //    tempSpawn.RemoveAt(random);

        //    random = Random.Range(0, tempSpawn.Count);
        //    Transform second = tempSpawn[random];
        //    tempSpawn.RemoveAt(random);

        //    Vector3 temp = first.position;
        //    first.position = second.position;
        //    second.position = temp;
        //}

        Vector3 initial = tempSpawn[0].position;

        for (int i = 0; i < tempSpawn.Count; i++)
        {
            int next = i + 1;
            if (next >= tempSpawn.Count)
            {
                next = 0;
                tempSpawn[i].position = initial;
                break;
            }

            tempSpawn[i].position = tempSpawn[next].position;
        }
    }

    public override void OnRoundEnd()
    {
        m_CountdownTimer = m_CountdownTime;
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
}
