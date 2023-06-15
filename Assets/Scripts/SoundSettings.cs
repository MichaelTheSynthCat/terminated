using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public GameObject sfx_slider_object;
    public GameObject music_slider_object;
    public GameObject sfx_text;
    public GameObject music_text;

    public const string key_sfx = "SFX volume";
    public const string key_music = "MUSIC volume";

    private float sfx_volume;
    private float music_volume;
    void Start()
    {
        sfx_volume = PlayerPrefs.GetFloat(key_sfx, 1.0f);
        music_volume = PlayerPrefs.GetFloat(key_music, 1.0f);

        SetSliderValue(sfx_slider_object, sfx_volume);
        SetSliderValue(music_slider_object, music_volume);
        ChangeText(sfx_text, sfx_volume);
        ChangeText(music_text, music_volume);
    }
    private void Update()
    {
        if(sfx_volume != GetSliderValue(sfx_slider_object) ||
            music_volume != GetSliderValue(music_slider_object))
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
    public void SaveSettings()
    {
        sfx_volume = GetSliderValue(sfx_slider_object);
        music_volume = GetSliderValue(music_slider_object);
        PlayerPrefs.SetFloat(key_sfx, sfx_volume);
        PlayerPrefs.SetFloat(key_music, music_volume);
        PlayerPrefs.Save();
        SFXPlayer.VolumeUpdate(sfx_volume);
        MusicPlayer.VolumeUpdate(music_volume);
    }
    public void ChangeSFX()
    {
        ChangeText(sfx_text, GetSliderValue(sfx_slider_object));
    }
    public void ChangeMusic()
    {
        ChangeText(music_text, GetSliderValue(music_slider_object));
    }

    private float GetSliderValue(GameObject slider_object)
    {
        Slider slider = slider_object.GetComponent<Slider>();
        return slider.value;
    }
    private void SetSliderValue(GameObject slider_object, float value)
    {
        Slider slider = slider_object.GetComponent<Slider>();
        slider.value = value;
    }
    public static void ChangeText(GameObject given_object, float value) // change text of a GameObject
    {
        if (given_object == null) return;
        string text = (value * 100.0f).ToString("0") + "%";
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
}
