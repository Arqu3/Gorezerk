using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerJumpToggle : Modifier
{
    protected override void Start()
    {
        Toolbox.Instance.m_CanJump = false;
    }

    public override void OnRoundEnd()
    {
    }

    public override void OnRoundStart()
    {
    }

    protected override void OnDestroy()
    {
        Toolbox.Instance.m_CanJump = true;
    }
}
