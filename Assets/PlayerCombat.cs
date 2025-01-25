using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform attackPosition;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackDamage = 20f;

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack();
        }
    }

    void Attack()   
    {
        Debug.Log("I ATTACK!!");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hitted "+enemy.name);
            enemy.GetComponentInParent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
