using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public GameObject player;
    public Transform[] exhibits;
    public Transform chargePoint;
    public Transform entrance;

    public float interactDistance = 2f;
    public float approachDistance = 5f;

    public bool isCharging { get; private set; } = false;
    public float lowCharge { get; private set; } = 5f;
    public float charge { get; set; } = 100f;
    public float batteryTime = 5f;
    public float chargeTime = 5f;

    public float speed { get; private set; } = 5f;

    void Start()
    {
        
    }

    public void Charge(bool isCharging)
    {
        this.isCharging = isCharging;
    }

    void Update()
    {
        if (isCharging)
        {
            charge += 100f / chargeTime * Time.deltaTime;
            if (charge >= 100f)
            {
                isCharging = false;
                charge = 100f;
                return;
            }
        }
        else
        {
            charge -= 100f / batteryTime * Time.deltaTime;
            if (charge <= 0f)
            {
                charge = 0f;
                return;
            }
        }
    }
}
