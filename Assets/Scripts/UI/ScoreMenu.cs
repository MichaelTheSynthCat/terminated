using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreMenu : MonoBehaviour
{
    public string campaign_name = "undefined"; // name of campaign in which the level belongs
    public string level_code = "undefined"; // code of the level
    public string continue_scene = "Main Menu"; // 

    public static ScoreMenu menu;
    public GameObject result_text; // shows Victory or Defeat
    public GameObject detail_text; // detail of win/lose
    public GameObject victory_text; // text to show in case of victory
    public GameObject defeat_text; // text to show in case of defeat
    public GameObject time_completed; // text showing time played
    public GameObject record_time; // text showing record time in which was the level beaten

    public const string level_key = "LEVEL";
    private void Awake()
    {
        menu = this;
    }
    void Start()
    {
        
    }
    public static void OpenScoreMenuStatic(bool victory, string detail, float time)
    {
        menu.OpenScoreMenu(victory, detail, time);
    }
    public void OpenScoreMenu(bool victory, string detail, float time) // opens score menu and ends level
    {
        AddText(time_completed, TimerController.FormatedTime(time));
        float record = PlayerPrefs.GetFloat(level_key + " " + campaign_name + " " + level_code, -1.0f); // get record time
        if (victory) // Victory situation
        {
            ChangeText(result_text, "Success");
            ChangeColorText(result_text, ObjectivesController.instance.completed_objective_color);

            ChangeText(detail_text, detail);
            ChangeColorText(detail_text, ObjectivesController.instance.completed_objective_color);

            victory_text.SetActive(true);

            if(record > time || record == -1.0f)
            {
                AddText(record_time, TimerController.FormatedTime(time));
                LevelUnlocks.BeatLevel(campaign_name, level_code, time);
            }
            else
            {
                AddText(record_time, TimerController.FormatedTime(record));
            }
        }
        else // Defeat situation
        {
            ChangeText(result_text, "Fail");
            ChangeColorText(result_text, ObjectivesController.instance.failed_objective_color);

            ChangeText(detail_text, detail);
            ChangeColorText(detail_text, ObjectivesController.instance.failed_objective_color);

            // show defeat text
            defeat_text.SetActive(true);

            if (record == -1.0f) // level has never been completed
            {
                AddText(record_time, "NULL");
            }
            else
            {
                AddText(record_time, TimerController.FormatedTime(record));
            }
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Continue()
    {
        SceneManager.LoadScene(continue_scene);
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
    public static void AddText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text += text;
    }
    public static void ChangeColorText(GameObject given_object, Color color) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.color = color;
    }
}

class LevelUnlocks
{
    public const string LEVEL_KEY = "LEVEL";
    public const string UNDEFINED = "undefined";
    public static float RecordLevelTime(string campaign, string level_code)
    {
        return PlayerPrefs.GetFloat(LEVEL_KEY + " " + campaign + " " + level_code, -1.0f);
    }
    public static bool LevelBeaten(string campaign, string level_code)
    {
        return RecordLevelTime(campaign, level_code) != -1.0f;
    }
    public static void BeatLevel(string campaign, string level_code, float time)
    {
        if(campaign == UNDEFINED || level_code == UNDEFINED)
        {
            Debug.LogWarning("Level's code or campaign's name in Score menu Gameobject was not defined");
            return;
        }
        if(RecordLevelTime(campaign, level_code) == -1.0f || RecordLevelTime(campaign, level_code) > time)
            PlayerPrefs.SetFloat(LEVEL_KEY + " " + campaign + " " + level_code, time);
        PlayerPrefs.Save();
    }
    public static void StartLevel(string campaign, string level_code)
    {
        SceneManager.LoadScene(campaign + " " + level_code);
    }
}