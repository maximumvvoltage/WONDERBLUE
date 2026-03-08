using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    
    public static AudioSource audioSource;
    private static AllSounds allSounds;
    [SerializeField] private Slider soundSlider;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            allSounds = GetComponent<AllSounds>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void Play(string soundName)
    { 
        AudioClip audioClip = allSounds.GetRandomClip(soundName);
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {
        soundSlider.onValueChanged.AddListener(delegate { OnValueChange(); } );
    } 
    
    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void OnValueChange()
    {
        SetVolume(soundSlider.value);
    }
}