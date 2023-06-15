using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MouseController controls mouse selection of devices on the scene.

public class MouseController : MonoBehaviour
{
    public GameObject Selectbox; // sprite showing selected device
    public GameObject Selectbox_ghost;
    public static GameObject selected = null; // var stores selected device/GameObject
    public static GameObject selectbox;
    public static GameObject selectbox_ghost;
    void Start()
    {
        if (Selectbox == null) Debug.LogError("MouseController: No selectbox assigned!");
        selected = null;
        selectbox = Selectbox;
        selectbox.SetActive(false);
        selectbox_ghost = Selectbox_ghost;
        selectbox_ghost.SetActive(false);
    }
    void Update()
    {
        RaycastMouse();
    }

    public static GameObject GetSelected() // returns selected device
    {
        return selected;
    }

    private static void RaycastMouse() // looks for a selected device
    {
        if (!CommandsController.commands.console_input_only)
        {
            Vector2 origin = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D ui_hit = Physics2D.Raycast(origin, Vector2.zero, 0f, layerMask: ~5);
            if (!ui_hit)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 0f, layerMask: ~0);
                if (hit)
                {
                    if (hit.transform.gameObject.tag != "RaycastBlock")
                    {
                        if(Input.GetMouseButtonDown(0))
                        {
                            selected = hit.transform.gameObject;
                            selectbox.SetActive(true);
                            selectbox.transform.position = selected.transform.position;
                        }
                        else
                        {
                            selectbox_ghost.SetActive(true);
                            selectbox_ghost.transform.position = hit.transform.position;
                            DeviceScript device = hit.transform.gameObject.GetComponent<DeviceScript>();
                            if (!device.stealth_identity || device.CanPlayerSee(1, true))
                            {
                                Color color = PlayerManager.GetPlayerColor(device.player);
                                color.a = 0.5f;
                                selectbox_ghost.GetComponent<SpriteRenderer>().color = color;
                            }
                            else
                            {
                                Color color = PlayerManager.GetPlayerColor(-1);
                                color.a = 0.5f;
                                selectbox_ghost.GetComponent<SpriteRenderer>().color = color;
                            }
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        selected = null;
                        selectbox.SetActive(false);
                    }
                    selectbox_ghost.SetActive(false);
                }
                if (Input.GetMouseButtonDown(0)) LoggerController.LogSelected(selected);
            }
            else selectbox_ghost.SetActive(false);
        }
    }
}
