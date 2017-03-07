using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerJumpToggle : Modifier
{
    public string m_ModName = "";

    protected override void Start()
    {
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
        Toolbox.Instance.m_CanMove = true;
    }

    public override string GetName()
    {
        return m_ModName;
    }
}
