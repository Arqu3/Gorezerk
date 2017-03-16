using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioPlayerMovement))]
public class SFXManager : MonoBehaviour {
    int material;
    int state;
    Vector3 playerPosition;
    private AudioPlayerMovement audioPlayerMovement;
    private AudioModifiers audioModifiers;
    private AudioStingers audioStingers;

    [FMODUnity.EventRef]
    public string sfxVolumeEv;
    FMOD.Studio.EventInstance sfxVolume;
    FMOD.Studio.ParameterInstance VolumeSettings;

    [FMODUnity.EventRef]
    public string sfxSliderEv;
    FMOD.Studio.EventInstance sfxSlider;
    FMOD.Studio.ParameterInstance SliderPosition;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioPlayerMovement = GetComponent<AudioPlayerMovement>();
        audioModifiers = GetComponent<AudioModifiers>();
        audioStingers = GetComponent<AudioStingers>();

        sfxVolume = FMODUnity.RuntimeManager.CreateInstance(sfxVolumeEv);
        sfxVolume.getParameter("SFXVolume", out VolumeSettings);

        sfxSlider = FMODUnity.RuntimeManager.CreateInstance(sfxSliderEv);
        sfxSlider.getParameter("Volume", out SliderPosition);
    }
    //::MASTER SETTINGS::

    void Start()
    {
        sfxVolume.start();
    }
    public void Volume(float volume)
    {
        VolumeSettings.setValue(volume);
        SliderPosition.setValue(volume);
        sfxSlider.start();
    }

    //::PLAYER::

    //Call these functions to play SFX
    public void PlayerPosition(Vector3 position)
    {
        audioPlayerMovement.playerPosition = position;
        audioModifiers.playerPosition = position;
    }

    public void PlayerRun(int material)
    {
        audioPlayerMovement.PlayerRun(material);
    }

    public void PlayerStop()
    {
        audioPlayerMovement.PlayerStop();
    }

    public void PlayerJump()
    {
        audioPlayerMovement.PlayerJump();
    }

    public void PlayerJumpLand(int material)
    {
        audioPlayerMovement.PlayerJumpLand(material);
    }

    public void PlayerSwing()
    {
        audioPlayerMovement.PlayerSwing();
    }

    public void PlayerParry()
    {
        audioPlayerMovement.PlayerParry();
    }

    public void PlayerDeath()
    {
        audioPlayerMovement.PlayerDeath();
    }

    public void GrappleFire( )
    {

        audioPlayerMovement.GrappleFire();
    }

    public void GrappleImpact( )
    {
        audioPlayerMovement.GrappleImpact();
    }

    public void GrappleDrag( )
    {
        audioPlayerMovement.GrappleDrag();
    }

    //::MODIFIERS::

    public void BodySnatchers()
    {
        audioModifiers.ModBodySnatchers();
    }

    public void ModBeesAmb()
    {
        audioModifiers.ModBeesAmb();
    }

    public void ModBeesHit()
    {
        audioModifiers.ModBeesHit();
    }

    //::STINGERS;;

    public void MatchStart()
    {
        audioStingers.MatchStart();
    }

    public void MatchWin()
    {
        audioStingers.MatchWin();
    }

    public void RoundWin()
    {
        audioStingers.RoundWin();
    }

    //::MENU SOUNDS::


}
