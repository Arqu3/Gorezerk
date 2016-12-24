using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toolbox : Singleton<Toolbox>
{
    //Make sure constructor cannot be used
    protected Toolbox(){}

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public List<PlayerInformation> m_Information = new List<PlayerInformation>();
    public List<Color> m_Colors = new List<Color>();

    public void ClearInformation()
    {
        m_Information.Clear();
        m_Colors.Clear();
    }

    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
