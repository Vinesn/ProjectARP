using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerController controller;
    Rigidbody2D rb;
    [SerializeField] float movementSpeed = 5f;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Transform attPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attPos = GetComponent<PlayerCombat>().attackPosition;
        controller = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (controller.currentState != PlayerController.playerState.Attacking)
        {
            controller.currentState = PlayerController.playerState.Running;

            rb.linearVelocity = moveInput * movementSpeed;
            if (moveInput != Vector2.zero && (moveInput.x == 0 || moveInput.y == 0))
            {
                attPos.position = moveInput.normalized + new Vector2(transform.position.x, transform.position.y);
            }

        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (moveInput == Vector2.zero && controller.currentState != PlayerController.playerState.Attacking)
        {
            controller.ChangeState(PlayerController.playerState.Idle);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
