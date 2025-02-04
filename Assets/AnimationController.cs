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
        switch (enemyController.currentEnemyState)
        {
            case EnemyController.enemyState.Idle:
                animator.SetTrigger("isIdle");
                break;
            case EnemyController.enemyState.Patrol:
                animator.SetTrigger("isMoving");
                break;
            case EnemyController.enemyState.Attack:
                animator.SetTrigger("isAttacking");
                break;
            case EnemyController.enemyState.Hurt:
                animator.SetTrigger("isHurt");
                break;
            case EnemyController.enemyState.Chase:
                animator.SetTrigger("isChase");
                break;
            default:
                animator.SetTrigger("isIdle");
                break;
        }
    }
}
