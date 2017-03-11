using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ControllerScene))]
public class ControllerModifiers : MonoBehaviour
{
    [Header("Round modifier vars")]
    [Range(1, 5)]
    public int m_RoundToChange = 2;
    [Range(0, 5)]
    public int m_ModAmount = 2;
    [Range(0.1f, 10.0f)]
    public float m_BarkTime = 2.0f;

    [Header("Modifier prefabs")]
    public List<GameObject> m_Modifiers = new List<GameObject>();

    //Component vars
    private Text m_BarkText;

    //Index vars
    private List<int> m_Available = new List<int>();
    private List<Modifier> m_ActiveMods = new List<Modifier>();
    private List<int> m_ActiveIndex = new List<int>();

    //Bark vars
    private string m_BarkString = "";

    void Awake()
    {
        if (m_Modifiers.Count == 0)
        {
            Debug.Log("Controller modifier does not have any modifiers!");
            enabled = false;
            return;
        }
        else
        {
            for (int i = 0; i < m_Modifiers.Count; i++)
            {
                Modifier mod = m_Modifiers[i].GetComponent<Modifier>();
                if (!mod)
                {
                    Debug.LogError("Modifier assigned on controller modifier at index: " + i + " does not have a modifier script!");
                    enabled = false;
                    return;
                }
                m_Available.Add(i);
            }
        }

        m_BarkText = GameObject.Find("ModifierBarkText").GetComponent<Text>();
        if (m_BarkText)
            m_BarkText.gameObject.SetActive(false);
    }

    private IEnumerator StartCountdown()
    {
        if (m_BarkText)
        {
            m_BarkText.gameObject.SetActive(true);
            m_BarkText.text = m_BarkString;
        }
        yield return new WaitForSeconds(m_BarkTime);
        if (m_BarkText)
            m_BarkText.gameObject.SetActive(false);
    }

    public void ChangeMods()
    {
        var colortimers = FindObjectsOfType<ChangeColorTimer>();
        Color col = new Color(Random.value, Random.value, Random.value);
        for (int i = 0; i < colortimers.Length; i++)
        {
            colortimers[i].ChangeColor(col);
        }
        m_BarkString = "";


        for (int i = 0; i < m_ModAmount; i++)
        {

            List<int> temp = new List<int>();
            for (int j = 0; j < m_Available.Count; j++)
            {
                temp.Add(m_Available[j]);
            }


            for (int k = 0; k < m_ActiveMods.Count; k++)
            {
                List<int> list = m_ActiveMods[k].GetFilteredMods();

                for (int j = 0; j < list.Count; j++)
                {
                    temp.Remove(list[j]);
                }
            }

            //Find avaliable mods
            //for (int l = 0; l < m_Modifiers.Count; l++)
            //{
            //    if (m_ActiveIndex.Count > 0)
            //    {
            //        for (int j = 0; j < m_ActiveIndex.Count; j++)
            //        {
            //            if (l != m_ActiveIndex[j])
            //            {
            //                Modifier mod = m_Modifiers[l].GetComponent<Modifier>();
            //                if (mod)
            //                {
            //                    List<int> filter = mod.GetFilteredMods();
            //                    if (filter.Count > 0)
            //                    {
            //                        for (int k = 0; k < filter.Count; k++)
            //                        {
            //                            if (l != mod.GetFilteredMods()[k])
            //                                temp.Add(l);
            //                        }
            //                    }
            //                    else
            //                        temp.Add(l);
            //                }
            //            }
            //        }
            //    }
            //    else
            //        temp.Add(l);
            //}

            if (m_Available.Count > 0)
            {
                int random = Random.Range(0, temp.Count);
                GameObject instance = Instantiate(m_Modifiers[temp[random]], Vector3.zero, Quaternion.identity);
                Modifier mod = instance.GetComponent<Modifier>();
                instance.transform.SetParent(transform);
                m_ActiveMods.Add(mod);
                m_ActiveIndex.Add(mod.GetID());
                m_Available.Remove(mod.GetID());

                m_BarkString += mod.GetName();
                if (i != m_ModAmount - 1)
                    m_BarkString += "       ";
            }
        }

        StartCoroutine(StartCountdown());

        if (m_ActiveMods.Count > m_ModAmount)
        {
            for (int i = 0; i < m_ActiveMods.Count - m_ModAmount; i++)
            {
                Destroy(m_ActiveMods[i].gameObject);

                m_Available.Add(m_ActiveIndex[i]);
            }
            m_ActiveMods.RemoveRange(0, m_ModAmount);
            m_ActiveIndex.RemoveRange(0, m_ModAmount);
        }
    }

    public List<Modifier> GetActiveMods()
    {
        return m_ActiveMods;
    }

    public void AddMod(Modifier mod)
    {
        m_ActiveMods.Add(mod);
    }
}
