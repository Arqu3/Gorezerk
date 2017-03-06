using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioPlayerMovement))]
public class SFXManager : MonoBehaviour {
    int material;
    int state;
    private AudioPlayerMovement audioPlayerMovement;

    void Start()
    {

    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioPlayerMovement = GetComponent<AudioPlayerMovement>();
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
}
