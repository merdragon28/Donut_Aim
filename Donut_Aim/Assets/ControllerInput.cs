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

    //determines whether the camera can move or not
    public bool camlock = false;

    //the camera attachedto the player
    public Camera cam;

    //Reticle
    public GameObject reticle;
    public Material retmat;

    //MovementLimitRing
    public GameObject ring;
    public float ringSize = 5;

    //Rays
    Ray ray;
    RaycastHit raycastHit;

    //dead angle (if vector is less than that angle camera will not move)
    public float deadzone = 10f;

    void Start()
    {
        //ring.transform.localScale = new Vector3(ringSize, ringSize, ringSize);
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
        //scales rings size in game
        ring.transform.localScale = new Vector3(ringSize, ringSize, ringSize);

        //finds and sets gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return; // No gamepad connected.

        //allows the player to straiff left and right from their current position and move forward or backward in the direction of the camera.
        Vector2 move = gamepad.leftStick.ReadValue();
        transform.position += move.y * transform.forward * Time.deltaTime * moveSpeed;
        transform.position += move.x * transform.right * Time.deltaTime * moveSpeed;

        //Draws aiming Direction
        Vector2 aim = gamepad.rightStick.ReadValue();
        float fov = cam.fieldOfView;
        float aspect = cam.aspect;
        Vector3 aimDirection = transform.forward + (transform.up * aim.y * (fov / 110)) + (transform.right * aim.x * (fov / 110) * aspect);
        Debug.DrawRay(transform.position, aimDirection * rayDistance, Color.green);

        if (deadzone > Vector3.Angle(aimDirection, transform.forward))
        {
            camlock = true;
        }
        else
        {
            camlock = false;
        }

        //allows player to aim the camera
        if (camlock == false)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1 * aim.y * rotationSpeed, aim.x * rotationSpeed, 0));
        }

        //makes the reticle to follow thet aim vector and changes its color when it collides
        ray = new Ray(transform.position, aimDirection);
        if (Physics.Raycast(ray, out raycastHit, laserMaxLength))
        {
            retmat.color = Color.green;
        }
        else
        {
            retmat.color = Color.red;
        }
        reticle.transform.position = ray.GetPoint(25);

        //ShootLaserFromTargetPosition(transform.position - new Vector3(0, 0.1f, 0), aimDirection, laserMaxLength);

        //Fire
        if (gamepad.rightTrigger.isPressed)
        {
            if (Physics.Raycast(ray, out raycastHit, laserMaxLength))
            {
                Object thing = raycastHit.collider.gameObject;
                Destroy(thing);
            }
        }

        //caues camera aim ring to follow cammera
        ray = new Ray(transform.position, transform.forward);
        ring.transform.position = ray.GetPoint(25);
        ring.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1 * aim.y * rotationSpeed, aim.x * rotationSpeed, 0));
    }
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
