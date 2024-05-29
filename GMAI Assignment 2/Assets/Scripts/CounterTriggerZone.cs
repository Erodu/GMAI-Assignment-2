using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterTriggerZone : MonoBehaviour
{
    [SerializeField] PotionMakerClass potionMaker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && potionMaker.approachButtonAffected == true)
        {
            Debug.Log(potionMaker.approachButtonAffected);
            potionMaker.btn_Approach.SetActive(true); // Set this to true when the player is in the trigger zone.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && potionMaker.approachButtonAffected == true)
        {
            Debug.Log(potionMaker.approachButtonAffected);
            potionMaker.btn_Approach.SetActive(false); // Vice versa.
        }
    }
}
