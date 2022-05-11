using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [Header("Turret Pieces")]

    [SerializeField]
    Transform Base;

    [SerializeField]
    Transform Pivot;

    [SerializeField]
    Transform LowerArm;

    [SerializeField]
    Transform UpperArm;

    [SerializeField]
    Transform Gun;

    [SerializeField]
    Transform IKTarget;

    [SerializeField]
    Transform IKCurrent;

    [SerializeField]
    Rigidbody BulletPrefab;

    [SerializeField]
    Transform shootPoint;

    [Header("Turret Action Settings")]
    public float activateDistance = 10f;
    public float rotationSpeed = 1f;
    public float MovementSpeed = 1f;
    public float shootCooldown = 0.1f;
    public float shootPower = 25f;

    private Quaternion pivotStartRotation = new Quaternion();
    private Quaternion gunStartRotation = new Quaternion();

    public float playerHeightOffset = 1;

    private Vector3 targetStart;

    private bool canShoot = true;
    private bool activated = false;

    // Known Side Lengths
    float ikSideC = 0.8324879f;
    float ikSideB = 0.8324879f;

    // Declare the unknown Side
    float ikSideA;

    // Start is called before the first frame update
    void Start()
    {
        pivotStartRotation = Pivot.rotation;
        gunStartRotation = Gun.localRotation;
        targetStart = IKTarget.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInDistance())
        {
            Active();
        }
        else
        {
            InActive();
        }
        UpdateIK();
    }

    void Active()
    {
        activated = true;
        RotatePivotTowardsPlayer();
        MoveTarget();
        Shoot();
    }

    void Shoot()
    {
        Vector3 gunFoward = Gun.forward * 10;
        Vector3 gunToPlayer = PlayerManager.cameraPosition() - Gun.position;
        float gunPlayerAngle = Vector3.Angle(gunFoward, gunToPlayer);
        // Debug.DrawRay(Gun.position, gunFoward, Color.green);
        // Debug.DrawRay(Gun.position, gunToPlayer, Color.red);
        // Debug.Log(gunPlayerAngle);

        if(gunPlayerAngle < 5 && canShoot && Vector3.Distance(IKTarget.position, IKCurrent.position) < 0.05f)
        {
            StartCoroutine(GunDelay());
            Rigidbody newBullet = Instantiate(BulletPrefab, shootPoint.position, Gun.rotation);
            newBullet.AddForce(Gun.forward * shootPower, ForceMode.Impulse);
        }
    }

    IEnumerator GunDelay()
    {
        canShoot = false;
        float MovementSpeedStart = MovementSpeed;
        MovementSpeed = 10f;
        IKTarget.localPosition = new Vector3(IKTarget.localPosition.x, IKTarget.localPosition.y, IKTarget.localPosition.z - 1); //IKTarget.localPosition - (Gun.forward * 1.1f);// - (IKTarget.up * 2.75f);
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
        IKTarget.localPosition = new Vector3(IKTarget.localPosition.x, IKTarget.localPosition.y, IKTarget.localPosition.z + 1); // IKTarget.localPosition + (Gun.forward * 1.1f);// + (IKTarget.up * 2.75f);
        MovementSpeed = MovementSpeedStart;
    }

    void MoveTarget()
    {
        float moveAmount = 1.2f * (PlayerManager.playerDistance(Gun) / activateDistance);

        IKTarget.localPosition = new Vector3(IKTarget.localPosition.x, targetStart.y + moveAmount, IKTarget.localPosition.z);
    }

    void UpdateIK()
    {
        // Smooths the IK Motion
        IKCurrent.position = Vector3.Lerp(IKCurrent.position, IKTarget.position, MovementSpeed * Time.deltaTime);
        if(IKCurrent.position.y < LowerArm.position.y)
        {
            IKCurrent.position = new Vector3(IKCurrent.position.x, LowerArm.position.y, IKCurrent.position.z);
        }

        // Sets the unknown Side A of the arms-to-base Triangle
        ikSideA = Vector3.Distance(LowerArm.position, IKCurrent.position);
        ikSideA = ikSideA > (ikSideB + ikSideC - 0.01f) ? (ikSideB + ikSideC - 0.01f) : ikSideA;

        // Finds the angles of the arms-to-base Triangle
        float angleA = (180 / Mathf.PI) * Mathf.Acos(((ikSideB * ikSideB) + (ikSideC * ikSideC) - (ikSideA * ikSideA)) / (2 * ikSideB * ikSideC));
        float angleB = (180 / Mathf.PI) * Mathf.Acos(((ikSideC * ikSideC) + (ikSideA * ikSideA) - (ikSideB * ikSideB)) / (2 * ikSideC * ikSideA));
        float angleC = 180 - angleA - angleB;

        // Checks to see if the arm-to-base triangle angles are too small
        angleA = angleA > 1 ? angleA : 1;
        angleB = angleB > 1 ? angleB : 1;
        angleC = angleC > 1 ? angleC : 1;

        // Updates the middle angle
        UpperArm.localEulerAngles = new Vector3(180 - angleA, 0, 0);

        /*
         Finds the gun-to-base angle
         */

        float offsetSideA = IKCurrent.position.y - LowerArm.position.y;
        float offsetSideB = Vector2.Distance(new Vector2(IKCurrent.position.x, IKCurrent.position.z), new Vector2(LowerArm.position.x, LowerArm.position.z));
        float offsetSideC = Mathf.Sqrt((offsetSideA * offsetSideA) + (offsetSideB * offsetSideB));

        float offsetAngleA = (180 / Mathf.PI) * Mathf.Atan(offsetSideA / offsetSideB);

        // Decides if offset is positive or negitive
        bool isPos = IKCurrent.InverseTransformPoint(LowerArm.position).z > 0;

        // Set Lower Angle

        if (!isPos)
        {
            float armAngle = 90 -angleC - offsetAngleA; 
            LowerArm.localEulerAngles = new Vector3(armAngle, 0, 0);
        }
        else
        {
            float armAngle =  -(angleC + (90 -offsetAngleA));
            armAngle = armAngle < -95 ? -95 : armAngle;
            LowerArm.localEulerAngles = new Vector3(armAngle, 0, 0);
        }

        if (activated) RotateGunTowardsPlayer();
    }

    void RotateGunTowardsPlayer()
    {
        float sideA = Gun.position.y - PlayerManager.cameraPosition().y;
        float sideC = PlayerManager.cameraDistance(Gun);

        float angle;
        angle = 90 - ((180 / Mathf.PI) * Mathf.Acos(sideA / sideC));

        float lowerArmAngle = LowerArm.localEulerAngles.x;
        lowerArmAngle = (lowerArmAngle > 180) ? lowerArmAngle - 360 : lowerArmAngle;

        float upperArmAngle = ConvertQuanToEuler(UpperArm.localRotation).x;

        float startOffset = -(lowerArmAngle + upperArmAngle);
        Quaternion targetRotation = Quaternion.Euler(startOffset + angle, 0, 0);

        Gun.localRotation = Quaternion.RotateTowards(Gun.localRotation, targetRotation, rotationSpeed);
    }

    Vector3 ConvertQuanToEuler(Quaternion input)
    {
        float x = input.x;
        float y = input.y;
        float z = input.z;
        float w = input.w;

        float t0 = 2 * (w * x + y * z);
        float t1 = 1 - 2 * (x * x + y * y);
        float roll_x = Mathf.Atan2(t0, t1);

        float t2 = 2 * (w * y - z * x);
        t2 = t2 > +1.0 ? 1 : t2;
        t2 = t2 < -1.0 ? -1 : t2;
        float pitch_y = Mathf.Asin(t2);

        float t3 = 2 * (w * z + x * y);
        float t4 = 1 - 2 * (y * y + z * z);
        float yaw_z = Mathf.Atan2(t3, t4);

        return new Vector3((180 / Mathf.PI) * roll_x, (180 / Mathf.PI) * pitch_y, (180 / Mathf.PI) * yaw_z);
    }

    void RotatePivotTowardsPlayer()
    {
        Vector3 targetVector = PlayerManager.playerPosition() - Pivot.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetVector);
        Quaternion rotateFix = Quaternion.Lerp(Pivot.rotation, targetRotation, rotationSpeed * 2 * Time.deltaTime);
        rotateFix.eulerAngles = new Vector3(0, rotateFix.eulerAngles.y, 0);
        Pivot.rotation = rotateFix;
    }

    void InActive()
    {
        activated = false;
        Pivot.rotation = Quaternion.Slerp(Pivot.rotation, pivotStartRotation, rotationSpeed * Time.deltaTime);
        Gun.localRotation = Quaternion.Lerp(Gun.localRotation, gunStartRotation, rotationSpeed * 0.5f * Time.deltaTime);
        IKTarget.localPosition = targetStart;
    }

    bool PlayerInDistance()
    {
        return PlayerManager.playerDistance(transform) < activateDistance;
    }
}
