using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform attPos;
    Rigidbody2D rb;
    Vector2 moveInput;
    [SerializeField] float movementSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attPos = GetComponent<PlayerCombat>().attackPosition;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * movementSpeed;
        if (moveInput != Vector2.zero)
        {
            attPos.position = moveInput.normalized + new Vector2(transform.position.x, transform.position.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
