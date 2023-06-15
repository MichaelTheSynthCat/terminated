using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcesInfo : MonoBehaviour
{
    public GameObject pcs_text;
    public GameObject catcoin_text;
    public GameObject stolendata_text;
    void Update()
    {
        ChangeText(pcs_text, PlayerManager.GetPCS(1).ToString());
        ChangeText(catcoin_text, PlayerManager.GetCatCoin(1).ToString());
        ChangeText(stolendata_text, PlayerManager.GetStolenData(1).ToString());
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
}
