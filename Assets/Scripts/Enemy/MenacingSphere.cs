using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenacingSphere : MonoBehaviour
{
    [SerializeField]
    Vector3 boosterUp;

    [SerializeField]
    float attackDistance = 10f;

    [SerializeField]
    Transform MSphereBody;

    [SerializeField]
    Transform MSphereEngine;

    [SerializeField]
    Transform MSphereLReciever;

    [SerializeField]
    Transform MSphereLSlide;

    [SerializeField]
    Transform MSphereLShootPoint;

    [SerializeField]
    Transform MSphereLSlideTarget;

    [SerializeField]
    Transform MSphereRReciever;

    [SerializeField]
    Transform MSphereRSlide;

    [SerializeField]
    Transform MSphereRShootPoint;

    [SerializeField]
    Transform MSphereRSlideTarget;

    [SerializeField]
    Transform LGunLookAt;

    [SerializeField]
    Transform RGunLookAt;

    [SerializeField]
    Transform lGunPivot;

    [SerializeField]
    Transform rGunPivot;

    [SerializeField]
    Rigidbody BulletPrefab;

    [SerializeField]
    float ShootSpeed = 0.5f;

    [SerializeField]
    AnimationCurve aniCurve;

    float ShootPower = 25f;
    float rotateSpeed = 5f;
    float wobbleSpeed = 2f;
    float timeElapsed = 0f;

    NavMeshAgent agent;

    bool canShoot = true;
    bool lShoot = true;
    bool lCocking = false;
    bool rCocking = false;
    bool movingUp = false;

    Vector3 lSlideStart;
    Vector3 rSlideStart;
    Vector3 lastFrameLocation;
    Vector3 shootRotate = new Vector3(20, 0, 0);
    Vector3 LshootTarget = Vector3.zero;
    Vector3 RshootTarget = Vector3.zero;


    float targetHeight;
    float floatStartHeight;
    float moveSpeed = 0;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        lSlideStart = MSphereLReciever.InverseTransformPoint(MSphereLSlide.position);
        rSlideStart = MSphereRReciever.InverseTransformPoint(MSphereRSlide.position);
        StartCoroutine(FloatWobble());
        lastFrameLocation = transform.position;
    }
    void GunRotate()
    {
        Vector3 playerVerticalDirection = PlayerManager.cameraPosition() - MSphereBody.position;

        float targetRotateAngle = Vector3.Angle(playerVerticalDirection, MSphereBody.forward);
        float rotateAngle = Mathf.Lerp(targetRotateAngle, lGunPivot.localEulerAngles.x, Time.deltaTime * 0.1f);

        rotateAngle = PlayerManager.cameraPosition().y > MSphereBody.position.y ? rotateAngle : -rotateAngle;

        lGunPivot.localEulerAngles = new Vector3(-rotateAngle, 0, 0);
        rGunPivot.localEulerAngles = new Vector3(-rotateAngle, 0, 0);

        Quaternion lTargetRotation = Quaternion.LookRotation((LGunLookAt.position - MSphereLReciever.position).normalized);
        Quaternion rTargetRotation = Quaternion.LookRotation((RGunLookAt.position - MSphereRReciever.position).normalized);

        MSphereLReciever.rotation = Quaternion.Lerp(MSphereLReciever.rotation, lTargetRotation, Time.deltaTime * 10f);
        MSphereRReciever.rotation = Quaternion.Lerp(MSphereRReciever.rotation, rTargetRotation, Time.deltaTime * 10f);
    }


    void updateOffset()
    {
        if (timeElapsed < wobbleSpeed)
        {
            agent.baseOffset = Mathf.Lerp(floatStartHeight, targetHeight, aniCurve.Evaluate(timeElapsed / wobbleSpeed));
        }
        else
        {
            agent.baseOffset = targetHeight;
        }

        timeElapsed += Time.deltaTime;
        agent.height = agent.baseOffset + 0.45f;
    }

    IEnumerator FloatWobble()
    {
        while(true)
        {
            wobbleSpeed = Random.Range(1.8f, 2.2f);
            timeElapsed = 0f;
            floatStartHeight = agent.baseOffset;
            if (movingUp)
            {
                movingUp = false;
                targetHeight = Random.Range(1.3f, 1.4f);
            }
            else
            {
                movingUp = true;
                targetHeight = Random.Range(1.1f, 1.2f);
            }
            yield return new WaitForSeconds(wobbleSpeed);
        }

    }

    void RotateThruster()
    {
        moveSpeed = Mathf.Round(Vector2.Distance(new Vector2(lastFrameLocation.x, lastFrameLocation.z), new Vector2(transform.position.x, transform.position.z)) * 1500f);
        MSphereEngine.localEulerAngles = Vector3.Lerp(new Vector3(moveSpeed, 0, 0), MSphereEngine.localEulerAngles, Time.deltaTime * 0.1f);
    }

    void setLastFrame()
    {
        lastFrameLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        updateOffset();
        RotateThruster();
        setLastFrame();
        GunRotate();

        if (PlayerManager.playerDistance(transform) > attackDistance)
        {
            agent.updateRotation = true;
            agent.destination = PlayerManager.playerPosition();
        }
        else
        {
            agent.destination = transform.position;
            agent.updateRotation = false;
            Vector3 direction = (PlayerManager.playerPosition() - transform.position).normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, Time.deltaTime * rotateSpeed);

            Shoot();
        }
        WeaponCocking();
    }

    void WeaponCocking()
    {
        if (lCocking)
        {
            if (Vector3.Distance(MSphereLSlide.position, MSphereLSlideTarget.position) < 0.05f)
            {
                MSphereLSlideTarget.position = MSphereLReciever.TransformPoint(lSlideStart);
                LshootTarget = new Vector3(0, 0, 0);
                LGunLookAt.localPosition = new Vector3(0, 0, 1);
                lCocking = false;
            }
        }

        if (rCocking)
        {
            if (Vector3.Distance(MSphereRSlide.position, MSphereRSlideTarget.position) < 0.05f)
            {
                MSphereRSlideTarget.position = MSphereRReciever.TransformPoint(rSlideStart);
                RshootTarget = Vector3.zero;
                RGunLookAt.localPosition = new Vector3(0, 0, 1);
                rCocking = false;
            }
        }

        MSphereLSlide.position = Vector3.Lerp(MSphereLSlide.position, MSphereLSlideTarget.position, Time.deltaTime * 10f);
        MSphereRSlide.position = Vector3.Lerp(MSphereRSlide.position, MSphereRSlideTarget.position, Time.deltaTime * 10f);
        //MSphereLReciever.localEulerAngles = Vector3.Lerp(MSphereLReciever.localEulerAngles, LshootTarget, Time.deltaTime * 10f);
        //MSphereRReciever.localEulerAngles = Vector3.Lerp(MSphereRReciever.localEulerAngles, RshootTarget, Time.deltaTime * 10f);
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;

        Vector3 rGunVectorToPlayer = (PlayerManager.playerPosition() + Vector3.up) - (MSphereRShootPoint.position + MSphereRShootPoint.forward);
        Vector3 lGunVectorToPlayer = (PlayerManager.playerPosition() + Vector3.up) - (MSphereLShootPoint.position + MSphereLShootPoint.forward);

        if (Physics.Raycast(MSphereRShootPoint.position, rGunVectorToPlayer, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag != "Player")
            {
                return false;
            }
        }
        if (Physics.Raycast(MSphereLShootPoint.position, lGunVectorToPlayer, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag != "Player")
            {
                return false;
            }
        }

        return true;
    }

    float CheckPlayerAngle(Transform startPoint)
    {
        Vector3 playerPositionXY = PlayerManager.cameraPosition();
        Vector3 thisPositionXY = startPoint.position;
        playerPositionXY.y = 0;
        thisPositionXY.y = 0;
        float angle = 180 - Vector3.Angle(thisPositionXY - playerPositionXY, startPoint.forward);

        if(!CanSeePlayer())
        {
            angle = 99999f;
        }

        Debug.Log(angle);

        return angle;
    }

    void Shoot()
    {
        if (canShoot)
        {
            if (CheckPlayerAngle(transform) < 7f)
            {
                StartCoroutine(ShootCoolDown());
                if (lShoot)
                {
                    lShoot = false;
                    lCocking = true;
                    LGunLookAt.localPosition = new Vector3(0, 0.5f, 1);
                    LshootTarget = shootRotate;
                    ShootBullet(MSphereLShootPoint);
                    MSphereLSlideTarget.localPosition = new Vector3(MSphereLSlideTarget.localPosition.x, MSphereLSlideTarget.localPosition.y, MSphereLSlideTarget.localPosition.z - 0.3f);
                }
                else
                {
                    lShoot = true;
                    rCocking = true;
                    RGunLookAt.localPosition = new Vector3(0, 0.5f, 1);
                    RshootTarget = shootRotate;
                    ShootBullet(MSphereRShootPoint);
                    MSphereRSlideTarget.localPosition = new Vector3(MSphereRSlideTarget.localPosition.x, MSphereRSlideTarget.localPosition.y, MSphereRSlideTarget.localPosition.z - 0.3f);
                }
            }
        }
    }

    void ShootBullet(Transform shootPoint)
    {
        Rigidbody newBullet = Instantiate(BulletPrefab, shootPoint.position, shootPoint.rotation);
        Vector3 targetVector = Vector3.Normalize((PlayerManager.cameraPosition() + ((Vector3.up * 0.375f) * (PlayerManager.cameraDistance(shootPoint) / attackDistance))) - shootPoint.position);
        newBullet.AddForce(targetVector * ShootPower, ForceMode.Impulse);
    }

    IEnumerator ShootCoolDown()
    {
        canShoot = false;
        yield return new WaitForSeconds(ShootSpeed);
        canShoot = true;
    }
}
