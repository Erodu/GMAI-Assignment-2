using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    // Code is thanks to ChatGPT
    public Camera playerCam;
    public float moveSpeed = 10f;
    public float lookSpeed = 3f;
    public float lookLimit = 45f;

    Vector3 moveDir = Vector3.zero;

    float rotationX = 0;
    float rotationY = 0;

    CharacterController charControl;
    // Start is called before the first frame update
    void Start()
    {
        charControl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.forward * Input.GetAxis("Vertical");
        Vector3 right = transform.right * Input.GetAxis("Horizontal");
        moveDir = (forward + right) * lookSpeed;

        charControl.Move(moveDir * Time.deltaTime);

        // Mouse look
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookLimit, lookLimit);
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;

        playerCam.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
        transform.localEulerAngles = new Vector3(0, rotationY, 0);

        playerCam.transform.position = transform.position + new Vector3(0, 0.6f, 0);
    }
}
