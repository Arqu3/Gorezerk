using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControllerAudio : MonoBehaviour
{
    //Public vars
    public AudioClip[] m_AudioClips;
    public bool m_Shuffle = false;
    public bool m_Mute = false;

    //Index vars
    private int m_CurrentIndex = 0;
    private float m_CurrentTime = 0.0f;

    //Component vars
    AudioSource m_Source;

	void Start()
    {
		if (m_AudioClips.Length < 1)
        {
            Debug.Log("Controller audio is missing clips!");
            enabled = false;
            return;
        }

        m_Source = GetComponent<AudioSource>();
        m_Source.volume = Toolbox.Instance.m_MusicVolume;

        PlayCurrentClip();
	}
	
	void Update()
    {
        if (!m_Mute)
        {
            if (m_Source.time >= m_CurrentTime * 0.99f)
                PlayNextClip();
        }
	}

    void PlayCurrentClip()
    {
        m_Source.Stop();

        m_Source.clip = m_AudioClips[m_CurrentIndex];

        m_CurrentTime = m_AudioClips[m_CurrentIndex].length;

        m_Source.Play();
    }

    void PlayNextClip()
    {
        if (m_CurrentIndex < m_AudioClips.Length - 1)
            m_CurrentIndex++;
        else
            m_CurrentIndex = 0;

        PlayCurrentClip();
    }

    public void ChangeVolume(float volume)
    {
        Toolbox.Instance.m_MusicVolume = volume;
        m_Source.volume = volume;
    }

    public void SetMute(bool state)
    {
        m_Mute = state;

        if (m_Mute)
        {
            m_Source.Pause();
            m_Source.volume = 0.0f;
        }
        else
        {
            m_Source.UnPause();
            m_Source.volume = Toolbox.Instance.m_MusicVolume;
        }
    }
}
