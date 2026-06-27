using UnityEngine;

namespace AssistiveARVisualGuidance
{
    // Desktop stand-in for AR-glasses movement: keyboard translation plus headset-style mouse look.
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float mouseSensitivity = 2.0f;
        [SerializeField] private float gravity = -18f;

        private CharacterController characterController;
        private float pitch;
        private float verticalVelocity;

        public void Initialize(Transform playerCamera)
        {
            cameraTransform = playerCamera;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            LookAround();
            Move();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void LookAround()
        {
            if (cameraTransform == null || Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            pitch = Mathf.Clamp(pitch - mouseY, -82f, 82f);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // CharacterController keeps movement simple and avoids rigidbody tuning for this prototype.
            Vector3 input = new Vector3(horizontal, 0f, vertical).normalized;
            Vector3 movement = transform.TransformDirection(input) * moveSpeed;

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            movement.y = verticalVelocity;

            characterController.Move(movement * Time.deltaTime);
        }
    }
}
