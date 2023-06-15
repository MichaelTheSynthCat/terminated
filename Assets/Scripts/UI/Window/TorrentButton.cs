using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TorrentButton : MonoBehaviour
{
    public string torrent_id = "";
    public int pcs_cost = 0;
    public string bonus;
    public float value;
    public GameObject text_object;
    public static Dictionary<string,TorrentButton> torrents = new Dictionary<string, TorrentButton>();
    private void Awake()
    {
        torrents.Clear();
    }
    void Start()
    {
        torrents.Add(torrent_id, this);
    }
    void Update()
    {
        if (!DeviceManagment.GetOperator(1).GetComponent<OperatorScript>().Installed_torrents.Contains(torrent_id))
        {
            text_object.GetComponent<TextMeshProUGUI>().text = "Install now";
        }
        else
        {
            text_object.GetComponent<TextMeshProUGUI>().text = "Already installed";
            GetComponent<Button>().interactable = false;
        }
    }
    public void Install()
    {
        UserController.StartInstallTorrent(torrent_id, pcs_cost);
    }
}
