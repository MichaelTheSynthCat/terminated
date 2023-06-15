using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ServerScript must be attached to every server in the scene.

public class ServerScript : MonoBehaviour
{
    public List<string> downloadable_content; // all content that can be downloaded
    public bool data_generator = false; // if the device generates data resource
    public float data_power_cost = 500.0f; // how much power must be accumulated to create 1 stolen data resource
    private float accumulated_power = 0.0f;

    private GameObject data_generator_icon = null;

    public DeviceScript This_device { get; private set; }
    public ComputerScript This_computer { get; private set; }
    void Start()
    {
        This_device = GetComponent<DeviceScript>();
        This_computer = GetComponent<ComputerScript>();

        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child_name == "Effect Data generator")
            {
                data_generator_icon = child.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (data_generator && !This_device.IsTerminated()) // generate stolen data
        {
            accumulated_power += This_computer.GetTruePower() * Time.deltaTime;
            if (accumulated_power > data_power_cost)
            {
                int data = (int)(accumulated_power / data_power_cost);
                PlayerManager.AddStolenData(This_device.player, data);
                accumulated_power -= data * data_power_cost;
            }
        }
        // show data generation icon
        data_generator_icon.SetActive(data_generator && This_device.player == 1 && !This_device.IsTerminated());
    }
    public string DownloadContent() // returns last item in downloadable content
    {
        string file = downloadable_content[downloadable_content.Count - 1];
        downloadable_content.RemoveAt(downloadable_content.Count - 1);
        return file;
    }
    public bool HasDownloadableContent()
    {
        return downloadable_content.Count > 0;
    }
}
