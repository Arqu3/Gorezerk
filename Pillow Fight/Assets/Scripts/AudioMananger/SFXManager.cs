using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {
    private SFXManager sfxManager;
    int material;
    private AudioPlayerMovement audioPlayerMovement;


    void Start ()
    {

    }
    void Awake ()
    {
        sfxManager = FindObjectOfType<SFXManager> ();
    }

    //Call these functions to play SFX
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
}
