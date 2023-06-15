using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string un_lock_condition = "";
    public string campaign = "";
    public string level_code = "";
    public GameObject text_level_code;
    public GameObject text_level_name;

    private Button button;

    public const string UNDEFINED = "undefined";
    private void Awake()
    {
        if (campaign == "") campaign = UNDEFINED;
        if (level_code == "") level_code = UNDEFINED;
    }
    void Start()
    {
        text_level_code.GetComponent<TextMeshProUGUI>().text = campaign + " " + level_code;
        button = GetComponent<Button>();
        button.interactable = Unlocked();
        text_level_code.SetActive(false);
        text_level_name.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Unlocked()
    {
        if (un_lock_condition == "") return true;
        if (un_lock_condition == "locked") return false;
        string[] arguments = un_lock_condition.Split();
        if(arguments[0] == "level")
        {
            return LevelUnlocks.LevelBeaten(arguments[1], arguments[2]);
        }
        Debug.LogError("Invalid level unlock condition: " + gameObject.name + " - " + un_lock_condition);
        return false;
    }
    public void OpenLevel()
    {
        LevelHolder.OpenLevel(campaign, level_code);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        text_level_code.SetActive(true);
        text_level_name.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text_level_code.SetActive(false);
        text_level_name.SetActive(false);
    }
}
