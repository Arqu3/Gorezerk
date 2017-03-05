using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerScene))]
public class ControllerModifiers : MonoBehaviour
{
    [Header("Round modifier vars")]
    [Range(1, 5)]
    public int m_ChangeModNum = 2;

    [Header("Modifier prefabs")]
    public List<GameObject> m_Modifiers = new List<GameObject>();

    //Component vars
    private ControllerScene m_Scene;

    //Index vars
    private List<int> m_Available = new List<int>();

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

        m_Scene = GetComponent<ControllerScene>();
    }

    public void ChangeMods(int currentRound)
    {
        if (currentRound >= m_ChangeModNum && currentRound % m_ChangeModNum == 0)
        {

        }
    }
}
