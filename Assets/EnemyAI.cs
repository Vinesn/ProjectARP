using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    //Attack, Hurt
    [Header("Pathfinding")]
    public Transform target;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public float pathUpdateSeconds = 0.5f;
    Collider2D closestTarget;
    GraphNode positionNode;
    GraphNode destinationNode;
    bool reachedEndOfPath = true;

    [Header("Chase")]
    public float sightRadius = 5f;

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
        patrolTimer = patrolCooldown;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void Update()
    {
        //Move Speed
        if (controller.currentEnemyState == EnemyController.enemyState.Patrol)
        {
            moveSpeed = ((controller.moveSpeed * 50) * patrolSpeedNerf);
        } else
        {
            moveSpeed = (controller.moveSpeed * 50);
        }

        if (patrolTimer > 0 && controller.currentEnemyState == EnemyController.enemyState.Idle)
        {
            patrolTimer -= Time.deltaTime;
        }
        
    }

    private void FixedUpdate()
    {
        if (TargetClose() && TargetVisible())
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
        if (TargetClose() && TargetVisible() && seeker.IsDone())
        {
            seeker.StartPath(transform.position, closestTarget.transform.position, OnPathComplete);
        }

        if (!hasPatrolPoint && controller.currentEnemyState != EnemyController.enemyState.Chase && seeker.IsDone())
        {
            while (true)
            {
                seeker.StartPath(transform.position, PickRandomPoint(), OnPathComplete);

                if (IsPathPossible(positionNode, destinationNode))
                {
                    break;
                }
            }
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

    bool TargetClose()
    {
        Collider2D[] closeTargets = Physics2D.OverlapCircleAll(rb.position, sightRadius, playerLayer);
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

    bool TargetVisible()
    {
        if (closestTarget != null)
        {
            Vector2 directionToTarget = (closestTarget.transform.position - transform.position).normalized;

            RaycastHit2D ray = Physics2D.Raycast(transform.position, directionToTarget, sightRadius);
            if (ray.collider != null && ray.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, directionToTarget * sightRadius, Color.green);
                return true;
            }
            Debug.DrawRay(transform.position, directionToTarget * sightRadius, Color.red);
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

        positionNode = AstarPath.active.GetNearest(transform.position).node;
        destinationNode = AstarPath.active.GetNearest(point).node;

        hasPatrolPoint = true;
        return point;
    }

    bool IsPathPossible(GraphNode node1, GraphNode node2)
    {
        return PathUtilities.IsPathPossible(node1, node2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        if (positionNode != null && destinationNode != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube((Vector3)positionNode.position, Vector3.one);
            Gizmos.DrawCube((Vector3)destinationNode.position, Vector3.one);
        }
    }
}