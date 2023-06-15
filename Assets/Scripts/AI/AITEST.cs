using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITEST : MonoBehaviour
{
    private OperatorScript ai_operator;
    public GameObject attack_device;
    void Start()
    {
        ai_operator = GetComponent<OperatorScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attack_device != null)
        {
            ai_operator.CyberAttack(attack_device);
        }
    }
}
