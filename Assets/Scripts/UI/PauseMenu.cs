using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pause_object;
    public GameObject documentation_object;
    private static bool paused;
    private static bool ended;
    public static PauseMenu pausemenu;
    private void Awake()
    {
        pausemenu = this;
        paused = false;
        ended = false;
    }
    void Start()
    {
        ContinueGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (ObjectivesController.GameGoingToEnd())
        {
            pause_object.SetActive(false);
            documentation_object.SetActive(false);
            return;
        }

        if (ended)
        {
            PauseGame();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F12) || Input.GetKeyDown(KeyCode.End))
        {
            documentation_object.SetActive(false);
            if (paused)
            {
                OpenPauseMenu(false);
            }
            else
            {
                OpenPauseMenu(true);
            }
        }
        if ((Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.PageUp)) && !paused)
        {
            documentation_object.SetActive(!documentation_object.activeSelf);
        }
        if (documentation_object.activeSelf)
        {
            PauseGame();
        }
    }

    public static void ContinueGame()
    {
        paused = false;
        Time.timeScale = 1.0f;
    }
    public static void PauseGame()
    {
        paused = true;
        Time.timeScale = 0.0f;
    }
    public static void OpenPauseMenu(bool open)
    {
        pausemenu.pause_object.SetActive(open);
        if (open)
        {
            PauseGame();
        }
        else
        {
            ContinueGame();
        }
    }

    public static bool IsGamePaused()
    {
        return paused;
    }
    public static void EndGame()
    {
        ended = true;
        PauseGame();
    }
    public static void ExitToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
