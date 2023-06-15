using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    public string campaign;
    private bool open = false;

    public static Dictionary<string, LevelHolder> holders;
    private void Awake()
    {
        holders = new Dictionary<string, LevelHolder>();
    }
    void Start()
    {
        holders.Add(campaign, this);
    }

    void Update()
    {
        
    }
    public static void OpenLevel(string campaign, string level_code)
    {
        holders[campaign].OpenLevelStarter(level_code);
    }
    public void OpenLevelStarter(string level_code)
    {
        open = true;
        foreach (Transform child in transform)
        {
            if(child.GetComponent<LevelStarter>() != null)
            {
                child.gameObject.SetActive(child.GetComponent<LevelStarter>().code_level == level_code);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    public void CloseEverything()
    {
        open = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public static bool HolderActive()
    {
        foreach(LevelHolder holder in holders.Values)
        {
            if (holder.open)
            {
                return true;
            }
        }
        return false;
    }
}
