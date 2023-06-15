using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> tracks;
    public AudioSource Audio { get; private set; }
    public static MusicPlayer Music { get; private set; }
    private void Awake()
    {
        Music = this;
    }
    void Start()
    {
        Audio = GetComponent<AudioSource>();
        Audio.volume = PlayerPrefs.GetFloat(SoundSettings.key_music, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Audio.isPlaying && tracks.Count > 0)
        {
            Audio.Stop();
            Audio.clip = PickClip();
            Audio.Play();
        }   
    }
    private AudioClip PickClip()
    {
        return tracks[Random.Range(0, tracks.Count)];
    }
    public static void VolumeUpdate(float value)
    {
        Music.Audio.volume = value;
    }
}
