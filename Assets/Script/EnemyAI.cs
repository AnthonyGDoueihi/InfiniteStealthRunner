using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;

    Animator anim;
    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float rotSpeed = 5.0f;

    float visibleRange = 10.0f;
    float shotRange = 10.0f;

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        agent.updatePosition = false;
        anim = GetComponentInChildren<Animator>();        
    }

    void Update()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        // Update animation parameters
        anim.SetBool("isMoving", shouldMove);
        anim.SetFloat("velocityX", velocity.x);
        anim.SetFloat("velocityY", velocity.y);

        transform.position = agent.nextPosition;
    }

    [Task]
    void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(0, 30), 0, Random.Range(0, 30));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    void TargetPlayer()
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    void LookAtTarget()
    {
        Vector3 direction = target - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(transform.forward, direction));
        }
        if (Vector3.Angle(transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    [Task]
    bool Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }

    [Task]
    bool SeePlayer()
    {
        Vector3 distance = player.transform.position - transform.position;

        RaycastHit hit;
        bool seeProps = false;

        if (Physics.Raycast(transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "Props")
            {
                seeProps = true;
            }
        }

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("Props = {0}", seeProps);

        if (distance.magnitude < visibleRange && !seeProps)
            return true;
        else return false;
    }

    [Task]
    bool Turn(float angle)
    {
        var p = transform.position + Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        target = p;
        return true;
    }

    [Task]
    void SetTargetDestination()
    {
        agent.SetDestination(target);
        Task.current.Succeed();
    }

    [Task]
    bool ShotLinedUp()
    {
        Vector3 distance = target - transform.position;
        if (distance.magnitude < shotRange && Vector3.Angle(transform.forward, distance) < 1.0f)
            return true;
        else return false;
    }
}
