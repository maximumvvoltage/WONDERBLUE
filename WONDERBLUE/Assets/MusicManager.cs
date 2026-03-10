using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;
    public AudioClip musicClip;
    [SerializeField] private Slider musicSlider;
    public bool musicPaused;
    [SerializeField] private Image musicImageIcon;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Sprite musicOn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        if (musicClip != null)
        {
            PlayBGM(false, musicClip);
        }
        musicSlider.onValueChanged.AddListener(delegate {SetVolume(musicSlider.value);} );
        musicImageIcon.GetComponent<Image>();
        musicImageIcon.sprite = musicOn;

        musicPaused = false;
    }

    public static void SetVolume(float volume)
    {
        instance.audioSource.volume = volume;
    }

    public void PlayBGM(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop();
            }
            audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        musicPaused = !musicPaused;
        
        if (musicPaused)
        {
            musicImageIcon.sprite = musicOff;
            audioSource.Pause();
        }
        else
        {
            musicImageIcon.sprite = musicOn;
            audioSource.UnPause();
        }
    }
}
