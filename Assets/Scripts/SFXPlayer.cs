using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip clip_ok;
    public AudioClip clip_success;
    public AudioClip clip_warning;
    public AudioClip clip_error;
    public AudioClip clip_in_button;
    public AudioClip clip_click_button;
    public AudioClip clip_completed;
    public AudioSource Audio { get; private set; }
    public static SFXPlayer SFX { get; private set; }

    public const string OK = "ok";
    public const string SUCCESS = "success";
    public const string WARNING = "warning";
    public const string ERROR = "error";
    public const string IN_BUTTON = "in button";
    public const string CLICK_BUTTON = "click button";
    public const string COMPLETED_OBJECTIVE = "completed";

    public static Dictionary<string, AudioClip> clips;
    private void Awake()
    {
        // sources.Clear();
        SFX = this;
        clips = new Dictionary<string, AudioClip>()
        {
            {OK, clip_ok },
            {SUCCESS, clip_success },
            {WARNING, clip_warning },
            {ERROR, clip_error },
            {IN_BUTTON, clip_in_button },
            {CLICK_BUTTON, clip_click_button },
            {COMPLETED_OBJECTIVE, clip_completed },
        };
    }
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        Audio.volume = PlayerPrefs.GetFloat(SoundSettings.key_sfx, 1.0f);
        // sources.Add(sound_effect, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySFX(string clip_name)
    {
        AudioClip clip = clips[clip_name];
        SFX.Audio.PlayOneShot(clip);
    }
    public static void VolumeUpdate(float value)
    {
        SFX.Audio.volume = value;
    }
}
