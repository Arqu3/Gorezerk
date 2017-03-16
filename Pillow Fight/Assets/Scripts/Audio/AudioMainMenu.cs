using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMainMenu : MonoBehaviour {

    [FMODUnity.EventRef]
    public string fmodButtonEv;
    FMOD.Studio.EventInstance fmodButton;
    FMOD.Studio.ParameterInstance State;

    [FMODUnity.EventRef]
    public string fmodCountdownEv;
    FMOD.Studio.EventInstance fmodCountdown;

    //public string xEv;
    //FMOD.Studio.EventInstance x;
    //FMOD.Studio.ParameterInstance paramx;

    void Awake()
    {
        //fmodClick = FMODUnity.RuntimeManager.CreateInstance(fmodClickEv);
        fmodButton.getParameter("Parameter", out State);
    }
    public void Hover()
    {
        State.setValue(0);
        FMODUnity.RuntimeManager.PlayOneShot(fmodButtonEv);
    }

    public void Click()
    {
        State.setValue(1);
        FMODUnity.RuntimeManager.PlayOneShot(fmodButtonEv);
    }

    public void Start()
    {
        State.setValue(2);
        FMODUnity.RuntimeManager.PlayOneShot(fmodButtonEv);
    }

    public void Countdown()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodCountdownEv);
    }
}
