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
    float crouchSpeed = 9.0f;
    [SerializeField]
    float walkSpeed = 12.0f;

    [SerializeField]
    float camYSpeed = 10.0f;
    [SerializeField]
    float camXSpeed = 10.0f;

    //Animator Parameters
    public bool isMoving;
    public bool isCrouched;
    [Range(-1, 1)]
    float velocityY;
    float velocityX;

    public int life = 3;
    public int areasPassed = 0;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        if (Time.timeScale != 0)
        {
            float mouseX = Input.GetAxis("Mouse X") * camXSpeed;
            transform.Rotate(new Vector3(0, mouseX, 0));

            float mouseY = -Input.GetAxis("Mouse Y") * camYSpeed;
            camArm.transform.Rotate(mouseY, 0, 0);

            if (camArm.transform.localEulerAngles.x < 320 && camArm.transform.localEulerAngles.x > 50)
                camArm.transform.localEulerAngles = new Vector3(320, 0, 0);
            if (camArm.transform.localEulerAngles.x > 30 && camArm.transform.localEulerAngles.x < 300)
                camArm.transform.localEulerAngles = new Vector3(30, 0, 0);
        }
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

        isCrouched = Input.GetButton("Crouch");


        if (isCrouched) { move = direction * crouchSpeed * Time.deltaTime; }
        else move = direction * walkSpeed * Time.deltaTime;

        transform.Translate(move);

        //Animator Movements
        isMoving = new Vector3(horizontal, 0, forward).magnitude > 0.2f;

        anim.SetBool("isMoving", isMoving);
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
        Cursor.lockState = CursorLockMode.None;
        canvas.GameOver();
        Time.timeScale = 0;
    }
}
