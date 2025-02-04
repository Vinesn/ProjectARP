using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //Zrobiæ Mechanike Patrolu (WaitforNextPatrol, RandomPatrolPoint), Ogarn¹æ State Machine
    [Header("Pathfinding")]
    public Transform target;
    public LayerMask playerLayer;
    public float pathUpdateSeconds = 0.5f;
    float activeDistance;
    Collider2D closestTarget;

    [Header("Physics")]
    public float nextWaypointDistance = 3f;
    float moveSpeed;

    public SpriteRenderer enemyGFX;
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
        activeDistance = controller.sightRadius;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        //move Speed * 50 ¿eby prêdkoœci by³y w tej samej skali co gracza.
        moveSpeed = (controller.moveSpeed * 50);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance())
        {
            PathFollow();
        } else
        {
            controller.ChangeEnemyState(EnemyController.enemyState.Idle);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }


    }

    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(transform.position, closestTarget.transform.position, OnPathComplete);
        }
    }

    void PathFollow()
    {
        //Checki
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //Animation
        if (controller.currentEnemyState != EnemyController.enemyState.Chase)
        {
            controller.ChangeEnemyState(EnemyController.enemyState.Chase);
        }

        float distanceToWaypoint;
        while (true)
        {
            distanceToWaypoint = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        //Move
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * moveSpeed * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(force.x, force.y);

        //GFX Flip
            if (direction.x >= 0.05f)
        {
            enemyGFX.flipX = false;
        }
        else if (direction.x <= 0.05f)
        {
            enemyGFX.flipX = true;
        }
    }

    bool TargetInDistance()
    {
        Collider2D[] closeTargets = Physics2D.OverlapCircleAll(rb.position, activeDistance, playerLayer);
        closestTarget = null;
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
            currentWaypoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activeDistance);
    }
}
