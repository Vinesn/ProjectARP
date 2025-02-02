using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    PlayerController controller;
    public LayerMask enemyLayer;
    public Transform attackPosition;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackDamage = 20f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float attackCooldown;
    float attackCooldownTimer;

    Coroutine attAnim;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        attackCooldown = attackSpeed * 1.3f;
    }

    private void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }


    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && attackCooldownTimer <= 0)
        {
            PerformAttack();
        }
    }

    void PerformAttack()   
    {
        attackPosition.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        controller.ChangeState(PlayerController.playerState.Attacking);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponentInParent<EnemyController>().TakeDamage(attackDamage);
        }

        if (attAnim != null)
        {
            StopCoroutine(attAnim);
        }
        attAnim = StartCoroutine(AttackAnimation());
        attackCooldownTimer = attackCooldown; 
    }

    IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(attackSpeed);
        attackPosition.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        controller.ChangeState(PlayerController.playerState.Idle);
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
