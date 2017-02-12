using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHomingMissiles : Modifier
{
    //Public vars
    public GameObject m_MissilePrefab;

    //Round start vars
    private List<GameObject> m_Missiles = new List<GameObject>();

    protected override void Start()
    {
        if (!m_MissilePrefab)
        {
            Debug.Log("Homing missiles is missing its prefab!");
            enabled = false;
            return;
        }

        //TODO - create missile spawnpoints?
    }

    void DestroyMissiles()
    {
        //Destroy all leftover missiles
        for (int i = 0; i < m_Missiles.Count; i++)
        {
            Destroy(m_Missiles[i]);
        }
        m_Missiles.Clear();
    }

    public override void OnRoundStart()
    {
        //Spawn new missiles
        for (int i = 0; i < ControllerScene.GetPlayerCount(); i++)
        {
            GameObject clone = (GameObject)Instantiate(m_MissilePrefab, Vector3.zero, Quaternion.identity);
            if (clone.GetComponent<HomingMissile>())
                clone.GetComponent<HomingMissile>().SetTarget(GetComponentInParent<ControllerScene>().m_Players[i].transform);

            m_Missiles.Add(clone);
        }
    }

    public override void OnRoundEnd()
    {
        DestroyMissiles();
    }

    protected override void OnDestroy()
    {
        DestroyMissiles();
    }
}
