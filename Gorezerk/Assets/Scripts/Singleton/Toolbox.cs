﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Toolbox singleton-class, use this to set and get any public variables needed,
/// note that no more than 1 instance of this class should be present at any time!
/// </summary>
public class Toolbox : Singleton<Toolbox>
{
    //Make sure constructor cannot be used, true singleton
    protected Toolbox(){}

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public int m_PlayerCount = 0;
    public List<PlayerInformation> m_Information = new List<PlayerInformation>();
    public List<Color> m_Colors = new List<Color>();

    //Used whenever menu is loaded *BE CAREFUL WHEN TO CALL THIS*
    public void ClearInformation()
    {
        m_Information.Clear();
        m_Colors.Clear();
        m_PlayerCount = 0;
    }

    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
