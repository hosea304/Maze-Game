using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Maze Settings")]
    public Vector2Int entrance = new Vector2Int(0, 0);  // Posisi pintu masuk

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera playerCamera;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Adjust player positioning relative to the entrance
        // Assumes the entrance is at (0,0) and walls are 2 units apart
        transform.position = new Vector3(entrance.x - 2f, 1f, entrance.y); // Menempatkan pemain tepat di depan pintu masuk
        transform.rotation = Quaternion.Euler(0f, 90f, 0f); // Menghadap ke pintu masuk
    }

    private void Update()
    {

        RaycastHit hit;
        Vector3 forwardDirection = transform.forward;
        if (Physics.Raycast(transform.position, forwardDirection, out hit, 1f)) // 1f adalah jarak cek
        {
            if (hit.collider.CompareTag("Wall"))
            {
                // Jika raycast mengenai wall, stop gerakan atau hentikan player.
                Debug.Log("Tembok di depan player!");
            }
        }
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Toggle cursor lock dengan Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ?
                            CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }
}
