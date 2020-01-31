using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Mov : MonoBehaviour
{
    private int moveSpeed = 4;
    private int rotationSpeed = 2;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Input.GetAxis("Vertical")*transform.forward*Time.deltaTime* moveSpeed;
        transform.position += Input.GetAxis("Horizontal")*transform.right*Time.deltaTime* moveSpeed;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1*Input.GetAxis("Mouse Y")* rotationSpeed, Input.GetAxis("Mouse X")* rotationSpeed, 0));
    }
}
