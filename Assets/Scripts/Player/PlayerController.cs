using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public playerState currentState;
    PlayerMovement playerMoveScript;

    private void Awake()
    {
        playerMoveScript = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        ChangeState(playerState.Idle);
    }

    public void ChangeState(playerState newState)
    {
        currentState = newState;
    }

    public enum playerState
    {
        Idle,
        Walking,
        Running,
        Attacking
    }

    public Vector2 ReturnMoveInput()
    {
        return playerMoveScript.moveInput;
    }

    public Transform ReturnAttackPosition()
    {
        return playerMoveScript.attPos;
    }
}
