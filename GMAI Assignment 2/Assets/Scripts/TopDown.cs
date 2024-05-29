using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDown : MonoBehaviour
{
    public float moveSpeed = 10f;
    CharacterController charControl;

    private void Start()
    {
        charControl = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horInput, 0, verInput).normalized;

        charControl.Move(moveDir * moveSpeed * Time.deltaTime);
    }
}
