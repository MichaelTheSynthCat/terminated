using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadedFiles : MonoBehaviour
{
    public List<string> INPUT_downloaded_files = new List<string>();
    private static List<string> downloaded_files = new List<string>();
    public const string DUMMY = "<dummy>";
    private void Awake()
    {
        if(INPUT_downloaded_files.Count > 0) downloaded_files = INPUT_downloaded_files;
    }
    void Start()
    {
        
    }
    void Update()
    {
        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child_name.Substring(0, 2) != "b-")
            {
                continue;
            }
            child.gameObject.SetActive(downloaded_files.Contains(child_name.Substring(2)));
        }
    }

    public static void SaveFile(string file)
    {
        if(file == DUMMY)
        {
            return;
        }
        downloaded_files.Add(file);
    }
}
