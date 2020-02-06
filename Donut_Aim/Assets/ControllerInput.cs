using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    //creates a line renderer and defines its width and length
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 1000f;
    //defines how fast the player rotates the camera and moves in game
    private int moveSpeed = 4;
    private int rotationSpeed = 2;
    //defines how far to draw debug ray
    int rayDistance = 100;

    void Start()
    {
        //hides cursor while game runs
        Cursor.visible = false;

        //Modified code from internet that sets Line render properties
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return; // No gamepad connected.



        //allows the player to straiff left and right from their current position and move forward or backward in the direction of the camera.
        Vector2 move = gamepad.leftStick.ReadValue();
        transform.position += move.y * transform.forward * Time.deltaTime * moveSpeed;
        transform.position += move.x * transform.right * Time.deltaTime * moveSpeed;

        //allows player to aim the camera
        Vector2 aim = gamepad.rightStick.ReadValue();
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1 * aim.y * rotationSpeed, aim.x * rotationSpeed, 0));

        //Fire
        if (gamepad.rightTrigger.isPressed)
        {
            
        }

        //Draws aiming Direction
        Vector3 aimDirection = transform.forward + transform.up * aim.y + transform.right * aim.x;
        Debug.DrawRay(transform.position, aimDirection * rayDistance, Color.green);

        ShootLaserFromTargetPosition(transform.position - new Vector3(0, 0.1f, 0), aimDirection, laserMaxLength);

        //copied from internet for drawing raycast
        void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
        {
            Ray ray = new Ray(targetPosition, direction);
            RaycastHit raycastHit;
            Vector3 endPosition = targetPosition + (length * direction);

            if (Physics.Raycast(ray, out raycastHit, length))
            {
                endPosition = raycastHit.point;
            }

            laserLineRenderer.SetPosition(0, targetPosition);
            laserLineRenderer.SetPosition(1, endPosition);
        }
    }
}
