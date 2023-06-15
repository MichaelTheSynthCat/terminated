using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public string category = "";
    public string page_address = "";
    public bool master_page = false;
    private PageController last_open = null;

    private static Dictionary<string, PageController> master_pages = new Dictionary<string, PageController>();
    private void Awake()
    {
        master_pages.Clear();
    }
    void Start()
    {
        if (master_page)
        {
            master_pages.Add(category, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenSubPage(string address)
    {
        ActivatePage(address);
        if(transform.parent.GetComponent<PageController>() != null)
        {
            transform.parent.GetComponent<PageController>().OpenSubPage(gameObject.name);
        }
    }
    public void LocalOpenPage(string address)
    {
        GlobalOpenPage(address, category);
    }
    public static void GlobalOpenPage(string address, string category)
    {

    }
    public void OpenPage()
    {
        gameObject.SetActive(true);
    }
    public void ClosePage()
    {
        if (transform.parent.GetComponent<PageController>() != null)
        {
            transform.parent.GetComponent<PageController>().OpenSubPage("");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void ActivatePage(string page_name)
    {
        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child.GetComponent<PageController>() == null)
            {
                continue;
            }

            if (page_name == child_name)
            {
                child.gameObject.SetActive(true);
                last_open = child.gameObject.GetComponent<PageController>();
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
