using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : Subject, IObserver
{
    private Transform currentTarget;
    Rigidbody rigidbody;
    Enemy[] enemyReference;
    enum EnemyState { Patrol, Chase, Attack }
    [SerializeField] EnemyState enemyState;

    [Header("Enemy Setttings")]
    public Transform player;
    public float aggroDistance;
    public float speed;
    bool isAlerted = false;

    [Header("Weapon Settings")]
    public float projectileFireRate;
    public float projectileForce;
    public Rigidbody projectilePrefab;
    public Transform projectileSpawnPoint;
    float timeSinceLastFire = 0f;
    bool isShooting = false;

    [Header("Patrol Settings")]
    public List<Transform> patrolPoints = new List<Transform>();
    public List<Transform> randomPoints = new List<Transform>();

    public Graph<Transform> patrolGraph = new Graph<Transform>();

    bool debugLog = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        int pointcount = patrolPoints.Count;
        pointcount = patrolPoints.Count;

        if (aggroDistance <= 0)
        {
            aggroDistance = 20.0f;
            if (debugLog)
                Debug.Log("Enemy AggroDistance not set on " + name + " defaulting to " + aggroDistance);
        }

        if (speed <= 0)
        {
            speed = 10.0f;
            if (debugLog)
                Debug.Log("Enemy Speed not set on " + name + " defaulting to " + speed);
        }

        if (projectileFireRate <= 0)
        {
            projectileFireRate = 3.0f;
            if (debugLog)
                Debug.Log("Enemy ProjectileFireRate not set on " + name + " defaulting to " + projectileFireRate);
        }

        if (projectileForce <= 0)
        {
            projectileForce = 30.0f;
            if (debugLog)
                Debug.Log("Enemy ProjectileForce not set on " + name + " defaulting to " + projectileForce);
        }

        for (int i = 0; i < pointcount; i++)
        {
            //PickRandomNode();
            int randNode = UnityEngine.Random.Range(0, patrolPoints.Count);
            randomPoints.Add(patrolPoints[randNode]);
            patrolPoints.Remove(patrolPoints[randNode]);
            patrolGraph.AddNode(randomPoints[i]);
        }

        for (int i = 0; i < randomPoints.Count; i++)
        {
            if (i == randomPoints.Count - 1)
            {
                patrolGraph.AddEdge(randomPoints[i], randomPoints[0]);
            }
            else
            {
                patrolGraph.AddEdge(randomPoints[i], randomPoints[i + 1]);
            }
        }

        currentTarget = patrolGraph.FindNode(randomPoints[0]).GetData();

        enemyReference = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemyReference.Length; i++)
        {
            Attach(enemyReference[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        Vector3 playerPosition = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        // points towards the player
        transform.LookAt(player.transform.position);

        // patrols the points
        if (currentTarget && !isAlerted)
            enemyState = EnemyState.Patrol;

        // has been alerted by the player
        if (isAlerted && (Vector3.Distance(transform.position, player.position) > aggroDistance))
            enemyState = EnemyState.Chase;
        else if (isAlerted && (Vector3.Distance(transform.position, player.position) <= aggroDistance))
            enemyState = EnemyState.Attack;

        if (enemyState == EnemyState.Chase)
            rigidbody.MovePosition(playerPosition);
        else if (enemyState == EnemyState.Patrol)
            rigidbody.MovePosition(position);
        else if (enemyState == EnemyState.Attack)
        {
            isShooting = true;

            if (Time.time >= timeSinceLastFire + projectileFireRate)
            {
                    timeSinceLastFire = Time.time;
                    fire();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Patrol"))
            currentTarget = patrolGraph.FindNode(other.transform).GetOutgoing()[0].GetData();

        if (other.CompareTag("PlayerProjectile"))
        {
            Notify();
            ObserverUpdate();
            Destroy(other.gameObject);
        }
    }

    public void ObserverUpdate()
    {
        isAlerted = true;
        enemyState = EnemyState.Chase;
    }

    public void fire()
    {
        Debug.Log("Incoming Attack from an Enemy!");

        if (projectileSpawnPoint && projectilePrefab)
        {
            // Make the Projectile
            Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            // Shoot Projectile
            temp.AddForce(Camera.main.transform.forward * -projectileForce, ForceMode.Impulse);

            // Destroy Projectile after 2.0s
            Destroy(temp.gameObject, 2.0f);
        }
    }
}