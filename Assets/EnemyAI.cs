using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //Zrobiæ Mechanike Patrolu (WaitforNextPatrol, RandomPatrolPoint), Ogarn¹æ State Machine

    [Header("Pathfinding")]
    public Transform target;
    public LayerMask playerLayer;
    public float activeDistance = 3f;
    public float pathUpdateSeconds = 0.5f;
    Collider2D closestTarget;

    [Header("Physics")]
    public float nextWaypointDistance = 3f;
    float moveSpeed;

    public Transform enemyGFX;
    Path path;
    int currentWaypoint = 0;
    Seeker seeker;
    EnemyController controller;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyController>();
        seeker = GetComponent<Seeker>();
    }

    private void Start()
    {
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        moveSpeed = (controller.moveSpeed * 20);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance())
        {
            controller.ChangeEnemyState(EnemyController.enemyState.Patrol);
            PathFollow();
        }

        if (rb.linearVelocity.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector2(1f, 1f);
        }
        else if (rb.linearVelocity.x <= 0.01f)
        {
            enemyGFX.localScale = new Vector2(-1f, 1f);
        } 
    }

    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * moveSpeed * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(force.x, force.y);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    bool TargetInDistance()
    {
        Collider2D[] closeTargets = Physics2D.OverlapCircleAll(rb.position, activeDistance, playerLayer);
        Collider2D closestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D target in closeTargets)
        {
            float distanceToTarget = Vector2.Distance(rb.position, target.transform.position);

            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            return true;
        }
        return false;
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activeDistance);
    }
}
