using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfferController : MonoBehaviour
{
    public string product;
    public string bonus;
    public float value;
    public int catcoin_cost = 1;

    public GameObject sold_object;
    public GameObject button;

    public static Dictionary<string, OfferController> offers = new Dictionary<string, OfferController>();
    private void Awake()
    {
        offers.Clear();
    }
    void Start()
    {
        offers.Add(product, this);
    }
    void Update()
    {
        if (!DeviceManagment.GetOperator(1).GetComponent<OperatorScript>().Installed_upgrades.Contains(product))
        {
            
        }
        else
        {
            sold_object.SetActive(true);
            button.GetComponent<Button>().interactable = false;
        }
    }
    public void Buy()
    {
        UserController.InstallUpgrade(product, catcoin_cost);
    }
}
