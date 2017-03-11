using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRedButton : Modifier
{
    //Public vars
    public string m_ModName = "";
    public GameObject m_ButtonPrefab;
    public int m_ID = 0;
    public List<GameObject> m_FilteredMods = new List<GameObject>();

    //Button vars
    private GameObject m_ButtonInstance;

    protected override void Start()
    {
        if (!m_ButtonPrefab)
        {
            Debug.Log("Big red button controller is missing its prefab!");
            enabled = false;
            return;
        }
    }

    public override void OnRoundEnd()
    {
        DestroyButton();
    }

    public override void OnRoundStart()
    {
        SpawnButton();
    }

    protected override void OnDestroy()
    {
        DestroyButton();
    }

    void SpawnButton()
    {
        ControllerScene scene = FindObjectOfType<ControllerScene>();
        int random = Random.Range(0, scene.GetSpawnPoints().Count);
        m_ButtonInstance = (GameObject)Instantiate(m_ButtonPrefab, scene.GetSpawnPoints()[random].position, Quaternion.identity);
    }

    void DestroyButton()
    {
        Destroy(m_ButtonInstance);
    }

    public override string GetName()
    {
        return m_ModName;
    }

    public override int GetID()
    {
        return m_ID;
    }

    public override List<int> GetFilteredMods()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < m_FilteredMods.Count; i++)
        {
            Modifier mod = m_FilteredMods[i].GetComponent<Modifier>();
            if (mod)
                list.Add(mod.GetID());
        }
        return list;
    }
}
