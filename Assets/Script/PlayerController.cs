using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;
    GameObject camArm;
    CanvasController canvas;

    //Movement Parameters
    [SerializeField]
    float crouchSpeed = 2.0f;
    [SerializeField]
    float walkSpeed = 5.0f;
    [SerializeField]
    float runSpeed = 10.0f;
    [SerializeField]
    float camYSpeed = 10.0f;
    [SerializeField]
    float camXSpeed = 10.0f;

    //Animator Parameters
    public bool isMoving;
    public bool isRunning;
    public bool isCrouched;
    [Range(-1, 1)]
    float velocityY;
    float velocityX;

    public int life = 3;
    public int areasPassed = 0;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        camArm = GameObject.FindWithTag("CameraArm");
        canvas = FindObjectOfType<CanvasController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovements();
        CameraMovements();

    }

    private void CameraMovements()
    {
        //Camera Movements
        float mouseX = Input.GetAxis("Mouse X") * camXSpeed;
        transform.Rotate(new Vector3(0, mouseX, 0));

        float mouseY = -Input.GetAxis("Mouse Y") * camYSpeed;
        camArm.transform.Rotate(mouseY, 0, 0);

        if (camArm.transform.localEulerAngles.x < 320 && camArm.transform.localEulerAngles.x > 50)
            camArm.transform.localEulerAngles = new Vector3(320, camArm.transform.localEulerAngles.y, camArm.transform.localEulerAngles.z);
        if (camArm.transform.localEulerAngles.x > 30 && camArm.transform.localEulerAngles.x < 300)
            camArm.transform.localEulerAngles = new Vector3(30, camArm.transform.localEulerAngles.y, camArm.transform.localEulerAngles.z);
    }

    private void PlayerMovements()
    {
        //Player Movements
        float forward = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, forward).normalized;
        Vector3 move;
        velocityX = horizontal;
        velocityY = forward;

        isRunning = Input.GetButton("Run");
        isCrouched = Input.GetButton("Crouch");

        //prioritse running
        if (isRunning && isCrouched) isCrouched = false;


        if (isRunning) { move = new Vector3(0, 0, forward) * runSpeed * Time.deltaTime; }
        else if (isCrouched) { move = direction * crouchSpeed * Time.deltaTime; }
        else move = direction * walkSpeed * Time.deltaTime;

        transform.Translate(move);

        //Animator Movements
        isMoving = new Vector3(horizontal, 0, forward).magnitude > 0.2f;

        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isCrouched", isCrouched);
        anim.SetFloat("velocityY", velocityY);
        anim.SetFloat("velocityX", velocityX);
    }

    public void HitByBullet()
    {
        life--;
        canvas.HitByBullet();
        if(life <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        canvas.GameOver();
        Time.timeScale = 0;
    }
}
