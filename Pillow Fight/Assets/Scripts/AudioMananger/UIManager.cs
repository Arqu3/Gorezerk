using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private AudioPauseMenu audioPauseMenu;
    private AudioMainMenu audioMainMenu;

    [FMODUnity.EventRef]
    public string UIVolumeEv;
    FMOD.Studio.EventInstance UIVolume;
    FMOD.Studio.ParameterInstance Setting;

    void Awake()
    {
        audioPauseMenu = FindObjectOfType<AudioPauseMenu>();
        audioMainMenu =  FindObjectOfType<AudioMainMenu>();

        UIVolume = FMODUnity.RuntimeManager.CreateInstance(UIVolumeEv);
        UIVolume.getParameter("Volume", out Setting);

    }

    void Start()
    {
        UIVolume.start();
    }
    public void Volume(float volume)
    {
        Setting.setValue(volume);
    }

    //::MAIN MENU::
    public void MenuHover()
    {
        audioMainMenu.Hover();
    }

    public void MenuClick()
    {
        audioMainMenu.Click();
    }

    public void StartClick()
    {
        audioMainMenu.Start();
    }

    public void Countdown()
    {
        audioMainMenu.Countdown();
    }

    //::PAUSE MENU::
    public void PauseHover()
    {
        audioPauseMenu.Hover();
    }
    
    public void PauseClick()
    {
        audioPauseMenu.Click();
    }

}
