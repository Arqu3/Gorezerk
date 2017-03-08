using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioModifiers : MonoBehaviour {

    [FMODUnity.EventRef]
    public string BeesEv;
    FMOD.Studio.EventInstance Bees;

    [FMODUnity.BankRef]
    public string BeesHitEv;
    FMOD.Studio.EventInstance BeesHit;

    [FMODUnity.EventRef]
    public string BodySnatchersEv;
    FMOD.Studio.EventInstance BodySnatchers;

    [FMODUnity.EventRef]
    public string MissileEv;
    FMOD.Studio.EventInstance Missile;

    [FMODUnity.EventRef]
    public string SanicEv;
    FMOD.Studio.EventInstance Sanic;

    [FMODUnity.EventRef]
    public string RedButtonEv;
    FMOD.Studio.EventInstance RedButton;

    [FMODUnity.EventRef]
    public string TrapsEv;
    FMOD.Studio.EventInstance Traps;

    [FMODUnity.EventRef]
    public string NoFeetEv;
    FMOD.Studio.EventInstance NoFeet;

    void Update()
    {

    }

    public void ModBeesAmb()
    {
        Bees.start();
    }

    public void ModBeesHit()
    {
        FMODUnity.RuntimeManager.PlayOneShot(BodySnatchersEv);
    }

    public void ModBodySnatchers()
    {
        FMODUnity.RuntimeManager.PlayOneShot(BodySnatchersEv);
    }

    public void ModMissile()
    {

    }

    public void ModSanic()
    {

    }

    public void ModRedButton()
    {

    }

    public void ModTraps()
    {

    }

    public void ModNoFeet()
    {

    }

    public void ModNeverStop()
    {

    }

}
