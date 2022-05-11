using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    [SerializeField]
    Transform shootPoint;

    [SerializeField]
    Rigidbody bulletPreFab;

    [SerializeField]
    float cooldownTime = 0.1f;

    [SerializeField]
    int ammo = 20;

    Camera cam;

    bool isAttached = false;
    bool canShoot = true;

    Rigidbody rigidbody;
    BoxCollider boxCollider;
    SphereCollider sphereCollider;

    string currentElement = "Fire";

    Mouse mouse;

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        cam = Camera.main;
        mouse = Mouse.current;
    }
    void Update()
    {
        if (isAttached)
        {
            PointGun();
            if (mouse == null) return;
            if (mouse.leftButton.isPressed && canShoot)
            {
                StartCoroutine(ShootCooldown());
                UIUpdater.targetScale = Vector2.one * 150;
                Rigidbody newBullet = Instantiate(bulletPreFab, shootPoint.position, transform.rotation);
                newBullet.AddForce(transform.forward * 30f, ForceMode.Impulse);
                newBullet.GetComponent<PlayerBullet>().damage = 1;
                PlayerManager.UseAmmo(1, currentElement);
                ammo -= 1;
                if(ammo <= 0)
                {
                    Destroy(gameObject);
                    PlayerManager.DisArm();
                }
            }

            if (mouse.rightButton.isPressed)
            {
                rigidbody.isKinematic = false;
                StartCoroutine(PickUpWait());
                isAttached = false;
                transform.SetParent(null);
                rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);
                PlayerManager.UseAmmo(ammo, currentElement);
                PlayerManager.DisArm();
            }
        }
    }

    IEnumerator PickUpWait()
    {
        yield return new WaitForSeconds(0.25f);
        boxCollider.enabled = true;
        sphereCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttached)
        {
            if (!PlayerManager.IsArmed())
            {
                Debug.Log(other.transform.tag);
                if (other.transform.tag == "Player")
                {
                    rigidbody.isKinematic = true;
                    boxCollider.enabled = false;
                    sphereCollider.enabled = false;
                    isAttached = true;
                    transform.SetParent(PlayerManager.PlayerTranform());
                    transform.localPosition = new Vector3(0.5f, 1, 0);
                    PlayerManager.AddAmmo(ammo, currentElement);
                    PlayerManager.Arm();
                }
            }
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }

    void PointGun()
    {
        RaycastHit hit;
        Ray targetRay = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(targetRay, out hit))
        {
            if (hit.transform.tag != "Player")
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(hit.point - transform.position), Time.deltaTime * 30f);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, Time.deltaTime * 30f);
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, Time.deltaTime * 30f);
        }
    }
}
