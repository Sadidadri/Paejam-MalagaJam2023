using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && controller.isGrounded;
    CharacterController controller;


    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    public Camera cam;
    public float mouseHorizontal = 3f;
    public float mouseVertical = 2f;
    public float minRotation = -65f;
    public float maxRotation = 60f;
    float h_mouse, v_mouse;

    private Vector3 move = Vector3.zero;
    private bool canCrouch = true;


    private KeyCode crouchKey = KeyCode.LeftControl;
    //Crouch parameters
    private float crouchHeight = 0.5f;
    private float standingHeight;
    private float timeToCrouch = 0.25f;
    private Vector3 crouchingCenter;
    private Vector3 standingCenter;
    private bool isCrouching;
    private bool duringCrouchAnimation;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        standingHeight = controller.height;
        crouchingCenter = controller.center;
        standingCenter = crouchingCenter;
        
    }

    // Update is called once per frame
    void Update()
    {
        h_mouse = mouseHorizontal * Input.GetAxis("Mouse X");
        v_mouse += mouseVertical * Input.GetAxis("Mouse Y");

        v_mouse = Mathf.Clamp(v_mouse, minRotation, maxRotation);
        cam.transform.localEulerAngles = new Vector3(-v_mouse,0f,0f);
        transform.Rotate(0f,h_mouse,0f);

    

        if (controller.isGrounded){
            move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            if (Input.GetKey(KeyCode.LeftShift)){
                 move = transform.TransformDirection(move)*runSpeed;
            } else {
                move = transform.TransformDirection(move)*walkSpeed;
            }

            
            if (Input.GetKeyDown(KeyCode.Space)){
                move.y = jumpSpeed;
            }
            
        }

        move.y -= gravity*Time.deltaTime;

        controller.Move(move*Time.deltaTime);

        if (canCrouch){
            HandleCrouch();
        }
        
    }


    private void HandleCrouch(){
        if (ShouldCrouch){
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand(){
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = controller.height;

        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while(timeElapsed < timeToCrouch){
            controller.height = Mathf.Lerp(currentHeight,targetHeight,timeElapsed/timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter,targetCenter,timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height =  targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}
