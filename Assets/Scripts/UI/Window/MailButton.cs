using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailButton : MonoBehaviour
{
    public Sprite open_mail;
    public GameObject mail_image;
    public bool open_on_start = false;
    void Start()
    {
        if (open_on_start)
        {
            SetOpenMailSprite();
        }
    }

    public void SetOpenMailSprite()
    {
        mail_image.GetComponent<Image>().sprite = open_mail;
    }
}
