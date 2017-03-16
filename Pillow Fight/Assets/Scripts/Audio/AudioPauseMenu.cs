using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPauseMenu : MonoBehaviour {

    [FMODUnity.EventRef]
    public string fmodButtonEv;
    FMOD.Studio.EventInstance fmodButton;
    FMOD.Studio.ParameterInstance State;

    void Awake()
    {
        //fmodClick= FMODUnity.RuntimeManager.CreateInstance(fmodClickEv);
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

}
