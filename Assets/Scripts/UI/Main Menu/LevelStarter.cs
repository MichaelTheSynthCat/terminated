using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelStarter : MonoBehaviour
{
    public string code_level;
    public string par_time = "0:00";

    public GameObject times_object;
    public GameObject difficulty_object;

    public const string EASY = "Easy";
    public const string NORMAL = "Normal";
    public const string HARD = "Hard";
    void Start()
    {
        float record_time = PlayerPrefs.GetFloat(ScoreMenu.level_key + " " + 
            transform.parent.gameObject.GetComponent<LevelHolder>().campaign + " " +
            code_level, -1.0f);
        
        if(record_time == -1.0f)
        {
            ChangeText(times_object,
                par_time + "\n" + "NULL");
        }
        else
        {
            ChangeText(times_object,
                par_time + "\n" + TimerController.FormatedTime(record_time));
        }

        string difficulty = difficulty_object.GetComponent<TextMeshProUGUI>().text;
        switch (difficulty)
        {
            case EASY:
                ChangeColorText(difficulty_object, Color.green);
                break;
            case HARD:
                ChangeColorText(difficulty_object, Color.red);
                break;
        }
    }
    public void LaunchLevel()
    {
        SceneManager.LoadScene(transform.parent.gameObject.GetComponent<LevelHolder>().campaign + " " +
            code_level);
    }
    public void CloseWindow()
    {
        transform.parent.gameObject.GetComponent<LevelHolder>().CloseEverything();
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
    public static void ChangeColorText(GameObject given_object, Color color) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.color = color;
    }
}
