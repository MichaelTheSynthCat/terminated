using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandInteractable : MonoBehaviour
{
    public bool selected;
    public bool admin_selected;
    public bool enemy_selected;
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
    }
    void Update()
    {
        button.interactable = true;
        if (selected)
        {
            if (!SomethingSelected())
            {
                button.interactable = false;
                return;
            }

            DeviceScript device = MouseController.GetSelected().GetComponent<DeviceScript>();
            if (admin_selected)
            {
                if(device.player != 1)
                {
                    button.interactable = false;
                    return;
                }
            }
            if (enemy_selected)
            {
                if (PlayerManager.ArePlayersFriendly(1, device.player))
                {
                    button.interactable = false;
                    return;
                }
            }
        }
    }
    private bool SomethingSelected()
    {
        return MouseController.GetSelected() != null;
    }
}
