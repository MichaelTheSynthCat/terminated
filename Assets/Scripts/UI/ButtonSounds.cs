using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public bool enter_sound = true;
    public bool click_sound = true;
    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(enter_sound && GetComponent<Button>().IsInteractable()) SFXPlayer.PlaySFX(SFXPlayer.IN_BUTTON);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (click_sound && GetComponent<Button>().IsInteractable()) SFXPlayer.PlaySFX(SFXPlayer.CLICK_BUTTON);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
