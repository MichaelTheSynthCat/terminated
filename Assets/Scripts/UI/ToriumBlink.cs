using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToriumBlink : MonoBehaviour, IPointerClickHandler
{
    public bool blink = false;

    private float blink_state = 0.0f;
    private float blink_time = 0.0f;
    private Image img;

    private const float blink_frequency = 0.2f;
    private const float blink_duration = 20.0f;
    public static ToriumBlink Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        img = GetComponent<Image>();
        if (blink) blink_time = blink_duration;
    }

    // Update is called once per frame
    void Update()
    {
        if(blink)
        {
            if (blink_state <= 0.0f)
            {
                img.enabled = !img.enabled;
                blink_state = blink_frequency;
            }
            else
            {
                blink_state -= Time.deltaTime;
            }
            if (blink_time <= 0.0f)
            {
                blink = false;
                img.enabled = false;
            }
            else blink_time -= Time.deltaTime;
        }
    }
    public static void Blink()
    {
        Instance.blink = true;
        Instance.blink_time = blink_duration;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        blink = false;
        img.enabled = false;
    }
}
