using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class MusicManager : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter musicManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        musicManager = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    //Call these functions to change music playback
    public void MenuMusic ()
    {
        musicManager.SetParameter("Intensity",0f);        
    }

    public void MatchMusic()
    {
        musicManager.SetParameter("Intensity", 1f);
    }

    public void PauseMusic()
    {
        musicManager.SetParameter("GamePause", 1f);
    }

}