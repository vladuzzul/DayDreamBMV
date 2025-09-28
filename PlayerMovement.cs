using TMPro;
using UnityEngine;

public class PlayerMovementCamera : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity = 50f;

    [Header("UI")]
    public TextMeshProUGUI sprintStatusText; // <- legăm textul din inspector

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float cameraPitch = 0f;
    private PlayerStatus playerStatus;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerStatus = GetComponent<PlayerStatus>();
        Cursor.lockState = CursorLockMode.Locked;
        
        
        UpdateSprintUI(); // setăm textul la start
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        UpdateSprintUI(); // actualizăm UI-ul în fiecare frame
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = cam.transform.forward * vertical + cam.transform.right * horizontal;
        move.y = 0f;

        float speed = walkSpeed;

        if (playerStatus != null && playerStatus.canSprint)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = sprintSpeed;
            }
        }

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void UpdateSprintUI()
    {
        if (sprintStatusText == null || playerStatus == null) return;

        if (playerStatus.canSprint)
        {
            sprintStatusText.text = "Sprint Available";
            sprintStatusText.color = Color.green;
        }
        else
        {
            sprintStatusText.text = "Sprint Disabled";
            sprintStatusText.color = Color.red;
        }
    }
}
