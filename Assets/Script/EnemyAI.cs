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
    float rotSpeed = 20.0f;

    float visibleRange = 30.0f;
    float shotRange = 10.0f;
    float visibleAngle = 20.0f;

    float hearRunRange = 30.0f;
    float hearWalkRange = 10.0f;
    float hearSneakRange = 3.0f;

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    public bool seePlayer = false;
    public bool hearPlayer = false;

    bool canFire = false;
    float recharge = 0;
    float reloadTime = 1.0f;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        agent.updatePosition = false;
        anim = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        Animations();

        if (!canFire)
        {
            recharge -= Time.deltaTime;
            if (recharge <= 0)
            {
                canFire = true;
            }
        }
    }

    private void Animations()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        anim.SetBool("isMoving", shouldMove);
        anim.SetFloat("velocityX", velocity.x);
        anim.SetFloat("velocityY", velocity.y);

        transform.position = agent.nextPosition;
    }

    [Task]
    void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(0, 30), 0, Random.Range(0, 30));
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.SetDestination(dest);
        }
        Task.current.Succeed();
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
        if (canFire)
        { 
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 500f);
        recharge = reloadTime;
        canFire = false;
        }
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
            if (hit.collider.gameObject.tag == "Props" || hit.collider.gameObject.tag == "EndArea")
            {
                seeProps = true;
            }
        }

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("Props = {0}", seeProps);

        if (distance.magnitude < visibleRange && !seeProps && Vector3.Angle(transform.forward, distance) < visibleAngle)
        {
            seePlayer = true;
            return true;
        }
        else
        {
            seePlayer = false;
            return false;
        }

    }

    [Task]
    bool HearPlayer()
    {
        Vector3 distance = player.transform.position - transform.position;
        PlayerController playerCon = player.GetComponent<PlayerController>();

        if (playerCon.isRunning && distance.magnitude < hearRunRange)
        {
            hearPlayer = true;
            return true;
        }
        else if (playerCon.isCrouched && distance.magnitude < hearSneakRange)
        {
            hearPlayer = true;
            return true;
        }
        else if (playerCon.isMoving && distance.magnitude < hearWalkRange)
        {
            hearPlayer = true;
            return true;
        }
        else
        {
            hearPlayer = false;
            return false;
        }

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

        if (distance.magnitude < shotRange && Vector3.Angle(transform.forward, distance) < 3.0f)
            return true;
        else return false;
    }

}
