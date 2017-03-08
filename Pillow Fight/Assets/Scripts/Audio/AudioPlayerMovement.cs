using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerMovement : MonoBehaviour
{
    public int material;
    public Vector3 playerPosition;
    
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

    [FMODUnity.EventRef]
    public string WeaponSwingEv;
    FMOD.Studio.EventInstance WeaponSwing;

    [FMODUnity.EventRef]
    public string DeathEv;
    FMOD.Studio.EventInstance Death;

    [FMODUnity.EventRef]
    public string GrappleEv;
    FMOD.Studio.EventInstance Grapple;
    FMOD.Studio.ParameterInstance GrappleState;

    [FMODUnity.EventRef]
    public string ParryEv;
    FMOD.Studio.EventInstance Parry;


    void Start()
    {
        Footsteps = FMODUnity.RuntimeManager.CreateInstance(FootstepsEv);
        JumpLand = FMODUnity.RuntimeManager.CreateInstance(JumpLandEv);
        Grapple = FMODUnity.RuntimeManager.CreateInstance(GrappleEv);

        JumpLand.getParameter("Material", out JMaterial);
        Footsteps.getParameter("Material", out FMaterial);
        Grapple.getParameter("State", out GrappleState);

    }

    public void PlayerPos(Vector3 position)
    {
        playerPosition = position;
        Footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(playerPosition));

    }

    public void PlayerJumpLand(int material)
    {
        JMaterial.setValue(material);
        JumpLand.start();
    }

    public void PlayerRun(int material)
    {
        FMaterial.setValue(material);
        Footsteps.start();

        //FMODUnity.RuntimeManager.PlayOneShot(FootstepsEv, position);
    }

    public void PlayerStop()
    {
        Footsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PlayerJump()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpEv);
    }

    public void PlayerSwing()
    {
        FMODUnity.RuntimeManager.PlayOneShot(WeaponSwingEv);

    }

    public void PlayerParry()
    {
        FMODUnity.RuntimeManager.PlayOneShot(ParryEv);
    }
    

    public void PlayerDeath()
    {
        FMODUnity.RuntimeManager.PlayOneShot(DeathEv);

    }

    //Grapple functions
    public void GrappleFire( )
    {
        GrappleState.setValue(0);
        Grapple.start();
    }

    public void GrappleImpact( )
    {
        GrappleState.setValue(1);
    }

    public void GrappleDrag( )
    {
        GrappleState.setValue(2);
    }

    public void GrappleEnd()
    {
        Grapple.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}

