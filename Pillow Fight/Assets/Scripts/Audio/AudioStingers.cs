using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStingers : MonoBehaviour {

    [FMODUnity.EventRef]
    public string MatchStingerEv;
    FMOD.Studio.EventInstance MatchStinger;
    FMOD.Studio.ParameterInstance State;

    void Start()
    {
        MatchStinger = FMODUnity.RuntimeManager.CreateInstance(MatchStingerEv);
        MatchStinger.getParameter("State", out State);
    }

    public void MatchStart()
    {
        State.setValue(0);
        MatchStinger.start();
    }

    public void RoundWin()
    {
        State.setValue(1);
        MatchStinger.start();
    }

    public void MatchWin()
    {
        State.setValue(2);
        MatchStinger.start();
    }

}
