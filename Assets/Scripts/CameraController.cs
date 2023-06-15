using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public bool normal_level = true;
    public bool main_menu = false;
    public bool moving = true; // enable camera moving
    public bool mouse_moving = true;
    public bool ban_mouse_moving = false;
    public int screen_boundary = 50;
    public float speed = 50.0f;

    public float max_x_boundary = -1.0f;
    public float max_y_boundary = -1.0f;

    public bool ban_scrolling = false;
    public bool mouse_scrolling = true;
    public float scrolling = 2.0f;
    public float biggest = 30.0f;
    public float smallest = 8.0f;

    private static CameraController main_camera;
    private CinemachineVirtualCamera vcam;
    private float z_position;
    private void Awake()
    {
        main_camera = this;
        z_position = transform.position.z;
    }
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (main_menu)
        { 
            if (MainMenuController.MainMenuActive() || LevelHolder.HolderActive()) return;
        }
        else if (normal_level)
        {
            if (ConsoleController.IsInputActive()) return;
        }

        if (moving)
        {
            // moving with mouse
            if (mouse_moving && !ban_mouse_moving)
            {
                if (Input.mousePosition.x > Screen.width - screen_boundary)
                {
                    MoveCamera(speed, 0.0f);
                    // transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
                }

                if (Input.mousePosition.x < 0 + screen_boundary)
                {
                    MoveCamera(-speed, 0.0f);
                    // transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
                }

                if (Input.mousePosition.y > Screen.height - screen_boundary)
                {
                    MoveCamera(0.0f, speed);
                    // transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
                }

                if (Input.mousePosition.y < 0 + screen_boundary)
                {
                    MoveCamera(0.0f, -speed);
                    // transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
                }

            }

            // moving with arrow keys
            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveCamera(speed, 0.0f);
                // transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveCamera(-speed, 0.0f);
                //transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveCamera(0.0f, speed);
                //transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveCamera(0.0f, -speed);
                // transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
        }

        // scrolling
        if (!ban_scrolling)
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus) ||
                        (Input.GetAxis("Mouse ScrollWheel") < 0f) && mouse_scrolling) // far out
            {
                ZoomOut();
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus) ||
                (Input.GetAxis("Mouse ScrollWheel") > 0f) && mouse_scrolling) // look closer
            {
                ZoomIn();
            }
        }
    }

    private void MoveCamera(float x, float y)
    {
        x *= Time.deltaTime;
        y *= Time.deltaTime;
        transform.Translate(new Vector3(x, y, 0.0f));
        if(max_x_boundary != -1.0f)
        {
            if(transform.position.x < 0.0f)
            {
                transform.position = new Vector3(0.0f, transform.position.y, z_position);
            }
            else if(transform.position.x > max_x_boundary)
            {
                transform.position = new Vector3(max_x_boundary, transform.position.y, z_position);
            }
        }
        if (max_y_boundary != -1.0f)
        {
            if (transform.position.y < 0.0f)
            {
                transform.position = new Vector3(transform.position.x, 0.0f, z_position);
            }
            else if (transform.position.y > max_y_boundary)
            {
                transform.position = new Vector3(transform.position.x, max_y_boundary, z_position);
            }
        }
    }
    public void ZoomIn()
    {
        vcam.m_Lens.OrthographicSize -= scrolling;
        if(vcam.m_Lens.OrthographicSize < smallest)
        {
            vcam.m_Lens.OrthographicSize = smallest;
        }
    }
    public void ZoomOut()
    {
        vcam.m_Lens.OrthographicSize += scrolling;
        if (vcam.m_Lens.OrthographicSize > biggest)
        {
            vcam.m_Lens.OrthographicSize = biggest;
        }
    }

    public static void LookAt(GameObject thing, bool game_ui_relative = false)
    {
        if(game_ui_relative) main_camera.transform.position = new Vector3(thing.transform.position.x, 
            thing.transform.position.y - main_camera.vcam.m_Lens.OrthographicSize * 0.33f,
            main_camera.transform.position.z);
        else main_camera.transform.position = new Vector3(thing.transform.position.x, thing.transform.position.y, 
            main_camera.transform.position.z);
    }
}
