using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    // Attributes will help organize the Inspector
    [Header("Player Settings")]
    [Space(2)]
    [Tooltip("Speed value between 1 and 6.")]
    [Range(1.0f, 6.0f)]
    public float speed;
    public float jumpSpeed;
    public float rotationSpeed;
    public float gravity;
    public int health;

    Vector3 moveDirection;

    enum ControllerType { SimpleMove, Move };
    [SerializeField] ControllerType type;

    [Header("Weapon Settings")]
    // Handle weapon shooting
    public float projectileForce;
    public Rigidbody projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Raycast Settings")]
    public Transform thingToLookFrom;
    public float lookDistance;

    bool debugLog = false;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            controller.minMoveDistance = 0.0f;

            animator.applyRootMotion = false;

            name = "Character";

            if (speed <= 0)
            {
                speed = 6.0f;
                if (debugLog)
                    Debug.Log("Speed not set on " + name + " defaulting to " + speed);
            }

            if (jumpSpeed <= 0)
            {
                jumpSpeed = 6.0f;
                if (debugLog)
                    Debug.Log("JumpSpeed not set on " + name + " defaulting to " + jumpSpeed);
            }

            if (rotationSpeed <= 0)
            {
                rotationSpeed = 10.0f;
                if (debugLog)
                    Debug.Log("RotationSpeed not set on " + name + " defaulting to " + rotationSpeed);
            }

            if (gravity <= 0)
            {
                gravity = 9.81f;
                if (debugLog)
                    Debug.Log("Gravity not set on " + name + " defaulting to " + gravity);
            }

            moveDirection = Vector3.zero;

            if (projectileForce <= 0)
            {
                projectileForce = 40.0f;
                if (debugLog)
                    Debug.Log("ProjectileForce not set on " + name + " defaulting to " + projectileForce);
            }

            if (!projectilePrefab)
                Debug.LogWarning("Missing projectilePrefab on " + name);

            if (!projectileSpawnPoint)
                Debug.LogWarning("Missing projectileSpawnPoint on " + name);

            if (lookDistance <= 0)
            {
                lookDistance = 10.0f;
                if (debugLog)
                    Debug.Log("LookDistance not set on " + name + " defaulting to " + lookDistance);
            }

            if (health <= 0)
            {
                health = 100;
                if (debugLog)
                    Debug.Log("Health not set on " + name + " defaulting to " + health);
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        catch (UnassignedReferenceException e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            Debug.LogWarning("Always get called");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case ControllerType.SimpleMove:

                //transform.Rotate(0, Input.GetAxis("Horizontal") * rotationSpeed, 0);

                controller.SimpleMove(transform.forward * Input.GetAxis("Vertical") * speed);

                break;

            case ControllerType.Move:

                if (controller.isGrounded)
                {
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                    moveDirection *= speed;

                    moveDirection = transform.TransformDirection(moveDirection);

                    if (Input.GetButtonDown("Jump"))
                        moveDirection.y = jumpSpeed;
                }

                moveDirection.y -= gravity * Time.deltaTime;

                controller.Move(moveDirection * Time.deltaTime);

                break;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");

            fire();
        }

        // Usage Raycast
        // - GameObject needs a Collider
        RaycastHit hit;

        if (thingToLookFrom)
        {
            Debug.DrawRay(thingToLookFrom.transform.position, thingToLookFrom.transform.forward * lookDistance, Color.red);

            if (Physics.Raycast(thingToLookFrom.transform.position, thingToLookFrom.transform.forward, out hit, lookDistance))
            {
                if (debugLog)
                    Debug.Log(name + ": Raycast hit - " + hit.transform.name);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * lookDistance, Color.blue);

            if (Physics.Raycast(transform.position, transform.forward, out hit, lookDistance))
            {
                if (debugLog)
                    Debug.Log(name + ": Raycast hit - " + hit.transform.name);
            }
        }

        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("Speed", transform.InverseTransformDirection(controller.velocity).z);
    }

    public void fire()
    {
        Debug.Log("Pew Pew");

        //animator.SetTrigger("Attack");

        if (projectileSpawnPoint && projectilePrefab)
        {
            // Make projectile
            Rigidbody temp = Instantiate(projectilePrefab, projectileSpawnPoint.position,
                projectileSpawnPoint.rotation);

            // Shoot projectile
            temp.AddForce(projectileSpawnPoint.forward * projectileForce, ForceMode.Impulse);

            // Destroy projectile after 2.0 seconds
            Destroy(temp.gameObject, 2.0f);
        }
    }

    // Collision Usage Rules:
    // - Both GameObjects need colliders
    // - One or both of the GameObjects need a Rigidbody
    // - Can set one of the Rigidbody components to "Is Kinematic"
    // - Called once on first collision between two GameObjects
    void OnCollisionEnter(Collision collision)
    {
        if (debugLog)
            Debug.Log(name + ": OnCollisionEnter - " + collision.gameObject.name);
    }

    // - Called as long as there is collision between two GameObjects
    void OnCollisionStay(Collision collision)
    {
        if (debugLog)
            Debug.Log(name + ": OnCollisionStay - " + collision.gameObject.name);
    }

    // - Called once on collision stopping between two GameObjects
    void OnCollisionExit(Collision collision)
    {
        if (debugLog)
            Debug.Log(name + ": OnCollisionExit - " + collision.gameObject.name);
    }

    // Collision Usage Rules:
    // - GameObject needs a CharacterController component
    // - One of the GameObjects need a Collider
    // - Behaves like OnCollisionStay
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (debugLog)
            Debug.Log(name + ": OnControllerColliderHit - " + hit.gameObject.name);
    }


    // Trigger Usage Rules:
    // - Both GameObjects need colliders
    // - Set one of the Collider components to "Is Trigger"
    // - Called once on first overlap
    void OnTriggerEnter(Collider other)
    {
        if (debugLog)
            Debug.Log(name + ": OnTriggerEnter - " + other.gameObject.name);
    }

    // - Called as long as there is overlap between two GameObjects
    void OnTriggerStay(Collider other)
    {
        if (debugLog)
            Debug.Log(name + ": OnTriggerStay - " + other.gameObject.name);
    }

    // - Called once on first overlap
    void OnTriggerExit(Collider other)
    {
        if (debugLog)
            Debug.Log(name + ": OnTriggerExit - " + other.gameObject.name);
    }

    // Adds a menu option to reset stats
    [ContextMenu("Reset Stats")]
    void ResetStats()
    {
        if (debugLog)
            Debug.Log("Perform operation");

        speed = 6.0f;
    }

    public void ChangeHealth(int damage)
    {
        health += damage;
    }
}