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

    public List<PlayerInformation> m_Information;
    public List<Color> m_Colors;

    static public T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
