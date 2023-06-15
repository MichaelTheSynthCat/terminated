using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject main_menu_object;
    public static MainMenuController instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Time.timeScale = 1.0f;
    }
    void Update()
    {
        
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public static bool MainMenuActive()
    {
        return instance.main_menu_object.activeSelf;
    }
}
