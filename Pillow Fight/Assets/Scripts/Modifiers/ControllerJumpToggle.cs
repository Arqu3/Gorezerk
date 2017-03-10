using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerJumpToggle : Modifier
{
    public string m_ModName = "";
    public int m_ID = 0;

    protected override void Start()
    {
        if (Toolbox.Instance)
            Toolbox.Instance.m_CanMove = false;
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
            Toolbox.Instance.m_CanMove = true;
    }

    public override string GetName()
    {
        return m_ModName;
    }

    public override int GetID()
    {
        return m_ID;
    }
}
