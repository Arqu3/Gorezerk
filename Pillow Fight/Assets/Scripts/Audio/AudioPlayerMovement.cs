using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerMovement : MonoBehaviour
{
    public int material;
    private SFXManager sfxMananger;

    [FMODUnity.EventRef]
    public string FootstepsEv;
    FMOD.Studio.EventInstance Footsteps;
    FMOD.Studio.ParameterInstance FMaterial;

    [FMODUnity.EventRef]
    public string JumpEv;
    FMOD.Studio.EventInstance Jump;

    [FMODUnity.EventRef]
    public string JumpLandEv;
    FMOD.Studio.EventInstance JumpLand;
    FMOD.Studio.ParameterInstance JMaterial;

    void Awake()
    {
        sfxMananger = FindObjectOfType<SFXManager>();
    }

    void Start()
    {
        Footsteps = FMODUnity.RuntimeManager.CreateInstance(FootstepsEv);
        JumpLand.getParameter("Material", out JMaterial);
        Footsteps.getParameter("Material", out FMaterial);
    }

    public void PlayerJumpLand(int material)
    {
        JMaterial.setValue(material);
        JumpLand.start();
    }

    public void PlayerRun(int material)
    {
        sfxMananger.PlayerRun(material);
        FMaterial.setValue(material);
        Footsteps.start();
    }

    public void PlayerStop()
    {
        Footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PlayerJump()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpEv);
    }
}

