using UnityEngine;
using Pathfinding;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    EnemyController enemyController;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void Update()
    {
        if (enemyController.currentEnemyState == EnemyController.enemyState.Idle)
        {
            animator.SetTrigger("isIdle");
        }

        if (enemyController.currentEnemyState == EnemyController.enemyState.Patrol)
        {
            animator.SetTrigger("isMoving");
        }
        if (enemyController.currentEnemyState == EnemyController.enemyState.Attack)
        {
            animator.SetTrigger("isAttacking");
        }
        if (enemyController.currentEnemyState == EnemyController.enemyState.Hurt)
        {
            animator.SetTrigger("isHurt");
        }
        if (enemyController.currentEnemyState == EnemyController.enemyState.Chase)
        {
            animator.SetTrigger("isChase");
        }
    }
}
