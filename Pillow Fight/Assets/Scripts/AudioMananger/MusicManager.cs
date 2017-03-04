using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter musicManager;

    //Call these functions to change music playback
    public void MenuMusic ()
    {
        musicManager.SetParameter("Intensity", 0f);        
    }

    public void MatchMusic()
    {
        musicManager.SetParameter("Intensity", 1f);
    }

    public void PauseMusic()
    {
        musicManager.SetParameter("Pause", 1f);
    }

}