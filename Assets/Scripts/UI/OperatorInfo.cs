using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// OperatorInfo class controls Operator Info Frame UI

public class OperatorInfo : MonoBehaviour
{
    private OperatorScript user_operator;

    public GameObject image_operator;
    // GameObjects with TextMeshProUGUI
    public GameObject integrity;
    public GameObject power;
    public GameObject defense;
    public GameObject security;
    public GameObject slaves;
    public GameObject stasis;

    // 
    public GameObject VisibleEffect;
    public GameObject VPNEffect;
    public GameObject DangerEffect;
    public GameObject VisibleVPNEffect;
    public GameObject DangerVPNEffect;
    // Start is called before the first frame update
    void Start()
    {
        // get operator of player 1
        user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        image_operator.GetComponent<Image>().color = PlayerManager.GetPlayerColor(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(user_operator == null)
        {
            user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        }
        // change texts of TextMeshProUGUI objects
        ChangeText(integrity,
            user_operator.This_device.GetTrueIntegrity().ToString("0") + "/" +
            user_operator.This_device.GetTrueIntegrity(true).ToString("0"));
        ChangeText(power, user_operator.GetSuperPower().ToString("0"));
        ChangeText(defense, user_operator.This_device.GetTrueDefense().ToString("0"));
        ChangeText(security, user_operator.This_device.GetTrueSecurity().ToString("0"));
        ChangeText(slaves, user_operator.slaves.Count.ToString() + "/" + user_operator.slaves_capacity.ToString());
        ChangeText(stasis, user_operator.stasis_charges.ToString() + "/" + user_operator.stasis_charges_max);

        // check visibility to other players
        VisibleEffect.SetActive(user_operator.This_device.IsVisibleToEnemies());

        // check VPN and safe mode
        VPNEffect.SetActive(user_operator.vpn_connection != null);
        DangerEffect.SetActive(!user_operator.This_device.IsSafe());

        VisibleVPNEffect.SetActive(false);
        DangerVPNEffect.SetActive(false);
        if(user_operator.vpn_connection != null)
        {
            VisibleVPNEffect.SetActive(user_operator.vpn_connection.GetComponent<DeviceScript>().IsVisibleToEnemies());
            DangerVPNEffect.SetActive(!user_operator.vpn_connection.GetComponent<DeviceScript>().IsSafe());
        }
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
}
