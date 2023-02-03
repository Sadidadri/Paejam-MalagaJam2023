using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        h_mouse = mouseHorizontal * Input.GetAxis("Mouse X");
        v_mouse += mouseVertical * Input.GetAxis("Mouse Y");

        v_mouse = Mathf.Clamp(v_mouse, minRotation, maxRotation);
        cam.transform.localEulerAngles = new Vector3(-v_mouse,0f,0f);
        transform.Rotate(0f,h_mouse,0f);

        Debug.Log(Input.GetAxis("Mouse X"));

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
    }
}
