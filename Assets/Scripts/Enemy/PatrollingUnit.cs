using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingUnit : MonoBehaviour
{
    public Transform[] points;
    public Transform[] lookDir;

    public Transform eyePivot;
    public Transform emiter;

    public float reactionTime = 1f;
    public float WaitAtPointInterval = 2f;
    public float attackDistance = 5;
    public float FOV = 45;
    public float hitRate = 1;

    public int startIndex = 0;
    public int damageAmount = 1;

    public AnimationCurve curve;

    int destPoint;

    NavMeshAgent agent;

    enum state { Patroling, Chasing, Searching };

    state currentState = state.Patroling;
    state lastFrameState;

    float distance;
    float seeDistance = 10f;
    float eyesONPlayerTime = 0;
    float agentSpeed;

    bool waitingAtPoint = false;
    bool canHit = true;

    IEnumerator wait;

    void Start()
    {
        wait = waitAtPatrolPoint();
        agent = GetComponent<NavMeshAgent>();
        destPoint = startIndex;
        agent.autoBraking = false;
        lastFrameState = currentState;
        GotoNextPoint();
        agentSpeed = agent.speed;
    }

    void attackPlayer(int damageAmount)
    {
        if (canHit)
        {
            StartCoroutine(hitCoolDown());
            PlayerManager.PlayerHurt(damageAmount);
        }
    }

    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;
        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;

        currentState = state.Patroling;
    }

    void PatrollingFunct()
    {
        if (CanSeePlayer())
        {
            eyesONPlayerTime += Time.deltaTime;
            if (eyesONPlayerTime > (reactionTime * (1 - (seeDistance / distance))) && distance < seeDistance)
            {
                currentState = state.Chasing;
                eyesONPlayerTime = 0;
                StopCoroutine(wait);
                eyePivot.rotation = lookDir[0].rotation;
                waitingAtPoint = false;
                return;
            }
        }
        else
        {
            eyesONPlayerTime = 0f;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = state.Searching;
        }
    }

    void ChasingFunct()
    {
        agent.speed = agentSpeed;
        if (distance > seeDistance)
        {
            currentState = state.Searching;
            eyesONPlayerTime = 0f;
        }
        else
        {
            agent.destination = PlayerManager.playerPosition();
        }
    }

    void SearchingFunct()
    {
        if (!waitingAtPoint)
        {
            wait = waitAtPatrolPoint();
            StartCoroutine(wait);
        }
        if (CanSeePlayer())
        {
            eyesONPlayerTime += Time.deltaTime;
            if (eyesONPlayerTime > reactionTime && distance < seeDistance)
            {
                currentState = state.Chasing;
                eyesONPlayerTime = 0;
                StopCoroutine(wait);
                wait = null;
                eyePivot.rotation = lookDir[0].rotation;
                waitingAtPoint = false;
                return;
            }
        }
        else
        {
            eyesONPlayerTime = 0f;
        }
    }

    void Update()
    {
        distance = PlayerManager.playerDistance(transform);

        if (distance > attackDistance)
        {
            switch (currentState)
            {
                case state.Patroling:
                    PatrollingFunct();
                    break;
                case state.Chasing:
                    ChasingFunct();
                    break;
                case state.Searching:
                    SearchingFunct();
                    break;
                default:
                    PatrollingFunct();
                    break;
            }
        }
        else
        {
            agent.destination = transform.position;
            attackPlayer(damageAmount);
        }

        lastFrameState = currentState;
    }

    IEnumerator waitAtPatrolPoint()
    {
        waitingAtPoint = true;

        agentSpeed = agent.speed;
        agent.speed = 0;

        float timer = 0;
        while (timer < WaitAtPointInterval * 0.5f)
        {
            eyePivot.rotation = Quaternion.Lerp(eyePivot.rotation, lookDir[1].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(WaitAtPointInterval * 0.1f);

        timer = 0;
        while (timer < WaitAtPointInterval * 0.5f)
        {
            eyePivot.rotation = Quaternion.Lerp(eyePivot.rotation, lookDir[2].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(WaitAtPointInterval * 0.1f);
        timer = 0;
        while (timer < WaitAtPointInterval * 0.5f)
        {
            eyePivot.rotation = Quaternion.Lerp(eyePivot.rotation, lookDir[0].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(WaitAtPointInterval * 0.1f);

        agent.speed = agentSpeed;
        GotoNextPoint();
        waitingAtPoint = false;
    }

    IEnumerator hitCoolDown()
    {
        canHit = false;
        yield return new WaitForSeconds(hitRate);
        canHit = true;
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;

        Vector3 rayDirection = PlayerManager.playerPosition() - emiter.position;

        Vector3 endVector = Quaternion.AngleAxis(FOV, Vector3.up) * emiter.forward * 10;
        Vector3 endVector2 = Quaternion.AngleAxis(-FOV, Vector3.up) * emiter.forward * 10;

        float angle = Vector3.Angle(rayDirection, emiter.forward);

        if (angle < FOV)
        {
            if (Physics.Raycast(emiter.position, rayDirection, out hit, 30f))
            {
                if (hit.transform.tag == "Player")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
