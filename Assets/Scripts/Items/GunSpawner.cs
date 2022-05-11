using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpawner : MonoBehaviour
{
    [SerializeField]
    Rigidbody gunPrefab;

    [SerializeField]
    Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player" && !PlayerManager.IsArmed())
        {
            Rigidbody newGun = Instantiate(gunPrefab, spawnPoint.position, Quaternion.EulerAngles(270,0,0));
            newGun.AddForce(Vector3.up * 4, ForceMode.Impulse);
        }
    }
}
