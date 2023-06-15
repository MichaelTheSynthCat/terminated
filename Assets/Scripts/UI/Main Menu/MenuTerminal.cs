using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuTerminal : MonoBehaviour
{
    public GameObject after_execution;
    private TMP_InputField inputfield;
    void Start()
    {
        inputfield = GetComponent<TMP_InputField>();
        ShowText("");
    }
    void Update()
    {
        if(inputfield.isFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Execute(inputfield.text);
        }
    }
    public void OnEndExecute()
    {
        Execute(inputfield.text);
    }
    private void Execute(string line)
    {
        if(line == "run ransomware")
        {
            float mem_sfx = PlayerPrefs.GetFloat(SoundSettings.key_sfx, 1.0f);
            float mem_music = PlayerPrefs.GetFloat(SoundSettings.key_music, 1.0f);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetFloat(SoundSettings.key_sfx, mem_sfx);
            PlayerPrefs.SetFloat(SoundSettings.key_music, mem_music);
            PlayerPrefs.Save();
            ShowText("All progress has been deleted. Start your life better!\nRestart game please.");
        }
        inputfield.text = "";
    }
    private void ShowText(string text)
    {
        after_execution.GetComponent<TextMeshProUGUI>().text = text;
    }
}
