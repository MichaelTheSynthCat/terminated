using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstLevelTutorial : MonoBehaviour
{
    public GameObject guide_prefab;
    public List<GameObject> targets;
    public GameObject scan_guide;
    public GameObject cyberattack_guide;
    public GameObject email_guide;
    public GameObject commands_guide;

    private Dictionary<GameObject, GameObject> scan_selects = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> cyberattack_selects = new Dictionary<GameObject, GameObject>();

    private OperatorScript user_operator;

    private float blink_state = 0.0f;
    private const float blink_frequency = 0.4f;
    void Start()
    {
        user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        foreach (GameObject target in targets)
        {
            GameObject scan = Instantiate(guide_prefab, target.transform.position, Quaternion.identity);
            scan.GetComponent<SpriteRenderer>().color = Color.green;
            scan_selects.Add(target, scan);

            GameObject cyberattack = Instantiate(guide_prefab, target.transform.position, Quaternion.identity);
            cyberattack.GetComponent<SpriteRenderer>().color = Color.red;
            cyberattack_selects.Add(target, cyberattack);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        email_guide.SetActive(ToriumBlink.Instance.blink);

        GameObject selected = MouseController.GetSelected();
        bool enemy_selected = selected != null;
        if (enemy_selected) enemy_selected = selected.GetComponent<DeviceScript>().player != 1;

        bool show_guides = selected == null;
        if (!show_guides)
        {
            DeviceScript device = selected.GetComponent<DeviceScript>();
            show_guides = !(CommandPossible.IsPossible(CommandPossible.Scan(device, user_operator))
                || CommandPossible.IsPossible(CommandPossible.Cyberattack(device, user_operator)));
        }
        if (show_guides && user_operator.EmptyTaskSpace())
        {
            foreach(KeyValuePair<GameObject, GameObject> keyValuePair in scan_selects)
            {
                DeviceScript device = keyValuePair.Key.GetComponent<DeviceScript>();
                keyValuePair.Value.SetActive(CommandPossible.IsPossible(CommandPossible.Scan(device, user_operator)));
            }
            foreach (KeyValuePair<GameObject, GameObject> keyValuePair in cyberattack_selects)
            {
                DeviceScript device = keyValuePair.Key.GetComponent<DeviceScript>();
                keyValuePair.Value.SetActive(CommandPossible.IsPossible(CommandPossible.Cyberattack(device, user_operator)));
            }
        }
        else
        {
            foreach (KeyValuePair<GameObject, GameObject> keyValuePair in scan_selects)
            {
                keyValuePair.Value.SetActive(false);
            }
            foreach (KeyValuePair<GameObject, GameObject> keyValuePair in cyberattack_selects)
            {
                keyValuePair.Value.SetActive(false);
            }
        }

        if (selected != null)
        {
            DeviceScript device = selected.GetComponent<DeviceScript>();
            cyberattack_guide.SetActive(CommandPossible.IsPossible(CommandPossible.Cyberattack(device, user_operator)));
            scan_guide.SetActive(CommandPossible.IsPossible(CommandPossible.Scan(device, user_operator)));
        }
        else
        {
            cyberattack_guide.SetActive(false);
            scan_guide.SetActive(false);
        }

        if (blink_state <= 0.0f)
        {
            blink_state = blink_frequency;
            cyberattack_guide.GetComponent<Image>().enabled = !cyberattack_guide.GetComponent<Image>().enabled;
            scan_guide.GetComponent<Image>().enabled = !scan_guide.GetComponent<Image>().enabled;
        }
        else blink_state -= Time.deltaTime;

        commands_guide.SetActive(cyberattack_guide.activeInHierarchy || scan_guide.activeInHierarchy);
    }
}
