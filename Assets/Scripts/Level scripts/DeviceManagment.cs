using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// DeviceManagment stores addresses of all devices in the scene and gives them a unique id.

public class DeviceManagment : MonoBehaviour
{
    private static int count = 0; // count of devices present in a scene, don't modify
    private static List<GameObject> devices = new List<GameObject>(); // all devices list
    private static Dictionary<string, GameObject> ip_list = new Dictionary<string, GameObject>();
    public List<GameObject> operators = new List<GameObject>(); // all operators list
    public static DeviceManagment instance;

    private static List<string> category_computer = new List<string>() { COMPUTER, "Desktop", "Notebook", };
    private static List<string> category_server = new List<string>() { SERVER, };
    private static List<string> category_router = new List<string>() { ROUTER, };

    public const string DEVICE = "Device";
    public const string COMPUTER = "Computer";
    public const string ROUTER = "Router";
    public const string SERVER = "Server";
    private void Awake()
    {
        instance = this;
        count = 0;
        devices.Clear();
        ip_list.Clear();

        // debug, check if all required Gameobjects are present in the scene
        if (GameObject.FindGameObjectsWithTag("LVL Selectbox").Length != 2)
            Debug.LogError("Invalid number of selectboxes!");
        if (GameObject.FindGameObjectsWithTag("Event System").Length != 1)
            Debug.LogError("Missing Event System!");
        if (GameObject.FindGameObjectsWithTag("MainCamera").Length != 2)
            Debug.LogError("Check MainCamera Gameobjects!");
    }
    private void Start()
    {
        // create device list based on IP addresses
        foreach(GameObject device in devices)
        {
            if (!ip_list.ContainsKey(device.GetComponent<DeviceScript>().GetIP()))
            {
                ip_list.Add(device.GetComponent<DeviceScript>().GetIP(), device);
            }
            else
            {
                Debug.LogWarning("IP duplicate - " + device.name + " : " + device.GetComponent<DeviceScript>().GetIP());
            }
        }
    }
    public static int Add_device(GameObject device) // add new device to the devices list
        // must be called by every device at the start of a scene
    {
        count += 1;
        devices.Add(device);
        return count;
    }
    public static GameObject GetDeviceByIP(string ip) // returns device based on the given IP address
    {
        return ip_list[ip];
    }
    public static GameObject GetOperator(int player) // get operator of the given player id
    {
        if(instance.operators[player] == null)
        {
            Debug.LogError("Operator for player " + player + " was not asigned or doesn't exist.");
            return null;
        }
        return instance.operators[player];
    }
    public static bool BelongsToCategory(GameObject given_object, string category) // belongs a given device to the given category
    {
        return BelongsToCategory(given_object.GetComponent<DeviceScript>().type, category);
    }
    public static bool BelongsToCategory(string given_type, string category) // belongs a given type to the given category
    {
        switch (category)
        {
            case COMPUTER:
                return category_computer.Contains(given_type);
            case SERVER:
                return category_server.Contains(given_type);
            case ROUTER:
                return category_router.Contains(given_type);
            case DEVICE:
                return true;
            default:
                Debug.LogError("Unknown category: " + category);
                return false;
        }
    }
    public static List<GameObject> AdminDevicesForPlayer(int ask_player) // returns all devices belonging to ask_player
    {
        List<GameObject> list = new List<GameObject>();
        foreach(GameObject gobject in devices)
        {
            if(gobject.GetComponent<OperatorScript>() != null)
            {
                if (gobject.GetComponent<OperatorScript>().dummy) continue;
            }
            if(gobject.GetComponent<DeviceScript>().player == ask_player)
            {
                list.Add(gobject);
            }
        }
        return list;
    }
    public static List<GameObject> VisibleDevicesForPlayer(int player, bool scan = false, string dev_type = "Device")
        // returns all devices that are visible to given player id
    {
        List<GameObject> visible = new List<GameObject>();
        foreach(GameObject gobject in devices)
        {
            DeviceScript device = gobject.GetComponent<DeviceScript>();
            if(device.CanPlayerSee(player, scan) && BelongsToCategory(gobject, dev_type))
            {
                visible.Add(gobject);
            }
        }
        return visible;
    }
    public static List<GameObject> GetAllDevicesList() 
    {
        return new List<GameObject>(devices);
    }
}
