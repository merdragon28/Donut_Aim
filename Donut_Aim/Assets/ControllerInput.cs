using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : MonoBehaviour
{
    //creates a line renderer and defines its width and length
    public LineRenderer laserLineRenderer;
    public float lineStartWidth;
    public float lineMaxEndWidth;
    public float laserMaxLength = 1000f;
    //defines how fast the player rotates the camera and moves in game
    private int rotationSpeed = 2;
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
    //dead angle (if vector is less than that angle camera will not move)
    public float deadzone;
    //direction player is aiming in
    Vector3 aimDirection;
    //necessary rays
    Ray forwardRay;
    Ray aimDirectionRay;
    void Start()
    {
        //ring.transform.localScale = new Vector3(ringSize, ringSize, ringSize);
        //hides cursor while game runs
        Cursor.visible = false;

        //Modified code from internet that sets Line render properties
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = lineStartWidth;
        laserLineRenderer.endWidth = lineMaxEndWidth;

        aimDirection = Vector3.zero;

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //scales rings size in game
        ring.transform.localScale = new Vector3(ringSize, ringSize, ringSize);

        bool fire = false;
        bool quit = false;
        Vector2 aimInput = Vector2.zero;
        //finds and sets gamepad
        if(Gamepad.current != null)
        {
            var gamepad = Gamepad.current;
            aimInput = gamepad.rightStick.ReadValue();
            fire = gamepad.rightTrigger.isPressed;
            quit = gamepad.startButton.isPressed;
        }
        //use mouse
        else if(Input.mousePresent)
        {
            aimInput = new Vector2((Input.mousePosition.x-(Screen.width/2))/(Screen.width/2),(Input.mousePosition.y-(Screen.height/2)) /(Screen.height/2));
            Debug.Log(aimInput);
            fire = Input.GetMouseButtonDown(1);
            quit = Input.GetKey(KeyCode.Escape);
            if (aimInput.x > 1)
                aimInput.x = 1;
            else if (aimInput.x < -1)
                aimInput.x = -1;
            if (aimInput.y > 1)
                aimInput.y = 1;
            else if (aimInput.y < -1)
                aimInput.y = -1;
        }

        if(quit)
        {
            Application.Quit();
        }

        //sets aiming Direction
        SetAimDirection(aimInput);

        forwardRay = new Ray(transform.position, transform.forward);
        aimDirectionRay = new Ray(transform.position, aimDirection);

        MoveCamera(aimInput, aimDirection);

        //makes the reticle to follow thet aim vector and changes its color when it collides
        MoveReticle(aimDirection, aimDirectionRay);

        //draws a line from center to aim reticle
        //ShootLaserFromTargetPosition(transform.forward*25, aimDirection, laserMaxLength);
        
        //feedback line
        SetLineRenderPoints(forwardRay.GetPoint(25), aimDirectionRay.GetPoint(25),5, aimInput);

        //Fire
        if (fire)
        {
            FireAction(aimDirectionRay);
        }

        //caues camera aim ring to follow cammera
        FollowCamera(aimInput, forwardRay);
    }

    void FollowCamera(Vector2 aimInput, Ray forwardRay)
    {
        forwardRay = new Ray(transform.position, transform.forward);
        ring.transform.position = forwardRay.GetPoint(25);
        ring.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1 * aimInput.y * rotationSpeed, aimInput.x * rotationSpeed, 0));
    }

    void SetLineRenderPoints(Vector3 start, Vector3 end, float deadzone, Vector2 aimInput)
    {
        //Debug.Log("Start: " + start + " End: " + end);
        if (Vector3.Distance(start, end) > deadzone)
        {
            Ray moveTowards = new Ray(start, (end - start).normalized);
            start = moveTowards.GetPoint(deadzone);
        }
        else
        { start = end; }
        laserLineRenderer.SetPosition(0, start);
        laserLineRenderer.SetPosition(1, end);
        laserLineRenderer.endWidth = aimInput.magnitude * lineMaxEndWidth;
    }

    void MoveReticle(Vector3 aimDirection, Ray aimingRay)
    {
        RaycastHit hit;
        //makes the reticle to follow thet aim vector and changes its color when it collides
        aimingRay = new Ray(transform.position, aimDirection);
        if (Physics.Raycast(aimingRay, out hit, laserMaxLength))
        {
            retmat.color = Color.green;
        }
        else
        {
            retmat.color = Color.red;
        }
        reticle.transform.position = aimingRay.GetPoint(25);
    }

    void SetAimDirection(Vector2 aimInput)
    {
        float fov = cam.fieldOfView;
        float aspect = cam.aspect;
        aimDirection = transform.forward + (transform.up * aimInput.y * (fov / 110)) + (transform.right * aimInput.x * (fov / 110) * aspect);
    }

    void MoveCamera(Vector2 aimInput, Vector3 aimDirection)
    {
        Debug.Log(aimInput.magnitude);
    //locks camera if in deadzone
    camlock = deadzone > aimInput.magnitude;
        //allows player to aim the camera
        if (camlock == false)
        {
            Vector2 startvelocity = aimInput;
            startvelocity = startvelocity.normalized*0.3f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-1 * (aimInput.y-startvelocity.y)* rotationSpeed, (aimInput.x-startvelocity.x)* rotationSpeed, 0));
        }
    }

    void FireAction(Ray aimingRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(aimingRay, out hit, laserMaxLength))
        {
            Debug.Log(hit.collider.GetComponent<MeshRenderer>().sharedMaterials[0]);
            hit.collider.GetComponent<MeshRenderer>().sharedMaterials[0].color = new Color (Random.value,Random.value,Random.value);
        }
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
