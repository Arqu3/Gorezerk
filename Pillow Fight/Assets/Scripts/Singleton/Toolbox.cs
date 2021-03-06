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
    public List<GameObject> m_Characters = new List<GameObject>();
    public float m_MusicVolume = 0.5f;

    //Modification variables
    public float m_MovementSpeed = 1.0f;
    public bool m_CanMove = true;

    //Sound components
    public SFXManager m_SfxManager;
    public MusicManager m_MusicManager;

    //Safety variables
    public bool m_IsApplicationQuit = false;

    //Used whenever menu is loaded *BE CAREFUL WHEN TO CALL THIS*
    public void ClearInformation()
    {
        m_MovementSpeed = 1.0f;
        m_CanMove = true;

        m_Characters.Clear();
        m_Information.Clear();
        m_Colors.Clear();
        m_PlayerCount = 0;
        if (m_SfxManager)
            m_SfxManager.PlayerStop();
    }

    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }

    public void InitSound()
    {
        m_MusicManager = FindObjectOfType<MusicManager>();
        m_SfxManager = FindObjectOfType<SFXManager>();
    }

    void OnApplicationQuit()
    {
        m_IsApplicationQuit = true;
        Debug.Log("Application about to quit, setting toolbox variables");
    }
}
