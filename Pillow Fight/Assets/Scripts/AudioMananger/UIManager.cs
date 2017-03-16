using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private AudioPauseMenu audioPauseMenu;
    private AudioMainMenu audioMainMenu;

    void Awake()
    {
        audioPauseMenu = FindObjectOfType<AudioPauseMenu>();
        audioMainMenu =  FindObjectOfType<AudioMainMenu>();

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

    public void Start()
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
