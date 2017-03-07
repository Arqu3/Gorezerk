using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStats : Modifier
{
    //Public vars
    public string m_ModName = "";
    public float m_MoveSpeedMod = 1.0f;

    protected override void Start()
    {
        if (Toolbox.Instance)
            Toolbox.Instance.m_MovementSpeed += m_MoveSpeedMod;
    }

    public override void OnRoundEnd()
    {
    }

    public override void OnRoundStart()
    {   
    }

    protected override void OnDestroy()
    {
        if (Toolbox.Instance)
            Toolbox.Instance.m_MovementSpeed -= m_MoveSpeedMod;
    }

    public override string GetName()
    {
        return m_ModName;
    }
}
