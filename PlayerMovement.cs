using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementForce = 100f;
    private Rigidbody rb;
    public float maxSpeed = 20f;

    [Header("Jumping")]
    public float jumpDistance = 1f;
    public float jumpForce = 200f;
    public LayerMask ground;

    [Header("Sliding")] public float slideForce = 200f;
    public float slideHeight = 1f;

    private float normalHeight;
    private CapsuleCollider capsule;
    private bool sliding = false;
    
    [Header("Camera")]
    public Camera cam;

    public float sensitivity= 50f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = gameObject.GetComponent<Rigidbody>();
        capsule = gameObject.GetComponent<CapsuleCollider>();
        normalHeight = capsule.height;
    }
    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (Grounded() && !isFast())
        {
            rb.AddForce(gameObject.transform.forward * movementForce * vertical * Time.deltaTime,  ForceMode.Impulse);
            rb.AddForce(gameObject.transform.right * movementForce * horizontal * Time.deltaTime,  ForceMode.Impulse);
        }
        
        cameraRotation();
        if(Input.GetKeyDown(KeyCode.Space) && Grounded() && !sliding)
            Jump();
        
        if(Input.GetKeyDown(KeyCode.LeftControl))
            OnSlide();
        
        if(Input.GetKeyUp(KeyCode.LeftControl))
            CancelSlide();
    }

    private void cameraRotation()
    {
        float cameraX = cam.transform.localRotation.eulerAngles.x;
        float cameraY = gameObject.transform.rotation.eulerAngles.y;
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        
        cameraX += mouseY * sensitivity;
        cameraY += mouseX * sensitivity;
        
        gameObject.transform.rotation = Quaternion.Euler(0, cameraY, 0);
        cam.transform.localRotation = Quaternion.Euler(cameraX, 0, 0);
    }

    private bool Grounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, jumpDistance, ground))
        {
            return true;
        }

        return false;
    }
    
    private void Jump()
    {
        rb.AddForce(gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    private void OnSlide()
    {
        capsule.height = slideHeight;
        rb.AddForce(gameObject.transform.forward * slideForce, ForceMode.Impulse);
        sliding = true;
    }

    private void CancelSlide()
    {
        capsule.height = normalHeight;
        sliding = false;
    }

    private bool isFast()
    {
        return rb.linearVelocity.magnitude > maxSpeed;
    }
}