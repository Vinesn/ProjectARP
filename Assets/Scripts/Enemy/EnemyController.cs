using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] public enemyState currentEnemyState = enemyState.Idle;

    [Header("Stats")]
    public float maxHealth = 100;
    public float moveSpeed = 2f;
    public float currentHealth;

    SpriteRenderer spriteController;
    EnemyAI enemyAI;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteController = GetComponentInChildren<SpriteRenderer>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(faintAndDie());
    }

    IEnumerator faintAndDie()
    {
        spriteController.flipY = true;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
    public enum enemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hurt
    }
    public void ChangeEnemyState(enemyState newState)
    {
        currentEnemyState = newState;
    }
}
