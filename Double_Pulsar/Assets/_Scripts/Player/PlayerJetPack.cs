using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJetPack : MonoBehaviour
{
    public Rigidbody2D rb2D;

    [Header("JetPack Settings")]
    public float jetpackForce = 5f;
    public float rotationSpeed = 5f;
    public float maxTiltAngle = 30f; // how much to tilt left/right

    private bool isJetPacking = false;
    private Vector2 moveInput;

    private void Awake()
    {
        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        if(!isJetPacking)
            return;

        // Apply force based on full input (vertical + horizontal)
        if (moveInput != Vector2.zero)
        {
            rb2D.AddForce(moveInput.normalized * jetpackForce, ForceMode2D.Force);
        }

        // Handle rotation based on horizontal input
        float targetAngle = 0f;

        if (moveInput.x > 0.01f)
        {
            targetAngle = -maxTiltAngle; // Tilt right
        }
        else if (moveInput.x < -0.01f)
        {
            targetAngle = maxTiltAngle; // Tilt left
        }

        // Smoothly rotate towards target angle
        rb2D.MoveRotation(Mathf.LerpAngle(rb2D.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime));
    }

    // Calling input system
    public void OnToggleJetPack(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            isJetPacking = !isJetPacking;
            Debug.Log("Jetpack: " + (isJetPacking ? "Activated" : "Deactivated"));
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
}
