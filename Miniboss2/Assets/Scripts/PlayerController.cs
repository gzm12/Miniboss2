using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Canvas menuCanvas;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Transform cameraTransform;
    private float xRotation = 0f;
    private Animation legacyAnimation;
    private bool menuOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        legacyAnimation = GetComponent<Animation>();
        cameraTransform = GetComponentInChildren<Camera>()?.transform;
        // Cursor'u baþlangýçta kilitlenmek üzere ayarla
        UnlockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        // ESC tuþu basýldýðýnda cursor kilit durumunu deðiþtir
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (!menuOpen)
        {
            HandleInput();
            HandleMouseLook();
            UpdateAnimation();
        }
    }

    void FixedUpdate()
    {
        if (!menuOpen)
        {
            ApplyMovement();
        }
    }

    private void ToggleMenu()
    {
        menuOpen = !menuOpen;

        if (menuOpen)
        {
            UnlockCursor();
            if (menuCanvas != null)
                menuCanvas.gameObject.SetActive(true);
        }
        else
        {
            LockCursor();
            if (menuCanvas != null)
                menuCanvas.gameObject.SetActive(false);
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void HandleInput()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1f;
            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;
            if (Keyboard.current.aKey.isPressed)
                input.x -= 1f;
            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;
        }
        moveDirection = new Vector3(input.x, 0f, input.y).normalized;
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null || cameraTransform == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        // Yatay rotasyon (oyuncu)
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
        
        // Dikey rotasyon (kamera)
        xRotation -= mouseDelta.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void UpdateAnimation()
    {
        if (legacyAnimation != null)
        {
            bool isMoving = moveDirection.magnitude > 0;
            
            if (isMoving)
            {
                legacyAnimation.CrossFade("walk", 0.1f);
            }
            else
            {
                legacyAnimation.CrossFade("idle", 0.1f);
            }
        }
    }

    private void ApplyMovement()
    {
        if (rb != null)
        {
            Vector3 movement = Vector3.zero;
            
            if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
            {
                // Mouse'un baktýðý yöne git (kameranýn forward yönü)
                movement = cameraTransform != null ? cameraTransform.forward : transform.forward;
            }
            else
            {
                // Normal WASD hareketi
                movement = moveDirection;
                movement = transform.TransformDirection(movement);
            }
            
            movement.y = 0f; // Y eksenini sýfýrla (sadece yatay hareket)
            rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
        }
    }
}
