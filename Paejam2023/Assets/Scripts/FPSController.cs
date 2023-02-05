using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSController : MonoBehaviour
{

    private bool ShouldCrouch => Input.GetKey(crouchKey) && !duringCrouchAnimation && controller.isGrounded;
    CharacterController controller;

    public GameObject txtInte;
    public GameObject txtBuscaCandado;
    public GameObject obj1;



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
    float timer;
    public float timeDelayToCrouch = 0.6f;
    //RayCast
    public float distanceToSee;
    RaycastHit whatIHit;
    //TextoBuscaPistas
    public GameObject txtHints;

     public GameObject txtCasoResuelto;

     public GameObject Puerta;
    public Animator puertaAnim;

    private bool doorOpened = false;

    public GameObject vid1, vid2, vid3, vid4;
    public GameObject videoFinal;

     //Pausa
    private KeyCode escapeKey = KeyCode.Escape;
    private bool isPaused = false;
    public GameObject pauseMenu;

    Vector3 posPuertaOriginal;

    Vector3 posPuertaFinal;

    //Objetos coleccionables
    private bool keyObtained = false;
    private bool cigaretteObtained = false;
    private bool carObtained = false;
    private bool dogObtained = false;
    private bool bananaObtained = false;


    // Start is called before the first frame update
    void Start()
    {
        timer = Time.time;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        standingHeight = controller.height;
        crouchingCenter = controller.center;
        standingCenter = crouchingCenter;
        txtInte.SetActive(false);
        txtHints.SetActive(true);
        Invoke("hideHintsTxt",4);
        puertaAnim.Play("Puerta");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (keyObtained && carObtained && bananaObtained && dogObtained && cigaretteObtained){
            
            Invoke("LoadCasoResuelto",11)
        }



        //pause handle
         if (Input.GetKeyDown(escapeKey)){
            handlePause();
        }
        

        timer += Time.deltaTime;

        txtInte.SetActive(false);
        //Raycasting
        Debug.DrawRay(cam.transform.position,cam.transform.forward * distanceToSee, Color.magenta);
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out whatIHit,distanceToSee)){

            if( whatIHit.collider.gameObject.tag == "PuertaValla" && !doorOpened){
                if(keyObtained){
                    txtInte.SetActive(true);
                     if (Input.GetKey(KeyCode.E)){
                        //Destroy(whatIHit.transform.gameObject);
                        Debug.Log("Abrir");
                        puertaAnim.Play("AbrirPuerta");
                        txtInte.SetActive(false);
                        doorOpened = true;
                     }
                }else{
                    txtBuscaCandado.SetActive(true);
                    Invoke("hideBuscaCandado",2.5);
                }
            }

            if (whatIHit.collider.gameObject.tag == "ObjectoSeleccionable"){             
              
                 switch(whatIHit.transform.gameObject.name){
                    case "llave":
                        txtInte.SetActive(true);
                        break;
                    case "cigarro":
                        if (!cigaretteObtained){
                              txtInte.SetActive(true);
                        }
                        break;
                    case "Coche_LP":  
                        if (!carObtained){
                              txtInte.SetActive(true);
                        }
                        break;
                    case "PERRO":
                        if (!dogObtained){
                              txtInte.SetActive(true);
                        }
                        break;
                    case "Badano":
                        if (!bananaObtained){
                              txtInte.SetActive(true);
                        }
                        break;
                }

                if (Input.GetKey(KeyCode.E)){
                txtInte.SetActive(false);
                switch(whatIHit.transform.gameObject.name){
                    case "llave":
                        Destroy(whatIHit.transform.gameObject);
                        keyObtained = true;
                        break;
                    case "cigarro":
                        cigaretteObtained = true;
                        vid1.SetActive(true);
                        Invoke("desaparecerVideos",4);
                        break;
                    case "Coche_LP":  
                        carObtained = true;
                        vid2.SetActive(true);
                        Invoke("desaparecerVideos",4);
                        break;
                    case "PERRO":
                        dogObtained = true;
                        vid3.SetActive(true);
                        Invoke("desaparecerVideos",4);
                        break;
                    case "Badano":
                        bananaObtained = true;
                        vid4.SetActive(true);
                        Invoke("desaparecerVideos2",9);
                        break;
                }                
                }
         }
        }

        h_mouse = isPaused ? 0 : mouseHorizontal * Input.GetAxis("Mouse X");
        v_mouse = isPaused ? v_mouse : v_mouse + mouseVertical * Input.GetAxis("Mouse Y");

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
             if(timer >= timeDelayToCrouch){
                StartCoroutine(CrouchStand());
                timer = 0;
             }
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

    void desaparecerVideos(){
        vid1.SetActive(false);
        vid2.SetActive(false);
        vid3.SetActive(false);
    }

    void desaparecerVideos2(){
        vid4.SetActive(false);
    }

    public void handlePause(){
        if (!isPaused){
            Time.timeScale = 0f;
        }else{
            Time.timeScale = 1f;
        }

        Cursor.visible = !Cursor.visible;

        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
    }

    public void Exit(){
        handlePause();
        SceneManager.LoadScene("Menu");
    }

    private void hideHintsTxt(){
        txtHints.SetActive(false);
    }

    private void hideBuscaCandado(){
        txtBuscaCandado.SetActive(false);
    }


    private void LoadCasoResuelto(){
        txtCasoResuelto.setActive(true);
        Invoke("LoadFinalVideo",3);
    }

    private void LoadFinalVideo(){
        txtCasoResuelto.setActive(false);
        videoFinal.setActive(true);
        Invoke("BackToMenu",33);
    }
    private void BackToMenu(){
        SceneManager.LoadScene("Menu");
    }


    // void OnTriggerEnter(Collider col){
    //     if(col.gameObject.tag == "Evento1"){
    //         txtInte.SetActive(true);
    //     }
    // }

    // void OnTriggerExit(Collider col){
    //     if(col.gameObject.tag == "Evento1"){
    //         txtInte.SetActive(false);
    //     }
    // }

    // void OnTriggerStay(Collider col){
    //     if(col.gameObject.tag == "Evento1"){
    //         if (Input.GetKey(KeyCode.E)){
    //             Destroy(obj1);
    //             txtInte.SetActive(false);
    //         }
    //     }
    // }
}
