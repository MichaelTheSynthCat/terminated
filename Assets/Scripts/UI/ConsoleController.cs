using TMPro;
using UnityEngine;
using System.Collections.Generic;
// ConsoleController operates the console window, shows useful info in real-time (notifications, errors, warnings).

public class ConsoleController : MonoBehaviour
{
    public GameObject quick_console;
    public GameObject console_input;
    public GameObject big_console_frame;
    public GameObject big_console;

    public Color INPUT_normal;
    public Color INPUT_success;
    public Color INPUT_warning;
    public Color INPUT_error;

    private static Color normal;
    private static Color success;
    private static Color warning;
    private static Color error;

    private static Queue<string> last_messages = new Queue<string>();
    private static string constant_text;
    private static Color constant_color;
    private static string quick_text;
    private static Color quick_color;
    private const float qucik_text_deadline = 2.0f;
    private static float quick_text_left = 0.0f;

    private const float blink_duration = 0.1f;
    private static float blink_now = 0.0f;
    private const int big_console_rows_limit = 12;

    private static ConsoleController console_holder;
    private static TextMeshProUGUI quick_console_text, big_console_text;
    private static TMP_InputField inputfield_text;
    private static Queue<string> last_commands = new Queue<string>();
    private void Awake()
    {
        console_holder = this;
        last_messages.Clear();
        blink_now = 0.0f;
        last_commands.Clear();
    }

    private void Start()
    {
        normal = INPUT_normal;
        success = INPUT_success;
        warning = INPUT_warning;
        error = INPUT_error;

        quick_console_text = quick_console.GetComponent<TextMeshProUGUI>();
        big_console_text = big_console.GetComponent<TextMeshProUGUI>();
        inputfield_text = console_input.GetComponent<TMP_InputField>();
        ShowText("Hello world");
        big_console_text.text = "";
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12) || Input.GetKeyDown(KeyCode.End) ||
            Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.PageUp) ||
            (console_input.activeSelf && !inputfield_text.isFocused))
        {
            CloseInput();
        }
        else if (PauseMenu.IsGamePaused() ||
            (console_input.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))))
        {
            CloseInput();
        }
        else if (!inputfield_text.isFocused && 
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            console_input.SetActive(true);
            inputfield_text.Select();
            inputfield_text.ActivateInputField();
        }

        big_console_frame.SetActive(IsInputActive());
        if(IsInputActive())
        {
            string messages = "";
            foreach (string message in last_messages)
            {
                messages += message + "\n";
            }
            big_console_text.text = messages;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                string[] comms = last_commands.ToArray();
                inputfield_text.text = comms[comms.Length - 1];
            }
        }
        quick_console.SetActive(!IsInputActive());


        if(quick_text_left > 0.0f)
        {
            quick_console_text.text = quick_text;
            if(blink_now > 0.0f)
            {
                quick_console_text.color = quick_color;
            }
            else
            {
                quick_console_text.color = normal;
                if(blink_now < -blink_duration)
                {
                    blink_now = blink_duration;
                }
            }
            quick_text_left -= Time.deltaTime;
            blink_now -= Time.deltaTime;
        }
        else
        {
            quick_console_text.text = constant_text;
            quick_console_text.color = constant_color;
        }
    }

    public static void ShowText(string input, string type = "normal") // show new constant text
    {
        constant_text = TimerController.FormatedTime() + " " + input;
        constant_color = GetTypeColor(type);
        quick_text_left = 0.0f;

        last_messages.Enqueue(constant_text);
        if(last_messages.Count > big_console_rows_limit)
        {
            last_messages.Dequeue();
        }
    }

    public static void ShowQuickText(string input, string type = "error")
    {
        quick_text = "ERROR " + input;
        quick_color = GetTypeColor(type);
        quick_text_left = qucik_text_deadline;
        blink_now = blink_duration;
    }
    public static void ShowUserProcessError(string process_name, DeviceScript device, string detail)
    {
        ShowText(process_name + " {" + device.GetIP() + "}, debug: "
            + CommandPossible.GetUserProcessErrorMessage(detail, false), "error");
        SFXPlayer.PlaySFX(SFXPlayer.ERROR);
    }
    public static bool IsInputActive()
    {
        return console_holder.console_input.activeSelf;
    }
    public static void CloseInput()
    {
        inputfield_text.text = "";
        console_holder.console_input.SetActive(false);
    }
    public void ExecuteInput()
    {
        if(inputfield_text.text != "")
        {
            UserController.ExecuteConsoleCommand(inputfield_text.text);
            last_commands.Enqueue(inputfield_text.text);
        }
    }
    private static Color GetTypeColor(string type)
    {
        switch (type)
        {
            case "normal":
                return normal;
            case "success":
                return success;
            case "warning":
                return warning;
            case "error":
                return error;
            default:
                return normal;
        }
    }
}
