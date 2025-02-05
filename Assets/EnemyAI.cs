using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //Zrobic Raycast zeby przeciwnik nie updatowal patha przy sciganiu gracza za przeszkodami, Nie tworzyc drogi do miejsc niedostępnych
    [Header("Pathfinding")]
    public Transform target;
    public LayerMask playerLayer;
    public float pathUpdateSeconds = 0.5f;
    float activeDistance;
    Collider2D closestTarget;
    bool reachedEndOfPath = true;

    [Header("Patrol")]
    [SerializeField] float patrolRadius = 5f;
    [Range(0f, 1f)][SerializeField] float patrolSpeedNerf = 0.7f;
    [SerializeField] float patrolCooldown = 3f;
    [SerializeField] float patrolTimer;
    bool hasPatrolPoint = false;

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
        patrolTimer = patrolCooldown;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        Debug.Log(moveSpeed);
        //Move Speed
        if (controller.currentEnemyState == EnemyController.enemyState.Patrol)
        {
            moveSpeed = ((controller.moveSpeed * 50) * patrolSpeedNerf);
        } else
        {
            moveSpeed = (controller.moveSpeed * 50);
        }

        //Update zasięg ścigania
        if (activeDistance != controller.sightRadius)
        {
            activeDistance = controller.sightRadius;
        }

        if (patrolTimer > 0 && controller.currentEnemyState == EnemyController.enemyState.Idle)
        {
            patrolTimer -= Time.deltaTime;
        }
        
    }

    private void FixedUpdate()
    {
        if (TargetInDistance())
        {
            //Animation
            if (controller.currentEnemyState != EnemyController.enemyState.Chase)
            {
                controller.ChangeEnemyState(EnemyController.enemyState.Chase);
            }

            PathFollow();
        }

        if (!reachedEndOfPath)
        {
            PathFollow();
        } else
        {
            if (controller.currentEnemyState != EnemyController.enemyState.Idle)
            {
                controller.ChangeEnemyState(EnemyController.enemyState.Idle);
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                hasPatrolPoint = false;
                patrolTimer = patrolCooldown;
            }
        }

        if (patrolTimer <= 0 && controller.currentEnemyState == EnemyController.enemyState.Idle)
        {
            controller.ChangeEnemyState(EnemyController.enemyState.Patrol);
            PathFollow();
        }
    }

    void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(transform.position, closestTarget.transform.position, OnPathComplete);
        }

        if (!hasPatrolPoint && !TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(transform.position, PickRandomPoint(), OnPathComplete);
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

        reachedEndOfPath = false;

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
                    reachedEndOfPath = true;
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
    Vector2 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * patrolRadius;
        point += transform.position;

        hasPatrolPoint = true;
        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activeDistance);
    }
}