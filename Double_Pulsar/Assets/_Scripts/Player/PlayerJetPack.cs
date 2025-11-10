using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJetPack : MonoBehaviour
{
    public static PlayerJetPack Instance;

    [Header("Jetpack Settings")]
    public float thrustForce = 8f;
    public float rotationSpeed = 180f; // degrees per second
    public TrailRenderer jetpackTrail;
    public float drag = 3f; // Speed cap

    private Rigidbody2D rb2D;
    private Vector2 moveInput;
    [HideInInspector] public bool isJetPacking = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (jetpackTrail == null)
            jetpackTrail = GetComponentInChildren<TrailRenderer>();

        if (jetpackTrail != null)
            jetpackTrail.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!isJetPacking) return;

        HandleRotation();
        HandleThrust();
    }

    private void HandleRotation()
    {
        float rotationInput = -moveInput.x; // right arrow -> negative rotation

        if (Mathf.Abs(rotationInput) > 0.01f)
        {
            float newRotation = rb2D.rotation + (rotationInput * rotationSpeed * Time.fixedDeltaTime);
            rb2D.MoveRotation(newRotation);
        }
    }
    private void HandleThrust()
    {
        if (isJetPacking && moveInput.y > 0.1f)
        {
            //rb2D.AddForce(transform.up * thrustForce, ForceMode2D.Force);

            rb2D.AddForce(transform.up * thrustForce * moveInput.y);
            rb2D.AddForce(-rb2D.linearVelocity * drag / 100);
        }
    }

    // Input System: Toggle Jetpack on/off
    public void OnToggleJetPack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isJetPacking = !isJetPacking;
            if(isJetPacking)
            {
                jetpackTrail.gameObject.SetActive(true);
            }
            else
            {
                jetpackTrail.gameObject.SetActive(false);
            }

            Debug.Log($"Jetpack: {(isJetPacking ? "Activated" : "Deactivated")}");
        }
    }

    // Input System: Movement (x = rotation, y = thrust)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
}
